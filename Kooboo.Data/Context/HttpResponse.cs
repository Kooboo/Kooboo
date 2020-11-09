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
        public string ContentType
        {
            get
            {
                if (string.IsNullOrEmpty(_contenttype))
                {
                    return Kooboo.Constants.Site.DefaultContentType;
                }
                return _contenttype;
            }
            set
            { _contenttype = value; }
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

            get
            {
                return _body;
            }
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

        public Kooboo.IndexedDB.FilePart FilePart { get; set; }

        public long OrginalLength { get; set; }

        private Dictionary<string, string> _headers;
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

        public void AppendCookie(string CookieName, string CookieValue, int days = 1)
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

        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Set to end the rendering and directly return back to client... 
        /// </summary>
        public bool End { get; set; }

        public void Redirect(int StatusCode, string FullOrRelativeUrl)
        {
            if (string.IsNullOrWhiteSpace(FullOrRelativeUrl))
            {
                return;
            }

            this.StatusCode = StatusCode;
            string url = Lib.Helper.UrlHelper.GetEncodedLocation(FullOrRelativeUrl);

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
