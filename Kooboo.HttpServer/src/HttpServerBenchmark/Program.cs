using System;
using Kooboo.HttpServer;

namespace HttpServerBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new HttpServerOptions()
            {
                // SslCertificateProvider = new KoobooCertificateManager(),
                //IsHttps=true
            };
            var server = new HttpServer(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 81), options);
            //var server = new HttpServer(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 81));
            server.Start();

            Console.WriteLine($"Server started on http://localhost:{server.EndPoint.Port}");

            Console.ReadKey();
        }
    }
}
