//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kooboo.Mail.Utility;


namespace Kooboo.Mail.Imap
{
    public class ImapStream : IDisposable
    {

        private Stream _stream;
        private PipeReader _reader;
        private PipeWriter _writer;

        private IPEndPoint _local;
        private IPEndPoint _remote;

        protected ImapStream()
        {
        }

        public ImapStream(TcpClient client, Stream stream)
        {
            _local = client.Client.LocalEndPoint as IPEndPoint;
            _remote = client.Client.RemoteEndPoint as IPEndPoint;

            _stream = stream;

            _reader = PipeReader.Create(_stream);
            _writer = PipeWriter.Create(_stream);
        }

        public virtual async Task<byte[]> ReadAsync(int count, CancellationToken cancellationToken = default)
        {
            var result = await _reader.ReadToAsync(count, TimeSpan.FromMinutes(30));
            MailLogger.WriteLine(_stream, result, "IMAP", true);
            return result;
        }

        public virtual async Task<string> ReadLineAsync()
        {
            var result = await _reader.ReadLineAsync(Encoding.Default, TimeSpan.FromSeconds(30));
            MailLogger.WriteLine(_stream, result, "IMAP", true);
            return result;
        }

        public virtual async Task WriteAsync(byte[] buffer)
        {
            MailLogger.WriteLine(_stream, buffer, "IMAP", false);
            await _writer.WriteAsync(buffer);
            await _writer.FlushAsync();
        }

        public virtual async Task WriteLineAsync(string line)
        {
            MailLogger.WriteLine(_stream, line, "IMAP", false);
            await _writer.WriteLineAsync(line, Encoding.UTF8, 30);
            await _writer.FlushAsync();
        }

        public void Dispose()
        {

        }

        private static Logging.ILogger _logger;
        static ImapStream()
        {
            _logger = Logging.LogProvider.GetLogger("imap", "imap");
        }

        private void LogRead(string line)
        {
            if (String.IsNullOrEmpty(line))
                return;

            _logger.LogDebug($"{_remote.Address} C: " + line);
        }

        private void LogWrite(string line)
        {
            if (line == "* BAD Empty command line")
                return;

            _logger.LogDebug($"{_remote.Address} S: " + line);
        }
    }

    public static class ImapStreamExtensions
    {
        /// <summary>
        /// https://tools.ietf.org/html/rfc3501#section-2.2.1
        /// https://tools.ietf.org/html/rfc3501#section-4.3
        /// </summary>
        public static async Task<CommandLine> ReadCommandAsync(this ImapStream stream)
        {
            var line = await stream.ReadLineAsync();
            if (String.IsNullOrWhiteSpace(line))
            {
                // await stream.WriteLineAsync("* BAD Empty command line");
                return null;
                //line = await stream.ReadLineAsync();
            }

            var spl = line.Split(new char[] { ' ' }, 3);

            // No command name
            if (spl.Length < 2)
                return null;

            var result = new CommandLine
            {
                Tag = spl[0],
                Name = spl[1].ToUpperInvariant()
            };

            // No argument
            if (spl.Length < 3)
                return result;

            // Rest part as default argument
            result.Args = spl[2];

            // APPEND is special to handle later
            if (result.Name == "APPEND")
                return result;

            var partialArgs = ArgumentUtility.Parse(result.Args);
            // One line command
            if (partialArgs.Size == null)
                return result;

            /* Continous command
             * R <tag> <command> <partial argument>{<continue size>}
             * W + Continue.
             * R ...
             * R <partial argument>{<continue size>}
             * W + Continue.
             * R ...
             * R <end argument>
             */
            var builder = new StringBuilder();
            while (partialArgs.Size != null)
            {
                builder.Append(partialArgs.Args);

                await stream.WriteLineAsync("+ Continue.");

                // Read bytes secified by literal {size}
                var bytes = await stream.ReadAsync(partialArgs.Size.Value);
                builder.Append(TextUtils.QuoteString(Encoding.ASCII.GetString(bytes)));

                // Try to read last line
                line = await stream.ReadLineAsync();
                if (line == null) return null;
                partialArgs = ArgumentUtility.Parse(line);
            }

            builder.Append(partialArgs.Args);
            result.Args = builder.ToString();

            return result;
        }


    }
}
