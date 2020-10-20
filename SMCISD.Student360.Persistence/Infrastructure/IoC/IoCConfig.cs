using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using System.Linq;

namespace SMCISD.Student360.Persistence.Infrastructure.IoC
{
    public static class IoCConfig
    {
        public static void RegisterDependencies(IServiceCollection container, IConfiguration configuration)
        {
            container.AddDbContext<Student360Context>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            RegisterCommandsAndQueriesByConvention<IPersistenceMarker>(container);
        }

        private static void RegisterCommandsAndQueriesByConvention<TMarker>(IServiceCollection container)
        {
            var types = typeof(TMarker).Assembly.ExportedTypes;

            var commandsToRegister = (
                from interfaceType in types.Where(t => t.Name.StartsWith("I") && t.Name.EndsWith("Commands"))
                from serviceType in types.Where(t => t.Name == interfaceType.Name.Substring(1))
                select new
                {
                    InterfaceType = interfaceType,
                    ServiceType = serviceType
                }
            );

            var queriesToRegister = (
                from interfaceType in types.Where(t => t.Name.StartsWith("I") && t.Name.EndsWith("Queries"))
                from serviceType in types.Where(t => t.Name == interfaceType.Name.Substring(1))
                select new
                {
                    InterfaceType = interfaceType,
                    ServiceType = serviceType
                }
            );

            container.AddScoped<IAuthenticationProvider, AuthenticationProvider>();

            foreach (var pair in commandsToRegister)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);

            foreach (var pair in queriesToRegister)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);
        }
    }
}
