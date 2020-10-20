using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Providers
{
    public interface ITokenValidationProvider
    {
        Task<IdentityProviderModel> ValidateTokenAsync(string token);
    }
}
