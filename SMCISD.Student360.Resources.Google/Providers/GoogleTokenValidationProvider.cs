using Google.Apis.Auth;
using SMCISD.Student360.Resources.Providers;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Google.Providers
{
    public class GoogleTokenValidationProvider : ITokenValidationProvider
    {
        public async Task<IdentityProviderModel> ValidateTokenAsync(string token)
        {
            var validPayload = await GoogleJsonWebSignature.ValidateAsync(token);

            if (validPayload == null)
                return null;

            return new IdentityProviderModel {
                FirstName = validPayload.GivenName,
                LastSurname = validPayload.FamilyName,
                ElectronicMailAddress = validPayload.Email,
                PictureUrl = validPayload.Picture
            }; 
        }
    }
}
