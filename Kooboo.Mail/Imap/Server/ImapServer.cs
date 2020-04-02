//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

using LumiSoft.Net;
using Newtonsoft.Json;

namespace Kooboo.Mail.Imap
{
    public class ImapServer : Kooboo.Tasks.IWorkerStarter
    {
        internal static Logging.ILogger _logger;
        private static long _nextConnectionId;
        static ImapServer()
        {
            _logger = Logging.LogProvider.GetLogger("imap", "socket");
        }

        private CancellationTokenSource _cancellationTokenSource;
        private TcpListener _listener;
        internal ConnectionManager _connectionManager;

        public ImapServer(int port)
        {
            Port = port;

            Heartbeat = Heartbeat.Instance;
            _connectionManager = new ConnectionManager(Options.MaxConnections);
            Heartbeat.Add(_connectionManager);
        }

        public ImapServer(int port, SslMode mode, X509Certificate certificate)
            : this(port)
        {
            SslMode = mode;
            Certificate = certificate;
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
                return EmailEnvironment.FQDN;
            }
        }

        [JsonIgnore]
        public X509Certificate Certificate { get; private set; }

        public SslMode SslMode { get; private set; }

        public ImapServerOptions Options { get; set; } = new ImapServerOptions();

        internal Heartbeat Heartbeat { get; }

        public async void Start()
        {
            if (Lib.Helper.NetworkHelper.IsPortInUse(Port))
                return;

            _listener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
            _listener.Start();

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cid = _nextConnectionId++;
                    _logger.LogInformation($"<ac {cid} {Thread.CurrentThread.ManagedThreadId}");
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    _logger.LogInformation($">ac {cid} {Thread.CurrentThread.ManagedThreadId} {tcpClient.Client.RemoteEndPoint}");

                    var session = new ImapSession(this, tcpClient, cid);
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
