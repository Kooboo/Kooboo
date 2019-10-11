//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Kooboo.Api.ApiResponse
{
    public class MetaResponse : IResponse
    {
        public MetaResponse()
        {
            FieldErrors = new List<FieldError>();
            Messages = new List<string>();
        }

        public bool DataChange
        {
            get; set;
        }

        public List<FieldError> FieldErrors
        {
            get; set;
        }

        public List<string> Messages
        {
            get; set;
        }

        public object Model
        {
            get; set;
        }

        public bool Success
        {
            get; set;
        }

        private HttpStringCollection _headers;

        public HttpStringCollection Headers
        {
            get => _headers ?? (_headers = new HttpStringCollection());
            set => _headers = value;
        }

        private List<string> _deletedCookieNames;

        public List<string> DeletedCookieNames
        {
            get => _deletedCookieNames ?? (_deletedCookieNames = new List<string>());
            set => _deletedCookieNames = value;
        }

        private List<Cookie> _appendCookies;

        public List<Cookie> AppendedCookies
        {
            get => _appendCookies ?? (_appendCookies = new List<Cookie>());
            set => _appendCookies = new List<Cookie>();
        }

        public void AppendCookie(string cookieName, string cookieValue, int days = 30)
        {
            var cookie = AppendedCookies.FirstOrDefault(o => o.Name == cookieName);

            if (cookie != null)
            {
                AppendedCookies.Remove(cookie);
            }

            AppendedCookies.Add(days == 0
                ? new Cookie {Name = cookieName, Value = cookieValue, Expires = default}
                : new Cookie {Name = cookieName, Value = cookieValue, Expires = DateTime.Now.AddDays(days)});
        }

        public void DeleteCookie(string cookieName)
        {
            if (!DeletedCookieNames.Contains(cookieName))
            {
                DeletedCookieNames.Add(cookieName);
            }
        }

        //public class CookieOptions
        //{
        //    public string Domain;

        //    private DateTime _expires;
        //    public DateTime Expires
        //    {
        //        get
        //        {
        //            if (_expires == default(DateTime))
        //            {
        //                _expires = DateTime.Now.AddHours(24);
        //            }
        //            return _expires;
        //        }
        //        set
        //        {
        //            _expires = value;
        //        }
        //    }
        //}

        //public class Cookie
        //{
        //    public Cookie()
        //    {
        //        this.Options = new CookieOptions();
        //    }

        //    public string Name { get; set; }
        //    public string Value { get; set; }

        //    public CookieOptions Options { get; set; }
        //}

        public string RedirectUrl { get; set; }

        public int StatusCode { get; set; } = 200;

        public void Redirect(string url, int statusCode = 302)
        {
            RedirectUrl = url;
            StatusCode = statusCode;
        }
    }
}