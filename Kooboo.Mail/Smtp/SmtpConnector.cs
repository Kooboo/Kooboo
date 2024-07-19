//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kooboo.Mail.Utility;

namespace Kooboo.Mail.Smtp
{
    public class SmtpConnector : IDisposable
    {
        private TcpClient _client;
        private Stream _stream;
        private PipeReader _reader;
        private PipeWriter _writer;
        private SmtpServer _server;

        private CancellationTokenSource _cancellationTokenSource;

        public SmtpConnector(SmtpServer server, TcpClient client)
        {
            _server = server;
            _client = client;
            Local = CopyIPEndPoint(_client.Client.LocalEndPoint as IPEndPoint);
            Client = CopyIPEndPoint(_client.Client.RemoteEndPoint as IPEndPoint);
        }

        public IPEndPoint Local { get; set; }

        public IPEndPoint Client { get; set; }

        public async Task Accept()
        {
            // Add cancellation token to allow cancel from any point calling Dispose()
            _cancellationTokenSource = new CancellationTokenSource();

            int MailCounter = 0;

            try
            {
                _stream = _client.GetStream();

                if (_server.SSL)
                {
                    var ssl = new SslStream(_stream, false);

                    X509Certificate2 cert = null;

                    if (this.Local.Port == 587)
                    {

                        cert = Kooboo.Data.SSL.SslCertificateProvider.SelectCertificate2(Settings.Port587SmtpDomain);
                    }
                    else
                    {
                        cert = Kooboo.Data.SSL.SslCertificateProvider.SelectCertificate2(Settings.SmtpDomain);
                    }


                    await ssl.AuthenticateAsServerAsync(cert);
                    _stream = ssl;
                }

                _stream.ReadTimeout =
                _stream.WriteTimeout = _server.Timeout;

                _reader = PipeReader.Create(_stream);
                _writer = PipeWriter.Create(_stream);

                SmtpSession session = new SmtpSession(this.Client.Address);

                var securityResult = Kooboo.Mail.SecurityControl.Manager.Check(this.Client.Address);
                if (!securityResult.CanConnect)
                {
                    await WriteLineAsync("550 " + securityResult.Error);
                    Dispose();
                    return;
                }

                //Service ready
                await WriteLineAsync(session.ServiceReady().Render());
                var commandLine = await _reader.ReadLineAsync(Encoding.Default, TimeSpan.FromSeconds(30));

                MailLogger.WriteLine(_stream, commandLine + " : " + this.Client.Address.ToString(), "SMTP", true);

                var cancellationToken = _cancellationTokenSource.Token;

                while (!cancellationToken.IsCancellationRequested && commandLine != null)
                {
                    var response = session.Command(commandLine);

                    if (response.SendResponse)
                    {
                        var responseline = response.Render();
                        await WriteLineAsync(responseline);
                    }

                    if (response.StartTls)
                    {

                        X509Certificate2 cert = null;

                        if (this.Local.Port == 587)
                        {
                            cert = Kooboo.Data.SSL.SslCertificateProvider.SelectCertificate2(Settings.Port587SmtpDomain);
                        }
                        else
                        {
                            cert = Kooboo.Data.SSL.SslCertificateProvider.SelectCertificate2(Settings.SmtpDomain);
                        }


                        var sslStream = new System.Net.Security.SslStream(_stream, false, ValidateCertificate);

                        // sslStream.AuthenticateAsServer(cert, false, false);
                        sslStream.AuthenticateAsServer(cert, true, true);

                        _stream = sslStream;
                        _reader = PipeReader.Create(_stream);
                        _writer = PipeWriter.Create(_stream);
                        session.ReSet();
                        session.State = SmtpSession.CommandState.Helo;
                    }

                    if (response.SessionCompleted)
                    {
                        await Kooboo.Mail.Transport.Incoming.Receive(session);
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

                        if (counter > 0)
                        {
                            if (!Kooboo.Data.Infrastructure.InfraManager.Test(session.OrganizationId, Data.Infrastructure.InfraType.Email, counter))
                            {
                                await WriteLineAsync("550 you have no enough credit to send emails");
                                Dispose();
                                break;
                            }
                        }

                        var data = await _reader.ReadToDotAsync(TimeSpan.FromMinutes(8));
                        if (data == null) break;

                        MailLogger.WriteLine(_stream, data, "SMTP", true);

                        var dataresponse = await session.Data(data);

                        // after data, can check the result... TODO: when all 3 failed, should return...
                        //if (session.TestResult !=null && session.TestResult.AllFailed)
                        //{ 
                        //}


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

                            MailCounter += 1;
                        }

                        if (dataresponse.Close)
                        {
                            Dispose();
                        }
                    }

                    if (MailCounter > _server.Options.MailsPerConnection)
                    {
                        await this.WriteLineAsync("550 Max mails per connection reached");
                        Dispose();
                        return;
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        commandLine = await _reader.ReadLineAsync(Encoding.Default, TimeSpan.FromSeconds(30));
                        MailLogger.WriteLine(_stream, commandLine, "SMTP", true);
                    }
                }

            }
            catch (ObjectDisposedException)
            {
                // Caused by our active connection closing, no need to handle as exception
            }
            catch (SocketException ex)
            {
                Kooboo.Data.Log.Instance.Exception.Write(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                Kooboo.Data.Log.Instance.Exception.Write(ex.ToString());
            }
            catch (InvalidDataException)
            {
                //maybe client clost connection.
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
                Kooboo.Data.Log.Instance.Exception.Write(ex.Message.ToString());
            }
            finally
            {
                Dispose();
            }
        }

        private bool ValidateCertificate(object s, X509Certificate c, X509Chain h, SslPolicyErrors p) => true;

        private async Task WriteLineAsync(string line)
        {
            MailLogger.WriteLine(_stream, line, $"SMTP", false);
            await _writer.WriteLineAsync(line, Encoding.ASCII, 30);
            await _writer.FlushAsync();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _client?.Close();
        }

        private IPEndPoint CopyIPEndPoint(IPEndPoint ip)
        {
            return new IPEndPoint(ip.Address, ip.Port);
        }
    }
}
