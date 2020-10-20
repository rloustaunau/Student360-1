using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Providers.Image
{
    public interface IImageProvider
    {
        Task<string> GetStudentImageUrlAsync(string studentUniqueId);
    }
}
