//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Kooboo.Data.Context
{
    public class HttpRequest
    {
        public NameValueCollection Headers { get; set; } = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

        public NameValueCollection QueryString { get; set; } = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

        public NameValueCollection Forms { get; set; } = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

        public List<Lib.NETMultiplePart.File> Files { get; set; }


        private Dictionary<string, string> _cookies;
        public Dictionary<string, string> Cookies
        {
            get
            {
                if (_cookies == null)
                {
                    _cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return _cookies;
            }
            set
            {
                _cookies = value;
            }
        }

        public string Path { get; set; }

        public Uri Uri { get; set; }

        public string Url { get; set; }

        private string _relativeurl;
        public string RelativeUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_relativeurl))
                {
                    if (string.IsNullOrEmpty(this.RawRelativeUrl))
                    { return ""; }
                    else
                    {
                        return this.RawRelativeUrl;
                    }
                }
                else
                {
                    return _relativeurl;
                }
            }
            set
            {
                _relativeurl = value;
            }
        }

        private string _rawrelativeurl;

        public string RawRelativeUrl
        {
            get
            {
                if (_rawrelativeurl == null)
                {
                    return "/";
                }
                else { return _rawrelativeurl; }
            }
            set
            {
                _rawrelativeurl = value; 
            }
        }

        private string _method; 
        public string Method {
            get { return _method;  }
            set
            {
                _method = value; 
                if (_method !=null)
                {
                    _method = _method.ToUpper(); 
                }
            }
        }

        public string Host { get; set; }


        public string Scheme { get; set; }

        public byte[] PostData { get; set; }

        private string _body = null;
        public string Body
        {
            get
            {
                if (_body == null)
                {
                    if (PostData != null)
                    {
                        _body = System.Text.Encoding.UTF8.GetString(this.PostData);
                        if (string.IsNullOrEmpty(_body))
                        {
                            _body = string.Empty;
                        }
                        _body = System.Net.WebUtility.UrlDecode(_body);
                    }
                }
                return _body;
            }
            set { _body = value; }
        }

        public object Model { get; set; }

        public string IP { get; set; }

        public string GetValue(string name,bool needDecode=true)
        {
            return RequestManager.GetValue(this, name,needDecode);
        }

        public string Get(string name)
        {
            return this.GetValue(name); 
        }

        public string GetValue(params string[] names)
        {
            return RequestManager.GetValue(this, names);
        }

        public string DiskRoot { get; set; }

        private HashSet<int> _alternativeViews;
        public HashSet<int> AltervativeViews
        {
            get
            {
                if (_alternativeViews == null)
                {
                    _alternativeViews = new HashSet<int>();
                }
                return _alternativeViews;
            }
            set { _alternativeViews = value; }
        }

        public string Culture { get; set; }

        public string SitePath { get; set; }

        public RequestChannel Channel { get; set; }

        public int Port { get; set; }

        public HttpRequest Clone()
        {
            HttpRequest newrequest = new HttpRequest();
            newrequest.Body = this.Body;
            newrequest.Channel = this.Channel;
            foreach (var item in this.Cookies)
            {
                newrequest.Cookies.Add(item.Key, item.Value);
            }
            foreach (var item in this.QueryString.AllKeys)
            {
                newrequest.QueryString.Add(item, this.QueryString[item]);
            }

            foreach (var item in this.Forms.AllKeys)
            {
                newrequest.Forms.Add(item, this.Forms[item]);
            }

            foreach (var item in this.Headers.AllKeys)
            {
                newrequest.Headers.Add(item, this.Headers[item]);
            }

            newrequest.Url = this.Url;
            newrequest.RawRelativeUrl = this.RawRelativeUrl;
            newrequest.RelativeUrl = this.RelativeUrl;
            newrequest.Method = this.Method;
            newrequest.Model = this.Model;
            newrequest.IP = this.IP;
            newrequest.Host = this.Host;

            return newrequest;

        }
    }
}
