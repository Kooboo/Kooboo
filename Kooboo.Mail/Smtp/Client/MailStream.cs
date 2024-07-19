//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Smtp
{
    public class MailStream
    {
        private TcpClient _client;
        private Stream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;

        public MailStream(TcpClient client, Stream stream)
        {
            _client = client;
            _stream = stream;
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream, Encoding.GetEncoding("us-ascii"));
            _writer.NewLine = "\r\n";
        }

        #region Async

        public async Task<string> ReadLineAsync()
        {
            var task = _reader.ReadLineAsync();
            if (_client.ReceiveTimeout == 0)
                return await task;

            var timeoutTask = Task.Delay(_client.ReceiveTimeout);

            await Task.WhenAny(task, timeoutTask);
            if (!task.IsCompleted)
                throw new SocketException((int)SocketError.TimedOut);

            return task.Result;
        }

        public async Task<ReadResult> ReadBeforeEndAsync(Func<string, bool> isEndLine)
        {
            var builder = new StringBuilder();
            var line = await ReadLineAsync();
            while (!isEndLine(line))
            {
                builder.AppendLine(line);
                line = await ReadLineAsync();
            }
            return new ReadResult
            {
                Text = builder.ToString(),
                EndLine = line
            };
        }

        public async Task<ReadResult> ReadToEndAsync(Func<string, bool> isEndLine)
        {
            var line = await ReadLineAsync();
            if (isEndLine(line))
                return new ReadResult { Text = line, EndLine = line };

            var builder = new StringBuilder();
            while (!isEndLine(line))
            {
                builder.AppendLine(line);
                line = await ReadLineAsync();
            }
            builder.Append(line);
            return new ReadResult
            {
                Text = builder.ToString(),
                EndLine = line
            };
        }

        public async Task WriteLineAsync(string str)
        {
            await _writer.WriteLineAsync(str);
            var task = _writer.FlushAsync();
            if (_client.SendTimeout == 0)
            {
                await task;
                return;
            }

            var timeoutTask = Task.Delay(_client.SendTimeout);

            await Task.WhenAny(task, timeoutTask);
            if (!task.IsCompleted)
                throw new SocketException((int)SocketError.TimedOut);
        }

        #endregion

        #region Sync

        public string ReadLine()
        {
            return _reader.ReadLine();
        }

        public ReadResult ReadBeforeEnd(Func<string, bool> isEndLine)
        {
            var builder = new StringBuilder();
            var line = ReadLine();
            while (!isEndLine(line))
            {
                builder.AppendLine(line);
                line = ReadLine();
            }
            return new ReadResult
            {
                Text = builder.ToString(),
                EndLine = line
            };
        }

        public ReadResult ReadToEnd(Func<string, bool> isEndLine)
        {
            var line = ReadLine();
            if (isEndLine(line))
                return new ReadResult { Text = line, EndLine = line };

            var builder = new StringBuilder();
            while (!isEndLine(line))
            {
                builder.AppendLine(line);
                line = ReadLine();
            }
            builder.Append(line);
            return new ReadResult
            {
                Text = builder.ToString(),
                EndLine = line
            };
        }

        public void Write(string str)
        {
            _writer.Write(str);
            _writer.Flush();
        }

        public void WriteLine()
        {
            _writer.WriteLine();
            _writer.Flush();
        }

        public void WriteLine(string str)
        {
            _writer.WriteLine(str);
            _writer.Flush();
        }

        #endregion

        public class ReadResult
        {
            public string Text { get; set; }

            public string EndLine { get; set; }
        }
    }
}
