using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SMCISD.Student360.Resources.Providers;
using SMCISD.Student360.Resources.Services.People;
using SMCISD.Student360.Web.Attributes;

namespace SMCISD.Student360.Api.Controllers.OAuth
{
    [NoAuthFilter]
    [ApiController]
    [Route("[controller]")]
    public class OAuthController : ControllerBase
    {
        private readonly ILogger<OAuthController> _logger;
        private readonly IConfiguration _config;
        private readonly ITokenValidationProvider _tokenValidationProvider;
        private readonly IPeopleService _peopleService;

        public OAuthController(ILogger<OAuthController> logger, IConfiguration config, ITokenValidationProvider tokenValidationProvider, IPeopleService peopleService)
        {
            _logger = logger;
            _config = config;
            _tokenValidationProvider = tokenValidationProvider;
            _peopleService = peopleService;
        }

        [HttpPost("exchangeToken"), AllowAnonymous]
        public async Task<ActionResult<string>> ExchangeToken([FromBody]OAuthTokenExchangeRequest request)
        {
            if (request.Grant_type != "client_credentials")
                return Unauthorized($"grant_type ({request.Grant_type}) not supported");

            // If token valid then continue...
            var identity = await _tokenValidationProvider.ValidateTokenAsync(request.Id_token);
            
            if (identity == null)
                return Unauthorized();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var accessTokenExpireTime = Convert.ToInt32(_config["Jwt:AccessToken:ExpiresInMinutes"]);
            var refreshTokenExpireTime = Convert.ToInt32(_config["Jwt:RefreshToken:ExpiresInDays"]);

            var claims = await GetClaims(identity);

            if (claims == null)
                return StatusCode(401, "There is no identity with that email.");

            var token = new JwtSecurityToken(
                  _config["Jwt:Issuer"],
                  _config["Jwt:Audience"],
                  expires: DateTime.Now.AddMinutes(accessTokenExpireTime),
                  claims: await GetClaims(identity),
                  signingCredentials: creds
              );

            var refreshToken = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                expires: DateTime.Now.AddDays(refreshTokenExpireTime),
                claims: new List<Claim> { new Claim(JwtRegisteredClaimNames.Email, identity.ElectronicMailAddress) },
                signingCredentials: creds
            );

            var model = new OAuthResponse
            {
                Access_token = await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(token)),
                Expires_in = accessTokenExpireTime * 60, // Returning seconds - Based on the specification https://tools.ietf.org/html/rfc6749#page-30
                Token_type = "Bearer",
                Refresh_token = await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(refreshToken)),
            };

            return Ok(model);
        }

        private async Task<IEnumerable<Claim>> GetClaims(IdentityProviderModel identity)
        {

            var identityData = await _peopleService.Get(identity.ElectronicMailAddress);

            if (identityData == null) {
                return null;
            }

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.NameId, identityData.UniqueId.ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, identityData.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, identityData.LastSurname),
                new Claim(JwtRegisteredClaimNames.Email, identityData.ElectronicMailAddress),

                // Add custom claims
                new Claim("person_usi", identityData.Usi.ToString(), ClaimValueTypes.Integer),
                new Claim("person_unique_id", identityData.UniqueId.ToString(), ClaimValueTypes.String),
                new Claim("firstname", identityData.FirstName, ClaimValueTypes.String),
                new Claim("lastsurname", identityData.LastSurname, ClaimValueTypes.String),
                new Claim("ed_org_associations", JsonConvert.SerializeObject(identityData.EdOrgAssociations), ClaimValueTypes.String),
                new Claim("role", identityData.Role, ClaimValueTypes.String),
                new Claim("access_level", identityData.AccessLevel, ClaimValueTypes.String),
                new Claim("level_id", identityData.LevelId.ToString(), ClaimValueTypes.Integer)
            };

            return claims;
        }
    }
}
