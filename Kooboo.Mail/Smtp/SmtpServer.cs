//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace Kooboo.Mail.Smtp
{
    public class SmtpServer : Kooboo.Tasks.IWorkerStarter
    {
        internal static Logging.ILogger _logger;
        private static long _nextConnectionId;

        static SmtpServer()
        {
            _logger = Logging.LogProvider.GetLogger("smtp", "socket");
        }

        private CancellationTokenSource _cancellationTokenSource;
        private TcpListener _listener;
        private Task _listenTask;
        internal ConnectionManager _connectionManager;

        public SmtpServer(string name)
            : this(name, 25)
        {
        }

        public SmtpServer(string name, int port)
            : this(name, port, null)
        {
        }

        public SmtpServer(string name, int port, X509Certificate cert)
        {
            Name = name;
            Port = port;
            Certificate = cert;

            Heartbeat = Heartbeat.Instance;
            _connectionManager = new ConnectionManager(Options.MaxConnections);
            Heartbeat.Add(_connectionManager);
        }

        [JsonIgnore]
        public X509Certificate Certificate { get; private set; }

        public string Name { get; set; }

        public int Port { get; set; }

        public int Timeout { get; set; }

        public bool AuthenticationRequired { get; set; }

        public SmtpServerOptions Options { get; set; } = new SmtpServerOptions();

        internal Heartbeat Heartbeat { get; }

        public void Start()
        {
            // 第一层端口占用保护
            if (Lib.Helper.NetworkHelper.IsPortInUse(Port))
                return;

            try
            {
                _listener = new TcpListener(new IPEndPoint(IPAddress.Any, Port));

                if (Timeout != 0)
                {
                    _listener.Server.ReceiveTimeout =
                    _listener.Server.SendTimeout = Timeout;
                }

                _listener.Start();
            }
            catch (SocketException ex)
            {
                // 第二层端口占用保护
                if (ex.ErrorCode == 10048)
                {
                    _listener = null;
                    return;
                }
                else
                {
                    throw ex;
                }
            }

            _listenTask = Task.Run(() => Loop());
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

        private async Task Loop()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                var needawait = false;
                try
                {
                    var cid = _nextConnectionId++;
                    _logger.LogInformation($"<ac {cid} {Thread.CurrentThread.ManagedThreadId}");
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    _logger.LogInformation($">ac {cid} {Thread.CurrentThread.ManagedThreadId} {tcpClient.Client.RemoteEndPoint}");

                    var session = new SmtpConnector(this, tcpClient, cid);
                    _ = session.Accept();
                }
                catch(Exception ex)
                {
                    Kooboo.Data.Log.Instance.Exception.Write(DateTime.Now.ToString()+ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Source);
                    needawait = true;
                }
                if (needawait)
                {
                    await Task.Delay(200);
                }
                
            }
        }
    }

    public class SmtpServerOptions
    {

        public SmtpServerOptions()
        {

            this.LiveTimeout = TimeSpan.FromSeconds(30);
            this.MailsPerConnection = 10;

#if DEBUG
            {
                this.LiveTimeout = TimeSpan.FromSeconds(30000);
                this.MailsPerConnection = 1000;
            }
#endif

        }

        public TimeSpan LiveTimeout { get; set; }

        public int MailsPerConnection { get; set; }

        public int? MaxConnections { get; set; }
    }
} 