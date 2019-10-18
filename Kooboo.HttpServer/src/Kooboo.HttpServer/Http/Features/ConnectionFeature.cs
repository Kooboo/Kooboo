using System.Net;

namespace Kooboo.HttpServer
{
    public class ConnectionFeature
    {
        public string ConnectionId { get; set; }

        public long HttpConnectionId { get; set; }

        public IPEndPoint LocalEndPoint { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }
    }
}
