//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Kooboo.Render.Response
{
    public class ResponseBase
    {
        public ResponseBase()
        {
        }

        public int StatusCode { get; set; } = 200;

        public string ContentType { get; set; }

        private HttpStringCollection _headers;

        public HttpStringCollection Headers
        {
            get { return _headers ?? (_headers = new HttpStringCollection()); }
            set
            {
                _headers = value;
            }
        }

        private List<string> _DeletedCookieNames;

        public List<string> DeletedCookieNames
        {
            get { return _DeletedCookieNames ?? (_DeletedCookieNames = new List<string>()); }
            set
            {
                _DeletedCookieNames = value;
            }
        }

        private List<Cookie> _appendcookies;

        public List<Cookie> AppendedCookies
        {
            get { return _appendcookies ?? (_appendcookies = new List<Cookie>()); }
            set
            {
                _appendcookies = new List<Cookie>();
            }
        }

        public void AppendCookie(string cookieName, string cookieValue, int days = 30)
        {
            var oldcookie = AppendedCookies.FirstOrDefault(o => o.Name == cookieName);

            if (oldcookie != null)
            {
                AppendedCookies.Remove(oldcookie);
            }

            AppendedCookies.Add(days == 0
                ? new Cookie() {Name = cookieName, Value = cookieValue, Expires = default(DateTime)}
                : new Cookie() {Name = cookieName, Value = cookieValue, Expires = DateTime.Now.AddDays(days)});
        }

        public void DeleteCookie(string cookieName)
        {
            if (!DeletedCookieNames.Contains(cookieName))
            {
                this.DeletedCookieNames.Add(cookieName);
            }
        }
    }
}