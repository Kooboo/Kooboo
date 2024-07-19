using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Lib.Utilities.UAParser
{

    //Define value here in order save storage space.
    public class UAConstants
    {
        public static UAConstants Instance { get; set; } = new UAConstants();

        private readonly object _locker = new();

        private Dictionary<string, byte> _device;
        private Dictionary<string, byte> Devices
        {
            get
            {
                if (_device == null)
                {
                    lock (_locker)
                    {
                        if (_device == null)
                        {
                            Dictionary<string, byte> Values = new Dictionary<string, byte>();
                            Values.Add("Apple", 1);
                            Values.Add("Honor", 2);
                            Values.Add("Huawei", 3);
                            Values.Add("Meizu", 4);
                            Values.Add("XiaoMi", 5);
                            Values.Add("OnePlus", 6);
                            Values.Add("Oppo", 7);
                            Values.Add("Samsung", 8);
                            Values.Add("Sony", 9);
                            Values.Add("Vertu", 10);
                            Values.Add("Vivo", 11);
                            _device = Values;
                        }
                    }
                }
                return _device;
            }
        }


        public string GetDeviceName(byte Id)
        {
            var item = Devices.Where(o => o.Value == Id).FirstOrDefault();
            return item.Key == null ? "Others" : item.Key;
        }

        public byte GetDeviceByte(string name)
        {
            return Devices.ContainsKey(name) ? Devices[name] : (byte)0;
        }

        private Dictionary<string, byte> _mailApp;
        private Dictionary<string, byte> MailApp
        {
            get
            {
                if (_mailApp == null)
                {
                    lock (_locker)
                    {
                        if (_mailApp == null)
                        {
                            Dictionary<string, byte> Values = new Dictionary<string, byte>();
                            Values.Add("AirMail", 31);
                            Values.Add("Apple Mail", 32);
                            Values.Add("Gmail", 33);
                            Values.Add("Outlook", 34);
                            Values.Add("Postbox", 35);
                            Values.Add("QQMail", 36);
                            Values.Add("Superhuman", 37);
                            Values.Add("Thunderbird", 38);
                            Values.Add("Windows Live Mail", 39);
                            Values.Add("Yahoo", 40);
                            _mailApp = Values;
                        }
                    }
                }
                return _mailApp;
            }
        }

        public string GetMailAppName(byte Id)
        {
            var item = MailApp.Where(o => o.Value == Id).FirstOrDefault();
            return item.Key == null ? "Others" : item.Key;
        }

        public byte GetMailAppByte(string name)
        {
            return MailApp.ContainsKey(name) ? MailApp[name] : (byte)0;
        }


        private Dictionary<string, byte> _Platform;
        private Dictionary<string, byte> Platform
        {
            get
            {
                if (_Platform == null)
                {
                    lock (_locker)
                    {
                        if (_Platform == null)
                        {
                            Dictionary<string, byte> Values = new Dictionary<string, byte>();
                            Values.Add("Desktop", 61);
                            Values.Add("Mobile", 62);
                            Values.Add("Tablet", 63);
                            _Platform = Values;
                        }
                    }
                }
                return _Platform;
            }
        }

        public string GetPlatformName(byte Id)
        {
            var item = Platform.Where(o => o.Value == Id).FirstOrDefault();
            return item.Key == null ? "Others" : item.Key;
        }

        public byte GetPlatformByte(string name)
        {
            return Platform.ContainsKey(name) ? Platform[name] : (byte)0;
        }


        private Dictionary<string, byte> _OS;
        private Dictionary<string, byte> OS
        {
            get
            {
                if (_OS == null)
                {
                    lock (_locker)
                    {
                        if (_OS == null)
                        {
                            Dictionary<string, byte> Values = new Dictionary<string, byte>();
                            Values.Add("Android", 71);
                            Values.Add("IOS", 72);
                            Values.Add("IPAD", 73);
                            Values.Add("Linux", 74);
                            Values.Add("Mac", 75);
                            Values.Add("Windows", 76);
                            _OS = Values;
                        }
                    }
                }
                return _OS;
            }
        }

        public string GetOSName(byte Id)
        {
            var item = OS.Where(o => o.Value == Id).FirstOrDefault();
            return item.Key == null ? "Others" : item.Key;
        }

        public byte GetOSByte(string name)
        {
            return OS.ContainsKey(name) ? OS[name] : (byte)0;
        }


        private Dictionary<string, byte> _Browser;
        private Dictionary<string, byte> Browser
        {
            get
            {
                if (_Browser == null)
                {
                    lock (_locker)
                    {
                        if (_Browser == null)
                        {
                            Dictionary<string, byte> Values = new Dictionary<string, byte>();
                            Values.Add("360 Browser", 81);
                            Values.Add("Baidu Spider", 82);
                            Values.Add("Bing bot", 83);
                            Values.Add("ChatGPT", 84);
                            Values.Add("Chrome", 85);
                            Values.Add("Firefox", 86);
                            Values.Add("Google Bot", 87);
                            Values.Add("GPT Bot", 88);
                            Values.Add("Microsoft Edge", 89);
                            Values.Add("Safari", 90);
                            Values.Add("Yandex Box", 91);
                            _Browser = Values;
                        }
                    }
                }
                return _Browser;
            }
        }

        public string GetBrowserName(byte Id)
        {
            var item = Browser.Where(o => o.Value == Id).FirstOrDefault();
            return item.Key == null ? "Others" : item.Key;
        }

        public byte GetBrowserByte(string name)
        {
            return Browser.ContainsKey(name) ? Browser[name] : (byte)0;
        }


    }
}
