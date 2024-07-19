//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kooboo.Mail.Smtp.Client;

namespace Kooboo.Mail.Smtp
{
    public class SmtpClient : IDisposable
    {
        private TcpClient _tcpClient;
        //private MailStream _stream;
        private PipeMailStream _pipeStream;

        public SmtpClient(IPAddress LocalIP)
        {
            Init(LocalIP);
        }
        public SmtpClient()
        {
            Init(System.Net.IPAddress.Any);
        }

        private void Init(IPAddress LocalIP)
        {
            //see:https://datatracker.ietf.org/doc/html/rfc2821#section-4.5.3.2
            _tcpClient = new TcpClient(new IPEndPoint(LocalIP, 0));
            _tcpClient.SendTimeout = 3 * 60 * 1000; // default to 3 minutes.
            _tcpClient.ReceiveTimeout = 3 * 60 * 1000; // default to 3 minutes.;
        }

        private int DefaultTimeOut = 2 * 60 * 1000;
        private CancellationToken NewToken()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            source.CancelAfter(DefaultTimeOut);
            return source.Token;
        }

        public bool Connected
        {
            get
            {
                return _tcpClient.Connected;
            }
        }

        public async Task Connect(string Host, int Port)
        {
            // await _tcpClient.ConnectAsync(Host, Port, NewToken());
            await _tcpClient.ConnectAsync(Host, Port);
            _pipeStream = new PipeMailStream(_tcpClient.GetStream());
        }

        public async Task Connect(System.Net.IPAddress RemoteIP, int Port)
        {
            await _tcpClient.ConnectAsync(RemoteIP, Port, NewToken());
            _pipeStream = new PipeMailStream(_tcpClient.GetStream());
        }

        public async Task ConnectSsl(string Host, int Port)
        {
            await _tcpClient.ConnectAsync(Host, Port, NewToken());

            var sslStream = new SslStream(_tcpClient.GetStream());

            sslStream.AuthenticateAsClient(new SslClientAuthenticationOptions()
            {
                TargetHost = Host,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    if (sslPolicyErrors == SslPolicyErrors.None)
                        return true;
                    if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
                        return true;
                    return false;
                }
            });

            _pipeStream = new PipeMailStream(sslStream);
        }

        public async Task<SmtpReply> CheckServiceReady()
        {
            var reply = await GetReply();

            return CheckReply(reply, SmtpStatusCode.ServiceReady);
        }

        public async Task<SmtpReply> StartTls(string host = null)
        {
            var reply = await Command("STARTTLS", SmtpStatusCode.ServiceReady);
            if (!reply.Ok)
            {
                return reply;
            }

            var sslStream = new System.Net.Security.SslStream(_tcpClient.GetStream());
            await sslStream.AuthenticateAsClientAsync(SmtpClientSetting.Options(), NewToken());

            _pipeStream = new PipeMailStream(sslStream);
            return reply;
        }

        public async Task<SmtpReply> Login(string username, string password)
        {
            var reply = await Command("AUTH LOGIN", SmtpStatusCode.StartAuthenticationInput);
            if (!reply.Ok)
                return reply;

            reply = await Data(Base64Encode(username), SmtpStatusCode.StartAuthenticationInput);
            if (!reply.Ok)
                return reply;

            return await Data(Base64Encode(password), SmtpStatusCode.AuthenticationCompleted);
        }

        public async Task<SmtpReply> Data(string data, SmtpStatusCode ok)
        {
            await _pipeStream.Output.WriteLineAsync(data, Encoding.UTF8);

            var reply = await GetReply();
            return CheckReply(reply, ok);
        }

        private string Base64Encode(string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public Task<SmtpReply> Ehlo(string fqdn)
        {
            return Command("EHLO " + fqdn, SmtpStatusCode.Ok);
        }

        public Task<SmtpReply> Helo(string fqdn)
        {
            return Command("HELO " + fqdn, SmtpStatusCode.Ok);
        }

        public Task<SmtpReply> MailFrom(string sender)
        {
            if (sender.Contains("<"))
            {
                return Command("MAIL FROM: " + sender, SmtpStatusCode.Ok);
            }
            else
            {
                return Command("MAIL FROM: <" + sender + ">", SmtpStatusCode.Ok);
            }

        }

        public Task<SmtpReply> RcptTo(string to)
        {
            if (to.Contains("<"))
            {
                return Command("RCPT TO: " + to, SmtpStatusCode.Ok);
            }
            else
            {
                return Command("RCPT TO: <" + to + ">", SmtpStatusCode.Ok);
            }

        }

        public async Task<SmtpReply> Data(string content)
        {
            var reply = await Command("DATA", SmtpStatusCode.StartMailInput);
            if (!reply.Ok)
                return reply;
            return await _SendData(content, SmtpStatusCode.Ok);
        }

        private int DataBufferSize = 4096 * 100;
        private async Task<SmtpReply> _SendData(string data, SmtpStatusCode ok)
        {
            if (data == null)
            {
                return new SmtpReply() { Ok = false, Reply = "Command not sent" };
            }

            //see: https://datatracker.ietf.org/doc/html/rfc2821#section-4.5.3.2 
            var DoubleDotContent = DoubleDot(data);

            if (DoubleDotContent.Length < this.DataBufferSize)
            {
                DoubleDotContent += "\r\n.\r\n";
                var bytes = System.Text.Encoding.UTF8.GetBytes(DoubleDotContent);

                CancellationTokenSource source = new CancellationTokenSource();
                source.CancelAfter(2 * 60 * 1000);
                await _pipeStream.Output.WriteAsync(bytes, source.Token);
                await _pipeStream.Output.FlushAsync();

                var reply = await GetReply();
                return CheckReply(reply, ok);
            }
            else
            {
                var blocks = ContentSplitter.Split(DoubleDotContent, DataBufferSize);

                var len = blocks.Count();

                for (int i = 0; i < len; i++)
                {
                    var blockText = ContentSplitter.ReadBlock(DoubleDotContent, blocks[i]);

                    if (i == len - 1)
                    {
                        blockText += "\r\n.\r\n";
                    }
                    var bytes = System.Text.Encoding.UTF8.GetBytes(blockText);

                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter(2 * 60 * 1000);
                    await _pipeStream.Output.WriteAsync(bytes, source.Token);
                    await _pipeStream.Output.FlushAsync(source.Token);
                }

                var reply = await GetReply();
                return CheckReply(reply, ok);

            }
        }

        public async Task<SmtpReply> DataCommand()
        {
            return await Command("DATA", SmtpStatusCode.StartMailInput);
        }
        public async Task<SmtpReply> DataContent(string content)
        {
            return await _SendData(content, SmtpStatusCode.Ok);
        }

        private string DoubleDot(string content)
        {
            return content.Replace("\r\n.", "\r\n..");
        }

        public Task<SmtpReply> Rset()
        {
            return Command("RSET", SmtpStatusCode.Ok);
        }

        public Task<SmtpReply> Quit()
        {
            return Command("QUIT", SmtpStatusCode.ServiceClosingTransmissionChannel);
        }

        public async Task<List<SmtpReply>> SendCommands(params SendCommand[] commands)
        {
            List<SmtpReply> replies = new List<SmtpReply>();

            foreach (var item in commands)
            {
                await _pipeStream.Output.WriteLineAsync(item.CommandLine, Encoding.ASCII);

                var reply = await GetReply();
                var res = CheckReply(reply, item.ExpectedResponse);

                replies.Add(res);

                if (!res.Ok && item.Command != "RSET")  // this is bug of Kooboo mail server. 
                {
                    return replies;
                }
            }

            return replies;
        }


        /*
 *  In particular, the commands RSET, MAIL FROM,
SEND FROM, SOML FROM, SAML FROM, and RCPT TO can all appear anywhere
in a pipelined command group.  The EHLO, DATA, VRFY, EXPN, TURN,
QUIT, and NOOP commands can only appear as the last command in a
group since their success or failure produces a change of state which
 * 
 */
        public async Task<List<SmtpReply>> PipeLineCommands(params SendCommand[] commands)
        {
            List<SmtpReply> replies = new List<SmtpReply>();

            string CommandLines = null;
            int CmdCount = commands.Count();

            for (int i = 0; i < CmdCount; i++)
            {
                var item = commands[i];
                if (i == CmdCount - 1)
                {
                    CommandLines += item.CommandLine;
                }
                else
                {
                    CommandLines += item.CommandLine + "\r\n";
                }
            }

            await _pipeStream.Output.WriteLineAsync(CommandLines, Encoding.ASCII);

            var res = await _pipeStream.ReadServerPipeliningResponseAsync(commands.Count());

            for (int i = 0; i < CmdCount; i++)
            {
                var line = res.lines[i];

                var cmd = commands[i];

                var checkReply = CheckReply(line, cmd.ExpectedResponse);
                replies.Add(checkReply);
            }

            return replies;
        }


        public List<SendCommand> BuildCommands(string MailFrom, bool IncludeRset, params string[] RcptTo)
        {
            // return Command("RSET", SmtpStatusCode.Ok);
            List<SendCommand> commands = new List<SendCommand>();
            if (IncludeRset)
            {
                commands.Add(new SendCommand() { CommandLine = "RSET", ExpectedResponse = SmtpStatusCode.Ok, Command = "RSET" });
            }

            commands.Add(new SendCommand() { CommandLine = "MAIL FROM: <" + MailFrom + ">", ExpectedResponse = SmtpStatusCode.Ok, Command = "MAILFROM" });

            foreach (var item in RcptTo)
            {
                commands.Add(new SendCommand() { CommandLine = "RCPT TO: <" + item + ">", ExpectedResponse = SmtpStatusCode.Ok, Command = "RCPTTO" });
            }

            commands.Add(new SendCommand() { CommandLine = "DATA", ExpectedResponse = SmtpStatusCode.StartMailInput, Command = "DATA" });

            return commands;
        }


        public void Dispose()
        {
            _tcpClient?.Close();
        }

        private async Task<SmtpReply> Command(string command, SmtpStatusCode statusCode)
        {
            await _pipeStream.Output.WriteLineAsync(command, Encoding.ASCII, NewToken());

            var reply = await GetReply();
            return CheckReply(reply, statusCode);
        }

        private async Task<string> GetReply()
        {
            var readResult = await _pipeStream.ReadServerResponseAsync();
            return readResult.Text;
        }

        private SmtpReply CheckReply(string reply, SmtpStatusCode code)
        {
            return new SmtpReply
            {
                Ok = reply.StartsWith(((int)code).ToString()),
                Reply = reply
            };
        }


        private int SendMailCount = 0;

        public async Task<bool> SendMail(string mailFrom, string to, string msgSource, string Mx, string FQDN, int port = 25)
        {
            if (!this.Connected)
            {
                await this.Connect(Mx, port);
                var checkServiceReply = await CheckServiceReady();
                if (!checkServiceReply.Ok)
                {
                    this.Dispose();
                    return false;
                }
            }

            var reply = await Ehlo(FQDN);

            if (reply.Reply != null && reply.Reply.ToLower().Contains("starttls"))
            {
                reply = await StartTls(Mx);

                if (!reply.Ok)
                {
                    this.Dispose();
                    return false;
                }

                reply = await Ehlo(FQDN);
            }

            reply = await MailFrom(mailFrom);

            if (!reply.Ok)
            {
                this.Dispose();
                return false;
            }

            reply = await RcptTo(to);

            if (!reply.Ok)
            {
                return false;
            }

            reply = await DataCommand();

            if (!reply.Ok)
            {
                return false;
            }

            reply = await DataContent(msgSource);

            return reply.Ok;

        }

    }
}

//timeout: see: https://datatracker.ietf.org/doc/html/rfc2821#section-4.5.3.2
