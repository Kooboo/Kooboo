//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Context
{ 
    public class HttpResponse
    {
        public HttpResponse()
        {
            this.StatusCode = 200;
        }
  
        private string _contenttype; 
        public string ContentType {
            get
            {
               if (string.IsNullOrEmpty(_contenttype))
                {
                    return Kooboo.Constants.Site.DefaultContentType; 
                }
                return _contenttype; 
            }
            set
            { _contenttype = value;  }
        }

        public byte[] Body { get; set; }

        public System.IO.Stream Stream { get; set; }

        private Dictionary<string,string> _headers;
        public Dictionary<string, string> Headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); 
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

        private List<Cookie> _appendcookies;
        public List<Cookie> AppendedCookies
        {
            get
            {
                if (_appendcookies == null)
                {
                    _appendcookies = new List<Cookie>();
                }
                return _appendcookies;
            }
            set
            {
                _appendcookies = new List<Cookie>();
            }
        }

        public void AppendCookie(string CookieName, string CookieValue, int days=1)
        {           
            AppendCookie(CookieName, CookieValue, DateTime.Now.AddDays(days));   
        }

        public void AppendCookie(string CookieName, string CookieValue, DateTime expires)
        {              
            AddCookie(new Cookie() { Name = CookieName, Value = CookieValue, Expires = expires });
        }


        public void AddCookie(Cookie cookie)
        {
            AppendedCookies.RemoveAll(o => o.Name == cookie.Name);  
            AppendedCookies.Add(cookie); 
        }

        public void DeleteCookie(string CookieName)
        {
            if (!DeletedCookieNames.Contains(CookieName))
            {
                this.DeletedCookieNames.Add(CookieName);
            }
        }

        public int StatusCode { get; set; } =200; 

        /// <summary>
        /// Set to end the rendering and directly return back to client... 
        /// </summary>
        public bool End { get; set; }

        public void Redirect(int StatusCode, string FullOrRelativeUrl)
        {
            this.StatusCode = StatusCode;
            this.Headers["location"]=FullOrRelativeUrl;
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
                this.Headers["location"] = value; 
            }
        }
         
    }
    

}
