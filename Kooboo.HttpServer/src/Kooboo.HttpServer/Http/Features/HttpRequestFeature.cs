using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;

namespace Kooboo.HttpServer.Http
{
    public class HttpRequestFeature
    {
        /// <summary>
        /// The HTTP-version as defined in RFC 7230. E.g. "HTTP/1.1"
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// The request uri scheme. E.g. "http" or "https". Note this value is not included
        /// in the original request, it is inferred by checking if the transport used a TLS
        /// connection or not.
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// The request method as defined in RFC 7230. E.g. "GET", "HEAD", "POST", etc..
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The first portion of the request path associated with application root. The value
        /// is un-escaped. The value may be string.Empty.
        /// </summary>
        public string PathBase { get; set; }

        /// <summary>
        /// The portion of the request path that identifies the requested resource. The value
        /// is un-escaped. The value may be string.Empty if <see cref="PathBase"/> contains the
        /// full path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The query portion of the request-target as defined in RFC 7230. The value
        /// may be string.Empty. If not empty then the leading '?' will be included. The value
        /// is in its original form, without un-escaping.
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// The request target as it was sent in the HTTP request. This property contains the
        /// raw path and full query, as well as other request targets such as * for OPTIONS
        /// requests (https://tools.ietf.org/html/rfc7230#section-5.3).
        /// </summary>
        /// <remarks>
        /// This property is not used internally for routing or authorization decisions. It has not
        /// been UrlDecoded and care should be taken in its use.
        /// </remarks>
        public string RawTarget { get; set; }

        /// <summary>
        /// Headers included in the request, aggregated by header name. The values are not split
        /// or merged across header lines. E.g. The following headers:
        /// HeaderA: value1, value2
        /// HeaderA: value3
        /// Result in Headers["HeaderA"] = { "value1, value2", "value3" }
        /// </summary>
        public IHeaderDictionary Headers { get; set; }

        /// <summary>
        /// A <see cref="Stream"/> representing the request body, if any. Stream.Null may be used
        /// to represent an empty request body.
        /// </summary>
        public Stream Body { get; set; }

        private string _originalQueryString;
        private QueryCollection _query;

        public QueryCollection Query
        {
            get
            {
                var current = QueryString;
                if (_query == null || !string.Equals(_originalQueryString, current, StringComparison.Ordinal))
                {
                    _originalQueryString = current;

                    var result = QueryHelpers.ParseNullableQuery(current);

                    if (result == null)
                    {
                        _query = QueryCollection.Empty;
                    }
                    else
                    {
                        _query = new QueryCollection(result);
                    }
                }
                return _query;
            }
            set
            {
                _query = value;
                if (value == null)
                {
                    _originalQueryString = string.Empty;
                    QueryString = string.Empty;
                }
                else
                {
                    _originalQueryString = Microsoft.AspNetCore.Http.QueryString.Create(_query).ToString();
                    QueryString = _originalQueryString;
                }
            }
        }

        private StringValues _originalCookie;
        private RequestCookieCollection _cookies;
        public RequestCookieCollection Cookies
        {
            get
            {
                var headers = Headers;
                StringValues current;
                if (!headers.TryGetValue(HeaderNames.Cookie, out current))
                {
                    current = string.Empty;
                }

                if (_cookies == null || _originalCookie != current)
                {
                    _originalCookie = current;
                    _cookies = RequestCookieCollection.Parse(current.ToArray());
                }

                return _cookies;
            }
            set
            {
                _cookies = value;
                _originalCookie = StringValues.Empty;
                if (_cookies == null || _cookies.Count == 0)
                {
                    Headers.Remove(HeaderNames.Cookie);
                }
                else
                {
                    var headers = new List<string>();
                    foreach (var pair in _cookies)
                    {
                        headers.Add(new CookieHeaderValue(pair.Key, pair.Value).ToString());
                    }
                    _originalCookie = headers.ToArray();
                    Headers[HeaderNames.Cookie] = _originalCookie;
                }
            }
        }
    }
}
