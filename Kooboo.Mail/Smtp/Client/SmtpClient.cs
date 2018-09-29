//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System; 
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Kooboo.Mail.Smtp
{
    public class SmtpClient : IDisposable
    {
        private TcpClient _tcpClient;
        private MailStream _stream;

        public SmtpClient(IPAddress ip)
        {
            _tcpClient = new TcpClient(new IPEndPoint(ip, 0));
            _tcpClient.SendTimeout = 15000;
            _tcpClient.ReceiveTimeout = 10000; 
        }

        public int Timeout
        {
            get
            {
                return _tcpClient.ReceiveTimeout;
            }
            set
            {
                _tcpClient.ReceiveTimeout =
                _tcpClient.SendTimeout = value;
            }
        }

        public ISmtpLogger Logger { get; set; }

        public bool Connected
        {
            get
            {
                return _tcpClient.Connected;
            }
        }

        public async Task Connect(string host, int port)
        {
            LogTryConnect(host);
            try
            {
                await _tcpClient.ConnectAsync(host, port);
                LogConnected();
            }
            catch
            {
                LogConnectionFailed();
                throw;
            }

            _stream = new MailStream(_tcpClient, _tcpClient.GetStream());
        }

        public async Task<SmtpReply> CheckServiceReady()
        {
            var reply = await GetReply();

            return CheckReply(reply, SmtpStatusCode.ServiceReady);
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
            return Command("MAIL FROM: <" + sender + ">", SmtpStatusCode.Ok);
        }

        public Task<SmtpReply> RcptTo(string to)
        {
            return Command("RCPT TO: <" + to + ">", SmtpStatusCode.Ok);
        }

        public async Task<SmtpReply> Data(string content)
        {
            var reply = await Command("DATA", SmtpStatusCode.StartMailInput);
            if (!reply.Ok)
                return reply; 
            return await Data(DoubleDot(content) +   "\r\n.", SmtpStatusCode.Ok);
        }

        public static string DoubleDot(string content)
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

        public void Dispose()
        {
            _tcpClient?.Close();
        }

        public void Release()
        {                
            this.Dispose(); 
        }

        private async Task<SmtpReply> Command(string command, SmtpStatusCode ok)
        {
            await _stream.WriteLineAsync(command);
            LogWrite(command);

            var reply = await GetReply();

            return CheckReply(reply, ok);
        }

        private async Task<SmtpReply> Data(string data, SmtpStatusCode ok)
        {
            await _stream.WriteLineAsync(data);
            LogWrite("...");

            var reply = await GetReply();

            return CheckReply(reply, ok);
        }

        private async Task<string> GetReply()
        {
            var readResult = await _stream.ReadToEndAsync(line => line[3] == ' ');
            LogRead(readResult.Text);

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

        private void LogRead(string message)
        {
            if (Logger == null)
                return;

            Logger.LogRead(
                _tcpClient.Client.LocalEndPoint as IPEndPoint,
                _tcpClient.Client.RemoteEndPoint as IPEndPoint,
                message);
        }

        private void LogWrite(string message)
        {
            if (Logger == null)
                return;

            Logger.LogWrite(
                _tcpClient.Client.LocalEndPoint as IPEndPoint, 
                _tcpClient.Client.RemoteEndPoint as IPEndPoint, 
                message);
        }

        private void LogTryConnect(string host)
        {
            if (Logger == null)
                return;

            Logger.LogTryConnect(host);
        }

        private void LogConnected()
        {
            if (Logger == null)
                return;

            Logger.LogConnected(
                _tcpClient.Client.LocalEndPoint as IPEndPoint,
                _tcpClient.Client.RemoteEndPoint as IPEndPoint);
        }

        private void LogConnectionFailed()
        {
            if (Logger == null)
                return;

            Logger.LogConnectionFailed();
        }
    }
}
