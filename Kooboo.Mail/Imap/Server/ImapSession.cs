//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Principal;
using System.Security.Authentication;

using LumiSoft.Net.AUTH;
using LumiSoft.Net;

namespace Kooboo.Mail.Imap
{
    public class ImapSession : IManagedConnection, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;
        private long _timeoutTimestamp = Int64.MaxValue;    // To check connection alive timeout
        private int _disposing;

        public ImapSession(ImapServer server, TcpClient client, long connectionId)
        {
            Server = server;
            TcpClient = client;
            Id = connectionId;
        }

        public long Id { get; set; }

        public ImapServer Server { get; private set; }

        public TcpClient TcpClient { get; private set; }

        public ImapStream Stream { get; set; }

        private List<string> _capabilities;
        public List<string> Capabilities
        {
            get
            {
                if (_capabilities == null)
                {
                    _capabilities = EnsureCapabilities();
                }
                return _capabilities;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return AuthenticatedUserIdentity != null;
            }
        }

        public GenericIdentity AuthenticatedUserIdentity { get; set; }

        public bool IsSecureConnection { get; set; }

        private Dictionary<string, AUTH_SASL_ServerMechanism> _authentications;
        public Dictionary<string, AUTH_SASL_ServerMechanism> Authentications
        {
            get
            {
                if (_authentications == null)
                {
                    _authentications = new Dictionary<string, AUTH_SASL_ServerMechanism>();
                }
                return _authentications;
            }
            set
            {
                _authentications = value;
            }
        }

        public SelectFolder SelectFolder { get; set; }

        private MailDb _db;
        public MailDb MailDb
        {
            get
            {
                if (_db == null)
                {
                    if (this.IsAuthenticated)
                    {
                        var user = Kooboo.Data.GlobalDb.Users.Get(this.AuthenticatedUserIdentity.Name);
                        if (user != null)
                        {
                            _db = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);
                        }
                    }
                }
                return _db;
            }
            set { _db = value;  }

        }

        public async Task Start()
        {
            Server._connectionManager.AddConnection(this);
            _cancellationTokenSource = new CancellationTokenSource();
            Interlocked.Exchange(ref _timeoutTimestamp, DateTime.UtcNow.Add(Server.Options.LiveTimeout).Ticks);

            try
            {
                if (Server.SslMode == SslMode.SSL)
                {
                    await StartSecureConnection();
                }
                else
                {
                    Stream = new ImapStream(TcpClient, TcpClient.GetStream());
                }

                await OnStart();

                var cancellationToken = _cancellationTokenSource.Token;
                while (!cancellationToken.IsCancellationRequested)
                {
                    var commandLine = await Stream.ReadCommandAsync();
                    if (commandLine == null)
                    {
                        await Stream.WriteStatusAsync("BAD", "Error: Command not recognized."); 
                    }
                    else
                    {
                        await Kooboo.Mail.Imap.Commands.CommandManager.Execute(this, commandLine.Tag, commandLine.Name, commandLine.Args);
                        OnCommandExecuted();
                    }
                }
            }
            catch (SessionCloseException)
            {
                // No need to dispose for finally block will do it
            }
            catch (Exception ex)
            {
                await Stream.WriteLineAsync("Local server error occured, bye!");
            }
            finally
            {
                Server._connectionManager.RemoveConnection(Id);
                Dispose();
            }
        }

        public async Task StartSecureConnection()
        {
            var sslStream = new SslStream(TcpClient.GetStream(), false);
            await sslStream.AuthenticateAsServerAsync(Server.Certificate, false, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false);

            IsSecureConnection = true;

            Stream = new ImapStream(TcpClient, sslStream);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposing, 1) == 0)
            {
                _cancellationTokenSource.Cancel();
                Stream.Dispose();
                TcpClient.Close();
            }
        }

        public void CheckTimeout()
        {
            var timestamp = Server.Heartbeat.UtcNow.Ticks;
            if (timestamp > Interlocked.Read(ref _timeoutTimestamp))
            {
                _timeoutTimestamp = Int64.MaxValue;
                Dispose();
            }
        }

        protected virtual Task OnStart()
        {
            var capabilities = String.Join(" ", Capabilities);
            var hostName = Net_Utils.GetLocalHostName(Server.HostName);

            return Stream.WriteLineAsync($"* OK <{hostName}> Kooboo Imap server ready.");
        }

        private List<string> EnsureCapabilities()
        {
            /* RFC 3501 6.1.1. CAPABILITY Command.  
                Example:  C: abcd CAPABILITY
                S: * CAPABILITY IMAP4rev1 STARTTLS AUTH=GSSAPI LOGINDISABLED
                S: abcd OK CAPABILITY completed
                C: efgh STARTTLS
                S: efgh OK STARTLS completed
                <TLS negotiation, further commands are under [TLS] layer>
                C: ijkl CAPABILITY
                S: * CAPABILITY IMAP4rev1 AUTH=GSSAPI AUTH=PLAIN
                S: ijkl OK CAPABILITY completed
            */

            var result = new List<string>
            {
                "IMAP4",
                "IMAP4rev1",
                "MOVE",
                "CHILDREN"
                //"UTF8=ACCEPT",
                //"SASL-IR"
            };

            if (!IsSecureConnection && Server.SslMode == SslMode.StartTLS)
            {
                result.Add("STARTTLS");
            }

            //foreach (AUTH_SASL_ServerMechanism auth in Authentications.Values)
            //{
            //    result.Add("AUTH=" + auth.Name);
            //}

            return result;
        }

        private void OnCommandExecuted()
        {
            // Todo: Could add max connection period limit with max connections limit later

            Interlocked.Exchange(ref _timeoutTimestamp, DateTime.UtcNow.Add(Server.Options.LiveTimeout).Ticks);
        }
    }
}



