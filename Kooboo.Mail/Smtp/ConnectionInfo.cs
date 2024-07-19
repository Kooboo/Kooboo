//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;

namespace Kooboo.Mail
{
    public abstract class ConnectionInfo : NullableDictionary<string, string>
    {
        protected abstract int DefaultPort { get; }

        public string Server
        {
            get
            {
                return this.GetValue<string>("server", "127.0.0.1");
            }
            set
            {
                this.SetValue<string>("server", value);
            }
        }

        public int Port
        {
            get
            {
                return this.GetValue<int>("port", GetPortByEnableSSL(this.GetValue<bool>("ssl", false)));
            }
            set
            {
                this.SetValue<int>("port", value);
            }
        }

        public bool EnableSSL
        {
            get
            {
                return this.GetValue<bool>("ssl", GetEnableSSLByPort(this.GetValue<int>("port", DefaultPort)));
            }
            set
            {
                this.SetValue<bool>("ssl", value);
            }
        }

        public string UserName
        {
            get
            {
                return this.GetValue<string>("user");
            }
            set
            {
                this.SetValue<string>("user", value);
            }
        }

        public string Password
        {
            get
            {
                return this.GetValue<string>("password");
            }
            set
            {
                this.SetValue<string>("password", value);
            }
        }

        public override string ToString()
        {
            return String.Join(";", this.Select(o => String.Format("{0}={1}", o.Key, o.Value)));
        }

        public void ParseSelf(string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return;

            if (str.Contains("="))
            {
                ParseKeyValueStyle(str);
            }
            else
            {
                ParseUriStyle(str);
            }
        }

        protected virtual int GetPortByEnableSSL(bool enableSSL)
        {
            return DefaultPort;
        }

        protected virtual bool GetEnableSSLByPort(int port)
        {
            return false;
        }

        private void ParseKeyValueStyle(string str)
        {
            foreach (var each in str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var pair = each.Trim().Split('=');
                this.Add(pair[0].Trim(), pair[1].Trim());
            }
        }

        private void ParseUriStyle(string str)
        {
            var splitted = str.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            Server = splitted[0];
            if (splitted.Length > 1)
            {
                int port;
                if (Int32.TryParse(splitted[1], out port))
                {
                    Port = port;
                }
            }
        }
    }
}
