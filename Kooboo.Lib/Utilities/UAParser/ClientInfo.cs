using System;
using System.Net;
using Kooboo.Lib.Extensions;

namespace Kooboo.Lib.Utilities.UAParser
{
    public class ClientInfo
    {
        public ClientInfo(string platform, string os, ApplicationInfo application, string device)
        {
            Platform = platform;
            OS = os;
            Application = application;
            Device = device;
        }

        public ClientInfo()
        {

        }

        public string Platform { get; set; }
        public string OS { get; set; }

        private ApplicationInfo _appInfo;
        public ApplicationInfo Application
        {
            get
            {
                if (_appInfo == null)
                {
                    _appInfo = new ApplicationInfo();
                }
                return _appInfo;
            }
            set
            {
                _appInfo = value;
            }
        }
        public string Device { get; set; }

        public override string ToString()
        {
            return Platform + "-" + OS + "-" + Device + "-" + Application;
        }

        //for better storage. 
        public int ToInt()
        {
            byte platform = UAConstants.Instance.GetPlatformByte(Platform);
            byte os = UAConstants.Instance.GetOSByte(OS);
            byte device = UAConstants.Instance.GetDeviceByte(Device);
            byte app;

            if (Application != null && Application.IsWebBrowser)
            {
                app = UAConstants.Instance.GetBrowserByte(Application.Name);
            }
            else
            {
                app = UAConstants.Instance.GetMailAppByte(Application.Name);
            }

            byte[] IntBytes = new byte[4];

            IntBytes[0] = platform;
            IntBytes[1] = os;
            IntBytes[2] = device;
            IntBytes[3] = app;

            return BitConverter.ToInt32(IntBytes);

        }


        public static ClientInfo FromInt(int value)
        {
            ClientInfo info = new ClientInfo();
            byte[] Bytes = BitConverter.GetBytes(value);

            info.Platform = UAConstants.Instance.GetPlatformName(Bytes[0]);
            info.OS = UAConstants.Instance.GetOSName(Bytes[1]);

            info.Device = UAConstants.Instance.GetDeviceName(Bytes[2]);

            if (Bytes[3] >= 80)
            {
                var appName = UAConstants.Instance.GetBrowserName(Bytes[3]);
                info.Application = new ApplicationInfo() { Name = appName, IsWebBrowser = true };
            }
            else
            {
                var appName = UAConstants.Instance.GetMailAppName(Bytes[3]);
                info.Application = new ApplicationInfo() { Name = appName };
            }
            return info;
        }


        public static bool IsGoogleBot(string VerifyIP)
        {
            // TODO: See: https://developers.google.com/search/docs/crawling-indexing/verifying-googlebot
            var googleIPList = GetGoogleBotIPList();
            // If you can't get google's ip list, return true
            if (googleIPList == null)
                return true;

            foreach (var item in googleIPList.Prefixes)
            {
                // Because the Google IP list is given in CIDR format, you need to determine whether the IP is in CIDR format range
                if (IsIPInCIDRRange(VerifyIP, item.ToString()))
                    return true;
            }
            return false;
        }

        public int CacheDays = 30;
        static string BaseFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");


        private static IPList GetGoogleBotIPList()
        {
            var googleIPList = new GoogleIPList();
            return googleIPList.ReadAllLines();
        }

        private static bool IsIPInCIDRRange(string ipAddress, string cidr)
        {
            try
            {
                // Convert the IP address and CIDR to the corresponding object
                IPAddress ip = IPAddress.Parse(ipAddress);
                string[] cidrParts = cidr.Split('/');
                IPAddress cidrIP = IPAddress.Parse(cidrParts[0]);
                int subnetMaskLength = int.Parse(cidrParts[1]);

                // Use the IsInRange method of IPAddressExtensions to check whether they are in the CIDR range
                return ip.IsInRange(cidrIP, subnetMaskLength);
            }
            catch (Exception)
            {
                // Handling exceptions (such as invalid IP addresses or CIDR ranges)
                // output Logger
                // Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }

}
