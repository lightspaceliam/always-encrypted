using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AlwaysEncypted.Poc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
              .ConfigureAppConfiguration((hostContext, configApp) =>
              {
                  configApp.SetBasePath(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName).FullName);
                  configApp.AddJsonFile("appsettings.json", optional: true);

                  configApp.AddJsonFile($"appsettings.Development.json");

              })
              .ConfigureServices((hostContext, services) =>
              {
                  services.AddLogging(configure => configure.AddConsole());

                  services.AddTransient<Stub>();
              });

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                try
                {
                    var stub = services.GetRequiredService<Stub>();
                    
                    await stub.Run();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
