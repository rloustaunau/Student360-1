using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Api.Controllers.OAuth
{
    public class OAuthTokenExchangeRequest
    {
        public string Id_token { get; set; }
        public string Grant_type { get; set; }
    }
}
