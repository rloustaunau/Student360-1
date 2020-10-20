
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Providers.Image
{
    public class ConventionBasedImageProvider : IImageProvider
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _env;

        public ConventionBasedImageProvider(IConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public async Task<string> GetStudentImageUrlAsync(string studentUniqueId)
        {
            var file = getFileName(studentUniqueId.Trim(), _config["ImagePaths:PhysicalPath"] +_config["ImagePaths:Student"]);

            if (file == null)
                return _config["ImagePaths:Default"];

            return _config["ImagePaths:Student"].Replace(_config["ImagePaths:PhysicalPath"], "") + file;
        }

        private string getFileName(string uniqueId, string path)
        {
            // Convention based uploaded file name.
            var physicalFilePath = Path.Combine(_env.ContentRootPath, path);

            // Get all files that match the convention no matter the extension.
            var files = Directory.GetFiles(physicalFilePath, uniqueId + ".*");

            if (files.Length > 0)
                return Path.GetFileName(files.FirstOrDefault());

            return null;
        }
    }
}
