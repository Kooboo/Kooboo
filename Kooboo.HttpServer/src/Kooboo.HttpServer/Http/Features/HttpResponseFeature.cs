using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Microsoft.AspNetCore.Http.Internal;

namespace Kooboo.HttpServer.Http
{
    public delegate Task ResponseWrite(ArraySegment<byte> data, CancellationToken cancellationToken = default(CancellationToken));
    public class HttpResponseFeature
    {
        private int _statusCode { get; set; }

        /// <summary>
        /// The status-code as defined in RFC 7230. The default value is 200.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return _statusCode;
            }
            set
            {
                if (HasStarted)
                {
                    throw new InvalidOperationException(CoreStrings.FormatParameterReadOnlyAfterResponseStarted(value));
                }

                _statusCode = value;
            }
        }

        private string _reasonPhrase;
        /// <summary>
        /// The reason-phrase as defined in RFC 7230. Note this field is no longer supported by HTTP/2.
        /// </summary>
        public string ReasonPhrase
        {
            get
            {
                return _reasonPhrase;
            }
            set
            {
                if (HasStarted)
                {
                    throw new InvalidOperationException(CoreStrings.FormatParameterReadOnlyAfterResponseStarted(value));
                }

                _reasonPhrase = value;
            }
        }

        /// <summary>
        /// The response headers to send. Headers with multiple values will be emitted as multiple headers.
        /// </summary>
        public IHeaderDictionary Headers { get; set; }

        /// <summary>
        /// The <see cref="Stream"/> for writing the response body.
        /// </summary>
        public Stream Body { get; set; }

        /// <summary>
        /// Indicates if the response has started. If true, the <see cref="StatusCode"/>,
        /// <see cref="ReasonPhrase"/>, and <see cref="Headers"/> are now immutable, and
        /// OnStarting should no longer be called.
        /// </summary>
        public bool HasStarted { get; }

        private ResponseCookies _cookiesCollection;
        public ResponseCookies Cookies
        {
            get
            {
                if (_cookiesCollection == null)
                {
                    var headers = Headers;
                    _cookiesCollection = new ResponseCookies(headers);
                }

                return _cookiesCollection;
            }
        }

    }
}
