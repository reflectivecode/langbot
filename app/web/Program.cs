using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DbUp;
using DbUp.Engine.Output;
using DbUp.SQLite.Helpers;
using LangBot.Web.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LangBot.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            MigrateDatabase(host);
            await RunWebHost(host);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .UseStartup<Startup>();
        }

        public static void MigrateDatabase(IWebHost host)
        {
            var logger = host.Services.GetRequiredService<ILogger<Startup>>();
            try
            {
                var options = host.Services.GetRequiredService<IOptions<DatabaseOptions>>();
                if (!String.IsNullOrEmpty(options.Value.DeleteOnStart))
                {
                    logger.LogInformation("Deleting database file {Database}", options.Value.DeleteOnStart);
                    File.Delete(options.Value.DeleteOnStart);
                }

                var connection = host.Services.GetRequiredService<IDbConnection>();
                var upgradeLog = host.Services.GetRequiredService<IUpgradeLog>();
                var sharedConnection = new SharedConnection(connection);
                var upgradeEngine = DeployChanges.To
                    .SQLiteDatabase(sharedConnection)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogTo(upgradeLog)
                    .Build();

                var result = upgradeEngine.PerformUpgrade();
                if (!result.Successful)
                    throw result.Error;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error preparing database");
                throw;
            }
        }

        private static async Task RunWebHost(IWebHost host)
        {
            try
            {
                await host.RunAsync();
            }
            catch (Exception e)
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "Error running webhost");
                throw;
            }
        }
    }
}
