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
        private CancellationTokenSource _cancellationTokenSource;
        private TcpListener _listener;
        internal ConcurrentDictionary<string, int> _connections;

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
            _connections = new ConcurrentDictionary<string, int>();
        }

        [JsonIgnore]
        public X509Certificate Certificate { get; private set; }

        public string Name { get; set; }

        public int Port { get; set; }

        public int Timeout { get; set; }

        public bool AuthenticationRequired { get; set; }

        public async void Start()
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

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync();

                    var session = new SmtpConnector(this, tcpClient);
                   session.Accept();
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
}
