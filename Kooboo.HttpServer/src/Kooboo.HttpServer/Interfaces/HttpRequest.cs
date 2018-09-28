using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using Kooboo.HttpServer.Http;
using Microsoft.AspNetCore.Http;
namespace Kooboo.HttpServer
{
    public class HttpRequest
    {
        private HttpFeatures _httpFeature;
        public HttpRequest(HttpFeatures feature)
        {
            _httpFeature = feature;
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

        public string[] AcceptTypes
        {
            get
            {
                return Headers.HeaderAccept;
            }
        }
       
        public int ClientCertificateError { get; }
        
        public Encoding ContentEncoding
        {
            get
            {
                return Encoding.GetEncoding(Headers.HeaderAcceptEncoding);
                
            }
        }
        
        public long? ContentLength64
        {
            get
            {
                return Headers.ContentLength;
            }
        }
        
        public string ContentType
        {
            get
            {
                return Headers.HeaderContentType;
            }
        }
        private CookieCollection _cookies;
        public CookieCollection Cookies
        {
            get
            {

                if (_cookies == null)
                {
                    _cookies = new CookieCollection();
                    foreach (var requestCookie in Request.Cookies)
                    {
                        Cookie current = new Cookie(requestCookie.Key, requestCookie.Value);
                        _cookies.Add(current);
                    }
                }
                return _cookies;
            }
        }
               
        
        public bool HasEntityBody
        {
            get
            {
                return InputStream.Length > 0;
            }
        }

        public HttpRequestHeaders Headers
        {
            get
            {
                return Request.Headers as HttpRequestHeaders;
            }
        }

        public string HttpMethod
        {
            get
            {
                return Request.Method;
            }
        }
        
        public Stream InputStream
        {
            get
            {
                return Request.Body;
            }
        }
        //todo confirm
        public bool IsAuthenticated { get; }
       
        public bool IsLocal
        {
            get
            {
                return IPAddress.IsLoopback(RemoteEndPoint.Address) || IPAddress.IsLoopback(LocalEndPoint.Address);
            }
        }
        
        public bool IsSecureConnection
        {
            get
            {
                return Request.Scheme == "https";
            }
        }
        
        public bool IsWebSocketRequest { get; }
       
        public bool KeepAlive
        {
            get
            {
                return bool.Parse(Headers.HeaderKeepAlive);
            }
        }
       
        public IPEndPoint LocalEndPoint
        {
            get
            {
                return Connection.LocalEndPoint;
            }
        }

        private Version _version;
        public Version ProtocolVersion
        {
            get
            {
                if(_version==null)
                {
                    _version = new Version(Request.Protocol);
                }
                return _version;
            }
        }

        private NameValueCollection _queryString;
        public NameValueCollection QueryString
        {
            get
            {
                if (_queryString == null)
                {
                    _queryString = new NameValueCollection();
                    foreach(var requestQueryString in Request.Query)
                    {
                        _queryString.Add(requestQueryString.Key, requestQueryString.Value);
                    }
                }
                return _queryString;
            }
        }
        
        public string RawUrl
        {
            get
            {
                return Request.Path;
            }
        }
        
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return Connection.RemoteEndPoint ;
            }
        }
        //todo confirm
        public string ServiceName { get; }
        
        public Uri Url
        {
            get
            {
                return new Uri(Request.Path);
            }
        }
        
        public Uri UrlReferrer
        {
            get
            {
                return new Uri(Headers.HeaderReferer);
            }
        }
       
        public string UserAgent
        {
            get
            {
                return Headers.HeaderUserAgent;
            }
        }
       
        public string UserHostAddress
        {
            get
            {
                return LocalEndPoint.ToString();
            }
        }

        public string UserHostName
        {
            get
            {
                return Headers.HeaderHost;
            }
        }
        public string[] UserLanguages
        {
            get
            {
                return Headers.HeaderContentLanguage;
            }
        }
    }
}
