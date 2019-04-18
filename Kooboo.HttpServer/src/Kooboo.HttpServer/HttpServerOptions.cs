using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.HttpServer
{
    public class HttpServerOptions
    {
        public IHttpHandler HttpHandler { get; set; } = new SampleHandler();
         

        public Func<string, X509Certificate> SelectCertificate { get; set; }
          
        public bool SslEnabled
        {
            get
            {
                return SelectCertificate != null;
            }
        }

        public bool IsHttps { get; set; }

        /// <summary>
        /// Configures the endpoints that Kestrel should listen to.
        /// </summary>
        /// <remarks>
        /// If this list is empty, the server.urls setting (e.g. UseUrls) is used.
        /// </remarks>
        //internal List<ListenOptions> ListenOptions { get; } = new List<ListenOptions>();

        /// <summary>
        /// Gets or sets whether the <c>Server</c> header should be included in each response.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public bool AddServerHeader { get; set; } = true;

        /// <summary>
        /// Gets or sets a value that determines how Kestrel should schedule user callbacks.
        /// </summary>
        /// <remarks>The default mode is <see cref="SchedulingMode.Default"/></remarks>
        //public SchedulingMode ApplicationSchedulingMode { get; set; } = SchedulingMode.Default;

        /// <summary>
        /// Gets or sets a value that controls whether synchronous IO is allowed for the  see  HttpContext.Request  and  see  "HttpContext.Response"  
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public bool AllowSynchronousIO { get; set; } = true;

        /// <summary>
        /// Enables the Listen options callback to resolve and use services registered by the application during startup.
        /// Typically initialized by UseKestrel()"/>.
        /// </summary>
        public IServiceProvider ApplicationServices { get; set; }

        /// <summary>
        /// Provides access to request limit options.
        /// </summary>
        public HttpServerLimit Limits { get; } = new HttpServerLimit()
        {
            MaxRequestBodySize= 100*1024*1024 //about 100M（ the default of IIS is 28.6M）
        };

        /// <summary>
        /// Provides a configuration source where endpoints will be loaded from on server start.
        /// The default is null.
        /// </summary>
        //public KestrelConfigurationLoader ConfigurationLoader { get; set; }

        ///// <summary>
        ///// A default configuration action for all endpoints. Use for Listen, configuration, the default url, and URLs.
        ///// </summary>
        //private Action<ListenOptions> EndpointDefaults { get; set; } = _ => { };

        ///// <summary>
        ///// A default configuration action for all https endpoints.
        ///// </summary>
        //private Action<HttpsConnectionAdapterOptions> HttpsDefaults { get; set; } = _ => { };

        /// <summary>
        /// The default server certificate for https endpoints. This is applied before HttpsDefaults.
        /// </summary>
        //internal X509Certificate2 DefaultCertificate { get; set; }

    }
}
