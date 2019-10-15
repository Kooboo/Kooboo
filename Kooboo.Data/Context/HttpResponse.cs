//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Net;

namespace Kooboo.Data.Context
{
    public class HttpResponse
    {
        public HttpResponse()
        {
            StatusCode = 200;
        }

        private string _contenttype;

        public string ContentType
        {
            get => string.IsNullOrEmpty(_contenttype) ? Kooboo.Constants.Site.DefaultContentType : _contenttype;
            set => _contenttype = value;
        }

        public void AppendString(string output)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(output);
            Body = bytes;
        }

        private byte[] _body;

        // this is actually only for text body... for binary... file..this will break..
        public byte[] Body
        {
            get => _body;
            set
            {
                if (_body == null)
                {
                    _body = value;
                }
                else
                {
                    // append directly.
                    int newlen = value.Length;
                    int oldlen = _body.Length;
                    byte[] newvalue = new byte[newlen + oldlen];

                    System.Buffer.BlockCopy(_body, 0, newvalue, 0, _body.Length);
                    System.Buffer.BlockCopy(value, 0, newvalue, _body.Length, value.Length);
                    _body = newvalue;
                }
            }
        }

        public System.IO.Stream Stream { get; set; }

        private Dictionary<string, string> _headers;

        public Dictionary<string, string> Headers
        {
            get => _headers ?? (_headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            set => _headers = value;
        }

        private List<string> _deletedCookieNames;

        public List<string> DeletedCookieNames
        {
            get => _deletedCookieNames ?? (_deletedCookieNames = new List<string>());
            set => _deletedCookieNames = value;
        }

        private List<Cookie> _appendcookies;

        public List<Cookie> AppendedCookies
        {
            get => _appendcookies ?? (_appendcookies = new List<Cookie>());
            set => _appendcookies = new List<Cookie>();
        }

        public void AppendCookie(string cookieName, string cookieValue, int days = 1)
        {
            AppendCookie(cookieName, cookieValue, DateTime.Now.AddDays(days));
        }

        public void AppendCookie(string cookieName, string cookieValue, DateTime expires)
        {
            AddCookie(new Cookie { Name = cookieName, Value = cookieValue, Expires = expires });
        }

        public void AddCookie(Cookie cookie)
        {
            AppendedCookies.RemoveAll(o => o.Name == cookie.Name);
            AppendedCookies.Add(cookie);
        }

        public void DeleteCookie(string cookieName)
        {
            if (!DeletedCookieNames.Contains(cookieName))
            {
                DeletedCookieNames.Add(cookieName);
            }
        }

        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Set to end the rendering and directly return back to client...
        /// </summary>
        public bool End { get; set; }

        public void Redirect(int statusCode, string fullOrRelativeUrl)
        {
            if (string.IsNullOrWhiteSpace(fullOrRelativeUrl))
            {
                return;
            }

            this.StatusCode = statusCode;
            string url = Lib.Helper.UrlHelper.GetEncodedLocation(fullOrRelativeUrl);

            this.Headers["location"] = url;
        }

        public string RedirectLocation
        {
            get
            {
                if (this.Headers.ContainsKey("location"))
                {
                    return this.Headers["location"];
                }
                return null;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.Headers["location"] = "";
                }
                else
                {
                    var url = Lib.Helper.UrlHelper.GetEncodedLocation(value);
                    this.Headers["location"] = url;
                }
            }
        }
    }
}