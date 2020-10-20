using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SMCISD.Student360.Api.Controllers
{
    [ApiController]
    [Route("api/samples/[controller]")]
    public class SecuredController : ControllerBase
    {
        private readonly ILogger<SecuredController> _logger;

        public SecuredController(ILogger<SecuredController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<SecureResourcemodel>>  Get()
        {
            var model = await Task.Run(() => new SecureResourcemodel { Text = "Very Secure Resource Model" } );

            if (model==null)
                return NotFound();

            return Ok(model);
        }

        public class SecureResourcemodel
        {
            public string Text { get; set; }
        }
    }
}
