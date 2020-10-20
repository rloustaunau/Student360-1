using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMCISD.Student360.Resources.Google.Providers;
using SMCISD.Student360.Resources.Providers;
using SMCISD.Student360.Resources.Providers.Image;

namespace SMCISD.Student360.Api.Infrastructure.IoC
{
    public static class IoCConfig
    {
        public static void RegisterDependencies(IServiceCollection container, IConfiguration configuration)
        {
            // Register other dependencies
            RegisterProviders(container);

            // Register resources/services dependencies
            Resources.Infrastructure.IoC.IoCConfig.RegisterDependencies(container, configuration);
        }

        private static void RegisterProviders(IServiceCollection container)
        {
            // Register the Token Validator
            container.AddScoped<ITokenValidationProvider, GoogleTokenValidationProvider>();
            container.AddScoped<IImageProvider, ConventionBasedImageProvider>();
        }
    }
}
