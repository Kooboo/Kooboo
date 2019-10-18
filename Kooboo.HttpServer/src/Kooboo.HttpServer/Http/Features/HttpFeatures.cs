using Kooboo.HttpServer.Http;

namespace Kooboo.HttpServer
{
    public class HttpFeatures
    {
        public ConnectionFeature Connection { get; set; } = new ConnectionFeature();

        public HttpRequestFeature Request { get; set; } = new HttpRequestFeature();

        public HttpResponseFeature Response { get; set; } = new HttpResponseFeature();
    }
}
