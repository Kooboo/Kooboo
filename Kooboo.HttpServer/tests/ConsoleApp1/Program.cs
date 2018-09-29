using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers;
using System.IO.Pipelines;
using System.Security.Cryptography.X509Certificates;

using Kooboo.HttpServer;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var bytes = File.ReadAllBytes(@"C:\Projects\Yardi\Kooboo2015\src\Kooboo.HttpServer\tests\ConsoleApp1\test.binary");
            var s = Encoding.UTF8.GetString(bytes);

            var ascii = Encoding.ASCII.GetString(bytes);

            var iso88591 = Encoding.GetEncoding("iso-8859-1").GetString(bytes);

            //var options = new HttpServerOptions()
            //{
            //    SslCertificateProvider = new KoobooCertificateManager(),
            //    IsHttps=true
            //};
            //var server = new HttpServer(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 443), options);
            var server = new HttpServer(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 81));
            server.Start();

            Console.WriteLine($"Server started on http://localhost:{server.EndPoint.Port}");

            Console.ReadKey();
        }

    }

    //public class KoobooCertificateManager:ISslCertificateProvider
    //{
    //    //cache certificates in memory
    //    private static Dictionary<string, X509Certificate> ServerCertificates = new Dictionary<string, X509Certificate>();
    //    private static Object certificateRequestLockObj = new object();
    //    private static Dictionary<string, bool> CertificateRequestDic = new Dictionary<string, bool>();
    //    public async Task<X509Certificate> SelectCertificate(string hostName)
    //    {
    //        var key = hostName.ToLower();
    //        X509Certificate serverCertificate = null;
    //        if (!ServerCertificates.TryGetValue(key, out serverCertificate))
    //        {
    //            bool isRequesting = false;

    //            if (!CertificateRequestDic.TryGetValue(key, out isRequesting))
    //            {
    //                lock (certificateRequestLockObj)
    //                {
    //                    if (!CertificateRequestDic.TryGetValue(key, out isRequesting))
    //                    {
    //                        isRequesting = true;
    //                        CertificateRequestDic.Add(key, isRequesting);
    //                    }
    //                }
    //                if (isRequesting)
    //                {
    //                    try
    //                    {
    //                        //await can't be in lock
    //                        //if certificate doesn't exist,get from server.
    //                        serverCertificate = await CertificateService.GetCertificate(key);
    //                        if (serverCertificate != null)
    //                            ServerCertificates.Add(key, serverCertificate);
    //                    }
    //                    catch (Exception ex)
    //                    {

    //                    }
    //                    finally
    //                    {
    //                        CertificateRequestDic.Remove(key);
    //                    }
    //                }
    //            }

    //        } 
    //        return serverCertificate;
    //    }
    //}
}
