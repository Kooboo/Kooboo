using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Mail.Smtp.Client
{
    public class PipeMailStream : IDuplexPipe, IDisposable
    {
        private Stream _stream;
        public PipeMailStream(Stream stream)
        {
            _stream = stream;
            Input = PipeReader.Create(_stream);
            Output = PipeWriter.Create(_stream);
        }

        public PipeReader Input { get; set; }

        public PipeWriter Output { get; set; }

        private int CommandTimeOut = 2 * 60 * 1000;   // in milliseconds.

        private CancellationToken NewToken()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            source.CancelAfter(CommandTimeOut);
            return source.Token;
        }

        public async Task<ReadResult> ReadServerResponseAsync()
        {
            //see: https://datatracker.ietf.org/doc/html/rfc2821#section-4.5.3.2

            var line = await Input.ReadLineAsync(Encoding.ASCII, NewToken());
            if (EndOfLine(line))
                return new ReadResult { Text = line, EndLine = line };

            string result = null;
            while (line != null && !EndOfLine(line))
            {
                result += line + "\r\n";
                line = await Input.ReadLineAsync(Encoding.ASCII, NewToken());
            }

            result += line;

            return new ReadResult
            {
                Text = result,
                EndLine = line
            };
        }


        public async Task<PipeLineResponse> ReadServerPipeliningResponseAsync(int count)
        {
            PipeLineResponse res = new PipeLineResponse();
            int ReadCount = 0;

            var line = await Input.ReadLineAsync(Encoding.ASCII, NewToken());
            string lineResult = null;

            while (line != null)
            {
                lineResult += line;
                if (EndOfLine(line))
                {
                    res.lines.Add(lineResult);
                    lineResult = null;
                    ReadCount += 1;
                    if (ReadCount >= count)
                    {
                        break;
                    }
                }
                line = await Input.ReadLineAsync(Encoding.ASCII, NewToken());
            }

            return res;

        }


        private bool EndOfLine(string input)
        {
            if (input != null && input.Length > 5)
            {
                if (input[3] == ' ' || input[3] == '\t')
                {
                    if (char.IsDigit(input[0]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public void Dispose()
        {
            if (_stream is not null)
            {
                _stream.Dispose();
                Input.Complete();
                Output.Complete();
            }
        }

        public class ReadResult
        {
            public string Text { get; set; }

            public string EndLine { get; set; }
        }

        public class PipeLineResponse

        {

            public List<string> lines = new List<string>();
        }
    }
}

