using System.Buffers;
using System.IO.Pipelines;
using System.Net;

namespace Kooboo.HttpServer.Http
{
    public class HttpConnectionContext
    {
        public ServiceContext ServiceContext { get; set; }

        public Pipe ReceivePipe { get; set; }

        public Pipe SendPipe { get; set; }

        public HttpFeatures Features { get; set; } = new HttpFeatures();

        public MemoryPool MemoryPool
        {
            get
            {
                return ServiceContext.MemoryPool;
            }
        }

        public string ConnectionId
        {
            get
            {
                return Features.Connection.ConnectionId;
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return Features.Connection.RemoteEndPoint;
            }
        }

        public IPEndPoint LocalEndPoint
        {
            get
            {
                return Features.Connection.LocalEndPoint;
            }
        }
    }
}