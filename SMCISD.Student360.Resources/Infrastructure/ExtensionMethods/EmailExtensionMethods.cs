using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMCISD.Student360.Resources.Infrastructure.ExtensionMethods
{
    public static class EmailExtensionMethods
    {
        /// <summary>Gets the .</summary>
        /// <param name="emails">The list of emails.</param>
        /// <returns>The primary email. If none marked as primary then a default existing one.</returns>
        public static string GetPrimaryOrDefaultEmail(this List<ElectronicMailModel> emails)
        {
            if (emails.Any(x => x.PrimaryEmailAddressIndicator == true))
                return emails.Single(x => x.PrimaryEmailAddressIndicator == true).ElectronicMailAddress;

            return emails.Any() ? emails.First().ElectronicMailAddress : null;
        }
    }
}
