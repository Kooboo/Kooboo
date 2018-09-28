using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Kooboo.HttpServer.Http;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Kooboo.HttpServer.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Kooboo.HttpServer
{
    public class HttpResponse
    {
        private HttpFeatures _httpFeature;
        public HttpResponse(HttpFeatures feature)
        {
            _httpFeature = feature;
        }
        private HttpResponseFeature Response
        {
            get
            {
                return _httpFeature.Response;
            }
        }
        private HttpRequestFeature Request
        {
            get
            {
                return _httpFeature.Request;
            }
        }

        private ConnectionFeature Connection
        {
            get
            {
                return _httpFeature.Connection;
            }
        }

        public Encoding ContentEncoding { get; set; }
        
        public long? ContentLength64
        {
            get
            {
                return Headers.ContentLength;
            }
            set
            {
                Headers.ContentLength = value;
            }
        }
       
        public string ContentType
        {
            get
            {
                return Headers.HeaderContentType;
            }
            set
            {
                Headers.HeaderContentType = value;
            }
        }
        public ResponseCookies Cookies
        {
            get
            {
                return Response.Cookies;
            }
        }
        public HttpResponseHeaders Headers
        {
            get
            {
                return Response.Headers as HttpResponseHeaders;
            }
            set
            {
                Response.Headers = value;
            }
        } 
        public bool KeepAlive
        {
            get
            {
                return bool.Parse(Headers.HeaderKeepAlive);
            }
            set
            {
                Headers.HeaderKeepAlive = value.ToString();
            }
        }
       
        public Stream OutputStream
        {
            get
            {
                return Response.Body;
            }
        }
       
        public Version ProtocolVersion
        {
            get
            {
                return new Version(Request.Protocol);
            }
        }
        
        public string RedirectLocation
        {
            get
            {
                return Headers.HeaderLocation;
            }
            set
            {
                Headers.HeaderLocation = value;
            }
        }
        
        public bool SendChunked { get; set; }
        
        public int StatusCode
        {
            get
            {
                return Response.StatusCode;
            }
            set
            {
                Response.StatusCode = value;
            }
        }
        
        public string StatusDescription
        {
            get
            {
                return Response.ReasonPhrase;
            }
            set
            {
                Response.ReasonPhrase = value;
            }
        }
        
        public void AddHeader(string name, string value)
        {
            AppendHeader(name, value);
        }
        
        public void AppendCookie(Cookie cookie)
        {
            var options = new CookieOptions()
            {
                Domain = cookie.Domain,
                Secure = cookie.Secure,
                HttpOnly = cookie.HttpOnly,
            };
            if (!string.IsNullOrEmpty(cookie.Path))
                options.Path = cookie.Path;
            if (cookie.Expires != DateTime.MinValue)
            {
                options.Expires = cookie.Expires;
            }

            Cookies.Append(cookie.Name, cookie.Value, options);
        }
        
        public void AppendHeader(string name, string value)
        {
            if (Headers == null)
                Headers = new HttpResponseHeaders();
            var headersDic = Headers as IHeaderDictionary;
            headersDic[name] = value;
        }
       
        public void Redirect(string url)
        {
            RedirectLocation = url;
            Response.StatusCode = 301;
        }
    }
}
