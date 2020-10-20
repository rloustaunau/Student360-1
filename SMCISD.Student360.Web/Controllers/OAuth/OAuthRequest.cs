using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Api.Controllers.OAuth
{
    public class OAuthRequest
    {
        public string Client_id { get; set; }
        public string Client_secret { get; set; }
        public string Grant_type { get; set; }
    }
}
