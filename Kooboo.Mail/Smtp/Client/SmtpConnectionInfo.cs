//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail.Smtp
{
    public class SmtpConnectionInfo : ConnectionInfo
    {
        protected override int DefaultPort
        {
            get
            {
                return 25;
            }
        }

        protected override bool GetEnableSSLByPort(int port)
        {
            return port == 465 || Port == 587;
        }

        protected override int GetPortByEnableSSL(bool enableSSL)
        {
            return enableSSL ? 587 : 25;
        }

        public static SmtpConnectionInfo Parse(string str)
        {
            var result = new SmtpConnectionInfo();
            result.ParseSelf(str);
            return result;
        }

        public static SmtpConnectionInfo Local()
        {
            return Create("127.0.0.1");
        }

        public static SmtpConnectionInfo Create(string server)
        {
            return new SmtpConnectionInfo
            {
                Server = server
            };
        }

        public static SmtpConnectionInfo Create(string server, int port)
        {
            return new SmtpConnectionInfo
            {
                Server = server,
                Port = port
            };
        }

        public static SmtpConnectionInfo Create(string server, string userName, string password)
        {
            return new SmtpConnectionInfo
            {
                Server = server,
                UserName = userName,
                Password = password
            };
        }

        public static SmtpConnectionInfo Create(string server, int port, string userName, string password)
        {
            return new SmtpConnectionInfo
            {
                Server = server,
                Port = port,
                UserName = userName,
                Password = password
            };
        }

        public static SmtpConnectionInfo Create(string server, bool enableSSL, string userName, string password)
        {
            return new SmtpConnectionInfo
            {
                Server = server,
                EnableSSL = enableSSL,
                UserName = userName,
                Password = password
            };
        }

        public static SmtpConnectionInfo Create(string server, int port, bool enableSSL, string userName, string password)
        {
            return new SmtpConnectionInfo
            {
                Server = server,
                Port = port,
                EnableSSL = enableSSL,
                UserName = userName,
                Password = password
            };
        }

        public static implicit operator SmtpConnectionInfo(string str)
        {
            return Parse(str);
        }
    }
}
