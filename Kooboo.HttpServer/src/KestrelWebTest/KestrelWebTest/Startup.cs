using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KestrelWebTest
{
    class Startup
    {
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {

            app.Run(async context =>
            {
                var response = "hello, world";
                context.Response.ContentLength = response.Length;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(response);
            });
        }
        public static Task Main(string[] args)
        {
            var hostBuilder = new WebHostBuilder()
                .UseKestrel((context, options) =>
                {
                    var basePort = 91;
                    //options.ConfigureEndpointDefaults(opt =>
                    //{
                    //    opt
                    //});
                    //options.ConfigureEndpointDefaults(opt =>
                    //{
                    //    opt.Protocols = HttpProtocols.Http1;
                    //});

                    options.ApplicationSchedulingMode = SchedulingMode.Inline;

                    options.Listen(IPAddress.Loopback, basePort, listenOptions =>
                    {

                    });



                    options.UseSystemd();
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>();

            hostBuilder.UseSockets();

            //hostBuilder.Build().Run();
            return hostBuilder.Build().RunAsync();

        }
    }
}
