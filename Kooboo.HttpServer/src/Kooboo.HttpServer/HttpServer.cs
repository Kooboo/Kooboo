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
            //todo add http2 heatbeatManager
            heartbeat = new Heartbeat(
                 new IHeartbeatHandler[] { ServiceContext.DateHeaderValueManager },
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
#if NETSTANDARD2_0
            EnableRebinding(listenSocket);
#endif

            listenSocket.Bind(EndPoint);

            listenSocket.Listen(512);

            _listenSocket = listenSocket;
            _listenTask = Task.Run(() => Loop());
        }

#if NETSTANDARD2_0
        [DllImport("libc", SetLastError = true)]
        private static extern int setsockopt(int socket, int level, int option_name, IntPtr option_value, uint option_len);

        private const int SOL_SOCKET_OSX = 0xffff;
        private const int SO_REUSEADDR_OSX = 0x0004;
        private const int SOL_SOCKET_LINUX = 0x0001;
        private const int SO_REUSEADDR_LINUX = 0x0002;

        // Without setting SO_REUSEADDR on macOS and Linux, binding to a recently used endpoint can fail.
        // https://github.com/dotnet/corefx/issues/24562
        private unsafe void EnableRebinding(Socket listenSocket)
        {
            var optionValue = 1;
            var setsockoptStatus = 0;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                setsockoptStatus = setsockopt(listenSocket.Handle.ToInt32(), SOL_SOCKET_LINUX, SO_REUSEADDR_LINUX,
                                              (IntPtr)(&optionValue), sizeof(int));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                setsockoptStatus = setsockopt(listenSocket.Handle.ToInt32(), SOL_SOCKET_OSX, SO_REUSEADDR_OSX,
                                              (IntPtr)(&optionValue), sizeof(int));
            }

            if (setsockoptStatus != 0)
            {
                Console.WriteLine(string.Format("Setting SO_REUSEADDR failed with errno '{0}'.", Marshal.GetLastWin32Error()));
            }
        }
#endif
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
