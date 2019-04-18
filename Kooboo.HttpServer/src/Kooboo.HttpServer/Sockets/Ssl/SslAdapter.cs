using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipelines;
using System.Net.Security;
using System.Buffers;
using StreamExtended;
using StreamExtended.Network;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace Kooboo.HttpServer.Http
{
    public class SslAdapter : IDisposable
    {
        private const int MinAllocBufferSize = 2048;

        private MemoryPool _memoryPool;
        private Pipe _receivePipe;
        private Pipe _sendPipe;

        private SslStream _sslStream;

        public SslAdapter(HttpConnectionContext context)
        {
            Context = context;

            _receivePipe = Context.ReceivePipe;
            _sendPipe = Context.SendPipe;
        }

        public HttpConnectionContext Context { get; set; }

        //private string GetProtocal()
        public async Task PrepareSsl(HttpConnection httpConnection)
        {
            var stream = new RawStream(_receivePipe.Reader, _sendPipe.Writer);

            // todo: 假如客户端不支持SNI

            // Get host name
            var wrappedStream = new CustomBufferedStream(stream, 4096);
            var clientHelloInfo = await SslTools.PeekClientHello(wrappedStream);

            if (clientHelloInfo == null)
                return;

            var hostName = clientHelloInfo.Extensions?.FirstOrDefault(o => o.Value.Name == "server_name").Value?.Data;
            var protocol = clientHelloInfo.Extensions?.FirstOrDefault(o => o.Value.Name == "ALPN").Value?.Data;
            if (!string.IsNullOrEmpty(protocol))
            {
                string[] stringList = protocol.Split(',');
                protocol = stringList[0].Trim();
            }

            // Select certificate
            var certificate =  Context.ServiceContext.ServerOptions.SelectCertificate(hostName);

            // Switch to SSL
            _sslStream = new SslStream(wrappedStream);

            System.Security.Authentication.SslProtocols sslProtocols =SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;

            await _sslStream.AuthenticateAsServerAsync(certificate,false, sslProtocols, false);

            Context.ReceivePipe = new Pipe(new PipeOptions(
                pool: _memoryPool
            ));
             Context.SendPipe = new Pipe(new PipeOptions(
                pool: _memoryPool
            ));
           
            httpConnection.ApplicationProtocol = protocol;
        }

        public async Task ReceiveNSend()
        {
            var inputTask = ReceiveAsync(_sslStream);
            var outputTask = SendAsync(_sslStream);

            await inputTask;
            await outputTask;
        }

        private async Task SendAsync(SslStream stream)
        {
            Exception error = null;

            var pipeReader = Context.SendPipe.Reader;

            try
            {
                if (stream == null)
                {
                    return;
                }

                while (true)
                {
                    var result = await pipeReader.ReadAsync();
                    var buffer = result.Buffer;

                    try
                    {
                        if (result.IsCancelled)
                        {
                            // Forward the cancellation to the transport pipe
                            _sendPipe.Reader.CancelPendingRead();
                            break;
                        }

                        if (buffer.IsEmpty)
                        {
                            if (result.IsCompleted)
                            {
                                break;
                            }
                            await stream.FlushAsync();
                        }
                        else if (buffer.IsSingleSegment)
                        {
                            var array = buffer.First.GetArray();
                            await stream.WriteAsync(array.Array, array.Offset, array.Count);
                        }
                        else
                        {
                            foreach (var memory in buffer)
                            {
                                var array = memory.GetArray();
                                await stream.WriteAsync(array.Array, array.Offset, array.Count);
                            }
                        }
                    }
                    finally
                    {
                        pipeReader.AdvanceTo(buffer.End);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                
                pipeReader.Complete();
                _sendPipe.Writer.Complete();
                
            }
        }

        private async Task ReceiveAsync(SslStream stream)
        {
            Exception error = null;

            var pipeWriter = Context.ReceivePipe.Writer;

            try
            {
                if (stream == null)
                {
                    // REVIEW: Do we need an exception here?
                    return;
                }

                while (true)
                {

                    var outputBuffer = pipeWriter.GetMemory(MinAllocBufferSize);

                    var array = outputBuffer.GetArray();
                    try
                    {
                        var bytesRead = await stream.ReadAsync(array.Array, array.Offset, array.Count);
                        pipeWriter.Advance(bytesRead);

                        if (bytesRead == 0)
                        {
                            // FIN
                            break;
                        }
                    }
                    finally
                    {
                        pipeWriter.Commit();
                    }

                    var result = await pipeWriter.FlushAsync();

                    if (result.IsCompleted)
                    {
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                // Don't rethrow the exception. It should be handled by the Pipeline consumer.
                error = ex;
            }
            finally
            {
                pipeWriter.Complete(error);
                // The application could have ended the input pipe so complete
                // the transport pipe as well
                _receivePipe.Reader.Complete();
            }
        }

        public void Dispose()
        {
            if (_sslStream !=null)
            {
                _sslStream.Dispose();
            }
            
        }
    }
}
