using System;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Lib.Whois
{

    public class TcpPipeReader
    {
        public async Task<string> ReadAsync(string server, string Domain)
        {
            try
            {
                TcpClient tcpClient = new TcpClient();

                tcpClient.Connect(server, 43);

                var _stream = tcpClient.GetStream();

                _stream = tcpClient.GetStream();

                _stream.ReadTimeout =
                _stream.WriteTimeout = 30;

                var _reader = PipeReader.Create(_stream);
                var _writer = PipeWriter.Create(_stream);

                using var source = new CancellationTokenSource();
                source.CancelAfter(30000);

                await _writer.WriteLineAsync(Domain, Encoding.UTF8, source.Token);

                var result = await _reader.ReadToEndAsync(source.Token);

                return Encoding.UTF8.GetString(result);
            }
            catch (Exception)
            {

            }
            return null;
        }

        public string Read(string server, string domain)
        {
            return this.ReadAsync(server, domain).Result;
        }

        public string ReadIana(string tld)
        {
            return Read("whois.iana.org", tld);
        }

        public async Task<string> ReadIanaAsync(string tld)
        {
            return await ReadAsync("whois.iana.org", tld);
        }
    }







}
