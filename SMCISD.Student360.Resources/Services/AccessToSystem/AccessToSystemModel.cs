using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.AccessToSystem
{
    public class AccessToSystemModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime LastLogin { get; set; }
        public string SchoolCode { get; set; }
    }
}
