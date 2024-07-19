//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Kooboo.Data.Context;

namespace Kooboo.Api.ApiResponse
{
    public class MetaResponse : JsonResponse
    {
        public MetaResponse()
        {
            this.FieldErrors = new List<FieldError>();
            this.Messages = new List<string>();
        }

        private HttpStringCollection _headers;
        public HttpStringCollection Headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new HttpStringCollection();
                }
                return _headers;
            }
            set
            {
                _headers = value;
            }
        }

        private List<string> _DeletedCookieNames;
        public List<string> DeletedCookieNames
        {
            get
            {
                if (_DeletedCookieNames == null)
                {
                    _DeletedCookieNames = new List<string>();
                }
                return _DeletedCookieNames;
            }
            set
            {
                _DeletedCookieNames = value;
            }
        }

        private List<Cookie> _appendCookies;
        public List<Cookie> AppendedCookies
        {
            get
            {
                if (_appendCookies == null)
                {
                    _appendCookies = new List<Cookie>();
                }
                return _appendCookies;
            }
            set
            {
                _appendCookies = new List<Cookie>();
            }
        }

        public void AppendCookie(string CookieName, string CookieValue, int days = 30)
        {
            var oldCookie = AppendedCookies.Where(o => o.Name == CookieName).FirstOrDefault();

            if (oldCookie != null)
            {
                AppendedCookies.Remove(oldCookie);
            }

            if (days == 0)
            {
                AppendedCookies.Add(new Cookie() { Name = CookieName, Value = CookieValue, Expires = default(DateTime) });
            }
            else
            {
                AppendedCookies.Add(new Cookie() { Name = CookieName, Value = CookieValue, Expires = DateTime.Now.AddDays(days) });
            }

        }

        public void DeleteCookie(string CookieName)
        {
            if (!DeletedCookieNames.Contains(CookieName))
            {
                this.DeletedCookieNames.Add(CookieName);
            }
        }

        public string RedirectUrl { get; set; }

        public int StatusCode { get; set; } = 200;

        public void Redirect(string url, int statusCode = 302)
        {
            this.RedirectUrl = url;
            this.StatusCode = statusCode;
        }
    }
}
