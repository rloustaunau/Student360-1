using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Providers.Pdf
{
    public interface IPdfProvider
    {
        byte[] GetPdfFromHtmlString(string htmlContent, string htmlStyles);
    }
}
