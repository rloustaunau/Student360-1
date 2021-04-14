using SelectPdf;
using System.IO;

namespace SMCISD.Student360.Resources.Providers.Pdf
{
    public class PdfProvider : IPdfProvider
    {

        public byte[] GetPdfFromHtmlString(string htmlContent, string htmlStyles)
        {
            GlobalProperties.LicenseKey = "iqG7qri/u6q7vbiquLqkuqq5u6S7uKSzs7Oz";
            HtmlToPdf converter = new HtmlToPdf();
            var baseHtml = @$"
                                <html>
                                    <style>
                                        {htmlStyles}
                                    </style>
                                    <body>
                                         {htmlContent}
                                    </body>
                                </html>";

            PdfDocument doc = converter.ConvertHtmlString(baseHtml);
            
            // create memory stream to save PDF
            MemoryStream pdfStream = new MemoryStream();

            // save pdf document into a MemoryStream
            doc.Save(pdfStream);
            doc.Close();
            // reset stream position
            pdfStream.Position = 0;

            return pdfStream.ToArray();
        }
    }
}
