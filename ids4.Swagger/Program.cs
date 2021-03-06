using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace ids4.Swagger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).UseSerilog((ctx, config) =>
            {
                config.MinimumLevel.Debug()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Verbose)
                    .Enrich.FromLogContext();

                if (ctx.HostingEnvironment.IsDevelopment())
                {
                    config.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}");
                }
                //else if (ctx.HostingEnvironment.IsProduction())
                //{
                //    config.WriteTo.File(@"D:\home\LogFiles\Application\identityserver.txt",
                //        fileSizeLimitBytes: 1_000_000,
                //        rollOnFileSizeLimit: true,
                //        shared: true,
                //        flushToDiskInterval: TimeSpan.FromSeconds(1));
                //}
            }).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
