//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Threading;
using Kooboo.Mail.Utility;

namespace Kooboo.Mail.Smtp
{
    public class SmtpConnector : IManagedConnection, IDisposable
    {
        private TcpClient _client;
        private Stream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private SmtpServer _server;
        private bool _disposed;

        private CancellationTokenSource _cancellationTokenSource;
        private long _timeoutTimestamp = Int64.MaxValue;    // To check connection alive timeout
        private int _receivedMails; // Record how many DATA recevied, to control max received mails per connection
        private int _disposing;

        public SmtpConnector(SmtpServer server, TcpClient client, long connectionId)
        {
            _server = server;
            _client = client;
            Id = connectionId;
            Local = CopyIPEndPoint(_client.Client.LocalEndPoint as IPEndPoint);
            Client = CopyIPEndPoint(_client.Client.RemoteEndPoint as IPEndPoint);
        }

        public long Id { get; set; }

        public IPEndPoint Local { get; set; }

        public IPEndPoint Client { get; set; }

        public async Task Accept()
        {
            // Add cancellation token to allow cancel from any point calling Dispose()
            _server._connectionManager.AddConnection(this);
            _cancellationTokenSource = new CancellationTokenSource();
            Interlocked.Exchange(ref _timeoutTimestamp, DateTime.UtcNow.Add(_server.Options.LiveTimeout).Ticks);

            try
            {
                _stream = _client.GetStream();
                if (_server.Certificate != null)
                {
                    var ssl = new SslStream(_stream, false);
                    await ssl.AuthenticateAsServerAsync(_server.Certificate);
                    _stream = ssl;
                }
                _reader = new System.IO.StreamReader(_stream);
                _writer = new System.IO.StreamWriter(_stream);
                _writer.AutoFlush = true;

                Kooboo.Mail.Smtp.SmtpSession session = new Smtp.SmtpSession(this.Client.Address.ToString());

                // Service ready
                await WriteLineAsync(session.ServiceReady().Render());

                var commandline = await _reader.ReadLineAsync();

                var cancellationToken = _cancellationTokenSource.Token;
                while (!cancellationToken.IsCancellationRequested && commandline != null)
                {
                    var response = session.Command(commandline);
                    if (response.SendResponse)
                    {
                        var responseline = response.Render();
                        await WriteLineAsync(responseline);
                    }

                    if (response.SessionCompleted)
                    {
                        await Kooboo.Mail.Transport.Incoming.Receive(session);
                        session.ReSet();
                    }

                    if (response.Close)
                    {
                        Dispose();
                        break;
                    }

                    // When enter the session state, read till the end . 
                    if (session.State == SmtpSession.CommandState.Data)
                    {
                        var externalto = AddressUtility.GetExternalRcpts(session);
                        var counter = externalto.Count();

                        Kooboo.Data.Log.Instance.Email.Write("--recipants");
                        Kooboo.Data.Log.Instance.Email.WriteObj(externalto);
                        Kooboo.Data.Log.Instance.Email.WriteObj(session.Log);

                        if (counter > 0)
                        {
                            if (!Kooboo.Data.Infrastructure.InfraManager.Test(session.OrganizationId, Data.Infrastructure.InfraType.Email, counter))
                            {
                                await WriteLineAsync("550 you have no enough credit to send emails");
                                Dispose();
                                break;
                            }
                        }

                        var data = await _stream.ReadToDotLine(TimeSpan.FromSeconds(60));

                        var dataresponse = session.Data(data);

                        if (dataresponse.SendResponse)
                        {
                            await WriteLineAsync(dataresponse.Render());
                        }

                        if (dataresponse.SessionCompleted)
                        {
                            await Kooboo.Mail.Transport.Incoming.Receive(session);

                            var tos = session.Log.Keys.Where(o => o.Name == SmtpCommandName.RCPTTO);

                            string subject = "TO: ";
                            if (tos != null)
                            {
                                foreach (var item in tos)
                                {
                                    if (item != null && !string.IsNullOrWhiteSpace(item.Value))
                                    {
                                        subject += item.Value;
                                    }
                                }
                            }

                            if (counter > 0)
                            {
                                Kooboo.Data.Infrastructure.InfraManager.Add(session.OrganizationId, Data.Infrastructure.InfraType.Email, counter, subject);
                            }

                            session.ReSet();

                            OnDataCompleted();
                        }

                        if (dataresponse.Close)
                        {
                            Dispose();
                        }
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        commandline = await _reader.ReadLineAsync();
                    }
                }

            }
            catch (ObjectDisposedException)
            {
                // Caused by our active connection closing, no need to handle as exception
            }
            catch (Exception ex)
            {
                try
                {
                    if (_client.Connected)
                    {
                        await WriteLineAsync("550 Internal Server Error");
                    }
                }
                catch
                {
                }
                Kooboo.Data.Log.Instance.Exception.Write(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Source);
            }
            finally
            {
                _server._connectionManager.RemoveConnection(Id);
                Dispose();
            }
        }

        private Task WriteLineAsync(string line)
        {
            return _writer.WriteAsync(line + "\r\n");
        }

        public void CheckTimeout()
        {
            var timestamp = _server.Heartbeat.UtcNow.Ticks;
            if (timestamp > Interlocked.Read(ref _timeoutTimestamp))
            {
                _timeoutTimestamp = Int64.MaxValue;
                Dispose();
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposing, 1) == 0)
            {
                _cancellationTokenSource.Cancel();
                _reader.Dispose();
                _writer.Dispose();
                _client.Close();
            }
        }

        private void OnDataCompleted()
        {
            _receivedMails++;
            if (_receivedMails >= _server.Options.MailsPerConnection)
            {
                Dispose();
                return;
            }

            Interlocked.Exchange(ref _timeoutTimestamp, DateTime.UtcNow.Add(_server.Options.LiveTimeout).Ticks);
        }

        private IPEndPoint CopyIPEndPoint(IPEndPoint ip)
        {
            return new IPEndPoint(ip.Address, ip.Port);
        }
    }
}
