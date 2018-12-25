using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Lib.Helper
{
    public class SystemAdapter
    {
        public static void OpenDefaultUrl(string url)
        {
#if NET45

            Process.Start(url);
#endif
        }

        public static string GetMimeMapping(string extension)
        {
#if NET45
            return System.Web.MimeMapping.GetMimeMapping(extension);
#else
            return MimeMapping.MimeUtility.GetMimeMapping(extension);
#endif
        }

        public static int GetPort(int port)
        {
            //when linux or macos port is closed,it still have active tcp.
            //so only bind to the defaultPort.
            
            if (Kooboo.Lib.Helper.RuntimeSystemHelper.IsWindow())
            {
                while (Lib.Helper.NetworkHelper.IsPortInUse(port) && port < 65535)
                {
                    port += 1;
                }
            }
            return port;
        }

        public static bool IsPortInUsed(int port)
        {
            //when linux or macos port is closed,it still have active tcp.
            //so only bind to the defaultPort.

            if (Kooboo.Lib.Helper.RuntimeSystemHelper.IsWindow())
            {
                if (Lib.Helper.NetworkHelper.IsPortInUse(port))
                {
                    return true;
                }
            }
            return false;
            
        }

        public static void ConsoleWait()
        {
#if NETCOREAPP
            //Console.ReadLine will have exception in linux...
            if (!Kooboo.Lib.Helper.RuntimeSystemHelper.IsWindow())
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
#endif
            var line = Console.ReadLine();

            while (line != null)
            {
                Console.Write(line);
                line = Console.ReadLine();
            }
        }

        public static List<string> GetTryPaths()
        {
            List<string> trypaths = new List<string>();

            if (RuntimeSystemHelper.IsWindow())
            {
                trypaths.Add(@"..\..\..\..\Github\Kooboo.Web");
                trypaths.Add(@"..\..\");
                trypaths.Add(@"..\..\..\");
                trypaths.Add(@"..\..\..\..\");
            }
            else
            {
                trypaths.Add(@"../../../Github/Kooboo.Web");
                trypaths.Add(@"../");
                trypaths.Add(@"../../");
                trypaths.Add(@"../../../");
            }
            return trypaths;
        }


        public static double GetDistance(double xLa, double xLong, double yLa, double yLong)
        {
#if NET45
            System.Device.Location.GeoCoordinate cordx = new System.Device.Location.GeoCoordinate(xLa, xLong);
            System.Device.Location.GeoCoordinate cordy = new System.Device.Location.GeoCoordinate(yLa, yLong);
            return cordx.GetDistanceTo(cordy);
#else

            GeoCoordinatePortable.GeoCoordinate cordx = new GeoCoordinatePortable.GeoCoordinate(xLa, xLong);
            GeoCoordinatePortable.GeoCoordinate cordy = new GeoCoordinatePortable.GeoCoordinate(yLa, yLong);
            return cordx.GetDistanceTo(cordy);
#endif
        }

        public static void RegisterEncoding()
        {
            //register encoding like gb18030,because Encoding don't contain these encoding in dotnet standard by default
#if NETSTANDARD
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        public static string GetUpgradeUrl(string convertApiUrl)
        {
            if (!Kooboo.Lib.Helper.RuntimeSystemHelper.IsWindow())
            {
                return convertApiUrl + "/_api/converter/LinuxServerPackage";
            }
            else
            {
                return convertApiUrl + "/_api/converter/WindowServerPackage";
            }
        }


        public static int GetEndLine(string source,int index)
        {
            int EndLine = -1;
            if (Kooboo.Lib.Helper.RuntimeSystemHelper.IsWindow())
            {
                EndLine = source.IndexOf("\n", index);
            }
            else
            {
                //linux source.IndexOf("\n", index) will be -1;
                //only source.IndexOf("\r\n", index) can get the index
                EndLine = source.IndexOf("\r\n", index) + 1;
            }
            return EndLine;
        }


    }
}
