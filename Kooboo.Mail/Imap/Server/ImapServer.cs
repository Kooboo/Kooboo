//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Kooboo.Mail.Imap
{
    public class ImapServer : Kooboo.Tasks.IWorkerStarter
    {
        internal static Logging.ILogger _logger;

        static ImapServer()
        {
            _logger = Logging.LogProvider.GetLogger("imap", "socket");
        }

        private CancellationTokenSource _cancellationTokenSource;
        private TcpListener _listener;

        public ImapServer(int port)
        {
            Port = port;

            Timeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
        }

        public ImapServer(int port, SslMode mode)
            : this(port)
        {
            SslMode = mode;
        }

        public string Name
        {
            get
            {
                return "Imap";
            }
        }

        public int Port { get; set; }

        public string HostName
        {
            get
            {
                return "Kooboo IMAP Server";
            }
        }

        public int Timeout { get; set; }

        public SslMode SslMode { get; private set; }

        public ImapServerOptions Options { get; set; } = new ImapServerOptions();


        public async void Start()
        {
            if (Lib.Helper.NetworkHelper.IsPortInUse(Port))
                return;

            _listener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
            _listener.Server.ReceiveTimeout =
            _listener.Server.SendTimeout = Timeout;
            _listener.Start();

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // _logger.LogInformation($"<ac {cid} {Thread.CurrentThread.ManagedThreadId}"); 
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    var session = new ImapSession(this, tcpClient);
                    _ = session.Start();
                }
                catch
                {
                }
            }
        }

        public void Stop()
        {
            if (_listener == null)
                return;

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }

            if (_listener != null)
            {
                _listener.Stop();
            }
        }
    }

    public class ImapServerOptions
    {
        public ImapServerOptions()
        {
#if DEBUG
            this.LiveTimeout = TimeSpan.FromHours(1);
#else
            this.LiveTimeout = TimeSpan.FromMinutes(1);
#endif
        }

        public TimeSpan LiveTimeout { get; set; }

        public int? MaxConnections { get; set; }
    }
}
