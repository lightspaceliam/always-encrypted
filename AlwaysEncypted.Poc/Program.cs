using AlwaysEncrypted.Entities;
using Microsoft.EntityFrameworkCore;
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
                  services.AddTransient<AdoStub>();

                  services.AddDbContext<AlwaysEncryptedDbContext>(options =>
                  {
                      options.UseSqlServer(
                        hostContext.Configuration["ConnectionStrings:LinqSqlConnectionString"],
                        optionsBuilder =>
                        {
                            optionsBuilder.ExecutionStrategy(
                                context => new SqlServerRetryingExecutionStrategy(context, 10, TimeSpan.FromMilliseconds(200), null));
                        });
                  });

                  services.AddTransient<LinqStub>();
              });

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                try
                {
                    var adoStub = services.GetRequiredService<AdoStub>();
                    await adoStub.Run();
                    var linqStub = services.GetRequiredService<LinqStub>();
                    await linqStub.Run();
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
