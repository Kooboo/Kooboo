//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using LumiSoft.Net; 

namespace Kooboo.Mail.Imap
{
    public class ImapStream : IDisposable
    {
        private Stream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

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
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);
            _writer.AutoFlush = true;
        }

        public virtual async Task ReadAsync(byte[] buffer, int offset, int count)
        {
            int hasRead = 0, actual = 0;
            do
            {
                actual = await _stream.ReadAsync(buffer, offset + hasRead, count - hasRead);

                hasRead += actual;
            }
            while (hasRead < count && actual > 0);

            LogRead("...");
        }

        public virtual async Task<byte[]> ReadAsync(int count)
        {
            var buffer = new byte[count];

            await ReadAsync(buffer, 0, count);

            return buffer;
        }

        public virtual async Task<string> ReadLineAsync()
        {
            var line = await _reader.ReadLineAsync();

            LogRead(line);

            return line;
        }

        public virtual async Task WriteAsync(byte[] buffer)
        {
            await _stream.WriteAsync(buffer, 0, buffer.Length);
            await _stream.FlushAsync();

            var str = Encoding.UTF8.GetString(buffer);
            var index = 0;
            while (index >= 0)
            {
                index = str.IndexOf("\r\n", index + 2);
            }

            LogWrite("...");
        }

        public virtual async Task WriteLineAsync(string line)
        {
            await _writer.WriteAsync(line + "\r\n");

            LogWrite(line);
        }
        

        public void Dispose()
        {
            _reader.Dispose();
            _writer.Dispose();
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
            while (String.IsNullOrEmpty(line))
            {
                await stream.WriteLineAsync("* BAD Empty command line");
                line = await stream.ReadLineAsync();
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
                builder.Append(TextUtils.QuoteString(Encoding.UTF8.GetString(bytes)));

                // Try to read last line
                line = await stream.ReadLineAsync();
                partialArgs = ArgumentUtility.Parse(line);
            }

            builder.Append(partialArgs.Args);
            result.Args = builder.ToString();

            return result;
        }

        public static Task WriteStatusAsync(this ImapStream stream, string status, string message)
        {
            return stream.WriteLineAsync(status + " " + message);
        }
    }
}
