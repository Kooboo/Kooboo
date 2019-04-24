using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using Kooboo.HttpServer.Sockets;
using Kooboo.HttpServer.Http;

namespace Kooboo.HttpServer
{
    public class HttpServer
    {
        private ISocketsTrace _trace = new DefaultSocketsTrace();
        private Heartbeat heartbeat;
        private Socket _listenSocket;
        private Task _listenTask;

        public HttpServer(IPEndPoint endPoint)
            : this(endPoint, null)
        {

        }

        public HttpServer(IPEndPoint endPoint, HttpServerOptions options)
        {
            EndPoint = endPoint;

            var logger = new DefaultLogger();
            var systemClock = new SystemClock();
            options = options ?? new HttpServerOptions();

            ServiceContext = new ServiceContext
            {
                ServerOptions = options,
                MemoryPool = new MemoryPool(),
                Log = logger, 
                SystemClock = systemClock,
                ConnectionManager = new HttpConnectionManager(logger, upgradedConnectionLimit: null),
                DateHeaderValueManager = new DateHeaderValueManager(systemClock),
                HttpParser = new HttpParser<Http1ParsingHandler>(),
                ThreadPool = GetThreadPool(logger),
                Application = new HttpApplication(options.HttpHandler)
            };
            var httpHeartbeatManager = new HttpHeartbeatManager(ServiceContext.ConnectionManager);
            //todo add http2 heatbeatManager
            heartbeat = new Heartbeat(
                 new IHeartbeatHandler[] { httpHeartbeatManager },
                ServiceContext.SystemClock,
                logger);
        }

        private KoobooThreadPool GetThreadPool(ILogger logger)
        {
            //CZ: walkaround
            return new LoggingThreadPool(logger);
        }

        public ServiceContext ServiceContext { get; set; }

        public IPEndPoint EndPoint { get; private set; }

        public void Start()
        {
            var listenSocket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listenSocket.Bind(EndPoint);

            listenSocket.Listen(512);

            _listenSocket = listenSocket;
            _listenTask = Task.Run(() => Loop());

            heartbeat.Start();
        }

        private async Task Loop()
        {
            while (true)
            {
                try
                {
                    var acceptSocket = await _listenSocket.AcceptAsync();

                    var connection = new SocketConnection(acceptSocket, ServiceContext);

                    _ = connection.StartAsync();
                }
                catch
                {
                }
            }
        }
    }
}
