//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Mail.Smtp
{
    public class SmtpServer : Kooboo.Tasks.IWorkerStarter
    {
        internal static Logging.ILogger _logger;

        static SmtpServer()
        {
            _logger = Logging.LogProvider.GetLogger("smtp", "socket");
        }

        private CancellationTokenSource _cancellationTokenSource;
        private TcpListener _listener;
        private Task _listenTask;

        public SmtpServer(string name)
            : this(name, 25)
        {
        }

        public SmtpServer(string name, int port)
        {
            Name = name;
            Port = port;
            Timeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
        }

        public string Name { get; set; }

        public bool SSL { get; set; }

        public int Port { get; set; }

        public int Timeout { get; set; }

        public bool AuthenticationRequired { get; set; }

        public SmtpServerOptions Options { get; set; } = new SmtpServerOptions();

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
                    var tcpClient = await _listener.AcceptTcpClientAsync();

                    var session = new SmtpConnector(this, tcpClient);
                    _ = session.Accept();
                }
                catch (Exception ex)
                {
                    Kooboo.Data.Log.Instance.Exception.Write(DateTime.Now.ToString() + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Source);
                    needawait = true;
                }

                if (needawait)
                {
                    await Task.Delay(100);
                }

            }
        }
    }

    public class SmtpServerOptions
    {

        public SmtpServerOptions()
        {

            this.LiveTimeout = TimeSpan.FromMinutes(1);
            this.MailsPerConnection = 100;

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