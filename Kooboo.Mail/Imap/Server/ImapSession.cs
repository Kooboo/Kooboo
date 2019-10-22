//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using LumiSoft.Net;
using LumiSoft.Net.AUTH;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap
{
    public class ImapSession
    {
        public ImapSession(ImapServer server, TcpClient client)
        {
            Server = server;
            TcpClient = client;
        }

        public ImapServer Server { get; private set; }

        public TcpClient TcpClient { get; private set; }

        public ImapStream Stream { get; set; }

        private List<string> _capabilities;

        public List<string> Capabilities
        {
            get { return _capabilities ?? (_capabilities = EnsureCapabilities()); }
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
            get { return _authentications ?? (_authentications = new Dictionary<string, AUTH_SASL_ServerMechanism>()); }
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
            set { _db = value; }
        }

        public async Task Start()
        {
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

                while (true)
                {
                    var commandLine = await Stream.ReadCommandAsync();
                    if (commandLine == null)
                    {
                        await Stream.WriteStatusAsync("BAD", "Error: Command not recognized.");
                    }
                    else
                    {
                        await Kooboo.Mail.Imap.Commands.CommandManager.Execute(this, commandLine.Tag, commandLine.Name, commandLine.Args);
                    }
                }
            }
            catch (SessionCloseException)
            {
                Close();
            }
            catch (Exception)
            {
                await Stream.WriteLineAsync("Local server error occured, bye!");

                if (TcpClient.Connected)
                {
                    TcpClient.Close();
                }
            }
        }

        public async Task StartSecureConnection()
        {
            var sslStream = new SslStream(TcpClient.GetStream(), true);
            await sslStream.AuthenticateAsServerAsync(Server.Certificate, false, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false);

            IsSecureConnection = true;

            Stream = new ImapStream(TcpClient, sslStream);
        }

        public void Close()
        {
            TcpClient.Close();
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
    }
}