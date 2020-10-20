using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMCISD.Student360.Resources.Services.People;

namespace SMCISD.Student360.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrentUserController : ControllerBase
    {
        private readonly IPeopleService _peopleService;
        public CurrentUserController(IPeopleService peopleService)
        {
            _peopleService = peopleService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<PeopleModel>>  GetProfile()
        {
            return await _peopleService.Get(User.FindFirst(x => x.Type.Contains("email")).Value);
        }
    }
}
