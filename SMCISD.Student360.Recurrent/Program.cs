using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.IO;

namespace SMCISD.Student360.Recurrent
{
    class Program
    {
        public static IConfigurationRoot Configuration;
        static void Main(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true);

            var notificationEndPoint = Configuration["NotificationsApi"];

            if (environment == "Development")
            {

                builder
                    .AddJsonFile(
                        Path.Combine(AppContext.BaseDirectory, string.Format("..{0}..{0}..{0}", Path.DirectorySeparatorChar), $"appsettings.{environment}.json"),
                        optional: true
                    );
            }
            else
            {
                builder
                    .AddJsonFile($"appsettings.{environment}.json", optional: false);
            }

            var client = new RestClient(notificationEndPoint);
            var request = new RestRequest(Method.GET);

            System.Console.WriteLine($"-> Calling endpoint: {notificationEndPoint}");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var response = client.Execute(request);
            watch.Stop();

            System.Console.WriteLine($"--> Status:{response.StatusDescription}, Content: {response.Content} in ({watch.ElapsedMilliseconds}ms)");

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                Environment.Exit(-1);
#if DEBUG
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
#endif
        }
    }
}
