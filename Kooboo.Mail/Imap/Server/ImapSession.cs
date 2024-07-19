//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap
{
    public class ImapSession : IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;

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
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                if (Server.SslMode == SslMode.SSL)
                {
                    await StartSecureConnection();
                }
                else
                {
                    var stream = TcpClient.GetStream();
                    stream.ReadTimeout =
                    stream.WriteTimeout = Server.Timeout;
                    Stream = new ImapStream(TcpClient, stream);
                }

                var capabilities = String.Join(" ", Capabilities);

                await Stream.WriteLineAsync($"* OK Kooboo Imap server ready.");

                var cancellationToken = _cancellationTokenSource.Token;

                // List<string> history = new List<string>();
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var commandLine = await Stream.ReadCommandAsync();
                        if (commandLine != null)
                        {
                            await Kooboo.Mail.Imap.Commands.CommandManager.Execute(this, commandLine.Tag, commandLine.Name, commandLine.Args);
                            // TODO: Execute should return to cancel connection. 
                            if (commandLine.Name == "LOGOUT")
                            {
                                break;
                            }
                        }
                        else
                        {
                            _cancellationTokenSource?.Cancel();
                        }

                    }
                    catch (InvalidDataException)
                    {
                        await Stream.WriteLineAsync("BAD Error: Command not recognized.");
                        throw;
                    }

                    // history.Add(commandLine.Name + "  " + commandLine.Args);
                }
            }

            catch (SocketException)
            {
            }

            catch (IOException)
            {
                //client close connection.
            }
            catch (Exception)
            {
                //if (Stream != null)
                //{
                //    await Stream.WriteLineAsync("Local server error occured, bye!");
                //} 
            }
            finally
            {
                Dispose();
            }
        }

        public async Task StartSecureConnection()
        {
            var sslStream = new SslStream(TcpClient.GetStream(), false);

            var cert = Kooboo.Data.SSL.SslCertificateProvider.SelectCertificate2(Settings.ImapDomain);

            await sslStream.AuthenticateAsServerAsync(cert, false, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13, false);

            IsSecureConnection = true;

            sslStream.ReadTimeout =
            sslStream.WriteTimeout = Server.Timeout;
            Stream = new ImapStream(TcpClient, sslStream);
        }

        public void Dispose()
        {
            if (!_cancellationTokenSource.IsCancellationRequested) _cancellationTokenSource.Cancel();
            if (Stream != null)
            {
                Stream.Dispose();
            }

            TcpClient.Close();

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



