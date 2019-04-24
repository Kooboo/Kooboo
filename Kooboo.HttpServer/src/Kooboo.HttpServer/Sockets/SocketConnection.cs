using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using Kooboo.HttpServer.Http;

namespace Kooboo.HttpServer.Sockets
{
    public class SocketConnection
    {
        private const int MinAllocBufferSize = 2048;

        private ILogger _trace;
        private Socket _socket;

        private SocketReceiver _receiver;
        private SocketSender _sender;

        private SslAdapter _sslAdapter;
        private HttpConnection _httpConnection;

        private bool _initializing = true;
        private readonly TaskCompletionSource<object> _socketClosedTcs = new TaskCompletionSource<object>();
        private static Action<Exception, object> _completeTcs = CompleteTcs;
        private volatile bool _aborted;

        private static long _lastHttpConnectionId = long.MinValue;

        public SocketConnection(Socket socket, ServiceContext context)
        {
            _socket = socket;
            ServiceContext = context;
            _trace = ServiceContext.Log;

            var localEndPoint = (IPEndPoint)_socket.LocalEndPoint;
            var remoteEndPoint = (IPEndPoint)_socket.RemoteEndPoint;

            var httpConnectionId = Interlocked.Increment(ref _lastHttpConnectionId);
            ConnectionFeature = new ConnectionFeature
            {
                ConnectionId = CorrelationIdGenerator.GetNextId(),
                HttpConnectionId= httpConnectionId,
                LocalEndPoint = localEndPoint,
                RemoteEndPoint = remoteEndPoint,
            };

            _receiver = new SocketReceiver(_socket);
            _sender = new SocketSender(_socket);
        }

        public ServiceContext ServiceContext { get; set; }

        public ConnectionFeature ConnectionFeature { get; set; }

        public Pipe ReceivePipe { get; set; }
        public Pipe SendPipe { get; set; } 

        public async Task StartAsync()
        {
            Exception sendError = null;
            try
            {
                ReceivePipe = new Pipe(new PipeOptions(
                    pool: ServiceContext.MemoryPool
                ));
                SendPipe = new Pipe(new PipeOptions(
                    pool: ServiceContext.MemoryPool
                ));

                _ = HandleConnection();

                sendError = await ReceiveNSend();
            }
            catch (Exception ex)
            { 
                _trace.LogError($"Unexpected exception in {nameof(SocketConnection)}.{nameof(StartAsync)}.", ex);
            }
            finally
            {
                // Complete the output after disposing the socket
                SendPipe.Reader.Complete(sendError);
            }
        }

        private async Task HandleConnection()
        {
            // Todo: Add connection limit here

            _trace.ConnectionStart(ConnectionFeature.ConnectionId);

            try
            {
                var context = new HttpConnectionContext
                {
                    ServiceContext = ServiceContext,
                    ReceivePipe = ReceivePipe,
                    SendPipe = SendPipe
                };
                context.Features.Connection = ConnectionFeature;

                _httpConnection = new HttpConnection(context);
                Task adapterTask = Task.FromResult(false);

                if (ServiceContext.ServerOptions.SslEnabled && ServiceContext.ServerOptions.IsHttps)
                {
                    // Wrap SSL and open new pipes
                    _sslAdapter = new SslAdapter(context);
                    await _sslAdapter.PrepareSsl(_httpConnection);
                    adapterTask = _sslAdapter.ReceiveNSend();
                }

                _initializing = false;
                var processTask = _httpConnection.Execute();

                CreateAssociatedClosing();

                await processTask;
                await adapterTask;
                await _socketClosedTcs.Task;
            }
            catch (Exception ex)
            {
                _trace.LogError($"{nameof(SocketConnection)}.{nameof(HandleConnection)}() {ConnectionFeature.ConnectionId}", ex);
            }
            finally
            {
                if (_sslAdapter != null)
                {
                    _sslAdapter.Dispose();
                }

                _trace.ConnectionStop(ConnectionFeature.ConnectionId);
            }
        }

        private async Task<Exception> ReceiveNSend()
        {
            Exception sendError;

            // Spawn send and receive logic
            Task receiveTask = DoReceive();
            Task<Exception> sendTask = DoSend();

            // If the sending task completes then close the receive
            // We don't need to do this in the other direction because the kestrel
            // will trigger the output closing once the input is complete.
            if (await Task.WhenAny(receiveTask, sendTask) == sendTask)
            {
                // Tell the reader it's being aborted
                _socket.Dispose();
            }

            // Now wait for both to complete
            await receiveTask;
            sendError = await sendTask;

            // Dispose the socket(should noop if already called)
            _socket.Dispose();

            return sendError;
        }

        private async Task DoReceive()
        {
            Exception error = null;

            var pipeWriter = ReceivePipe.Writer;

            try
            {
                while (true)
                {
                    // Ensure we have some reasonable amount of buffer space
                    var buffer = pipeWriter.GetMemory(MinAllocBufferSize);

                    try
                    {
                        var bytesReceived = await _receiver.ReceiveAsync(buffer);

                        if (bytesReceived == 0)
                        {
                            // FIN
                            _trace.ConnectionReadFin(ConnectionFeature.ConnectionId);
                            break;
                        }

                        pipeWriter.Advance(bytesReceived);
                    }
                    finally
                    {
                        pipeWriter.Commit();
                    }

                    var flushTask = pipeWriter.FlushAsync();

                    if (!flushTask.IsCompleted)
                    {
                        _trace.ConnectionPause(ConnectionFeature.ConnectionId);

                        await flushTask;

                        _trace.ConnectionResume(ConnectionFeature.ConnectionId);
                    }

                    var result = flushTask.GetAwaiter().GetResult();
                    if (result.IsCompleted)
                    {
                        // Pipe consumer is shut down, do we stop writing
                        break;
                    }
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
            {
                error = new ConnectionResetException(ex.Message, ex);
                _trace.ConnectionReset(ConnectionFeature.ConnectionId);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted ||
                                             ex.SocketErrorCode == SocketError.ConnectionAborted ||
                                             ex.SocketErrorCode == SocketError.Interrupted ||
                                             ex.SocketErrorCode == SocketError.InvalidArgument)
            {
                if (!_aborted)
                {
                    // Calling Dispose after ReceiveAsync can cause an "InvalidArgument" error on *nix.
                    error = new ConnectionAbortedException();
                    _trace.ConnectionError(ConnectionFeature.ConnectionId, error);
                }
            }
            catch (ObjectDisposedException)
            {
                if (!_aborted)
                {
                    error = new ConnectionAbortedException();
                    _trace.ConnectionError(ConnectionFeature.ConnectionId, error);
                }
            }
            catch (IOException ex)
            {
                error = ex;
                _trace.ConnectionError(ConnectionFeature.ConnectionId, error);
            }
            catch (Exception ex)
            {
                error = new IOException(ex.Message, ex);
                _trace.ConnectionError(ConnectionFeature.ConnectionId, error);
            }
            finally
            {
                if (_aborted)
                {
                    error = error ?? new ConnectionAbortedException();
                }
                pipeWriter.Complete(error);
            }
        }

        private async Task<Exception> DoSend()
        {
            Exception error = null;

            var pipeReader = SendPipe.Reader;

            try
            {
                while (true)
                {
                    // Wait for data to write from the pipe producer
                    var result = await pipeReader.ReadAsync();
                    var buffer = result.Buffer;

                    if (result.IsCancelled)
                    {
                        break;
                    }

                    try
                    {
                        if (!buffer.IsEmpty)
                        {
                            await _sender.SendAsync(buffer);
                        }
                        else if (result.IsCompleted)
                        {
                            break;
                        }
                    }
                    finally
                    {
                        pipeReader.AdvanceTo(buffer.End);
                    }
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
            {
                error = null;
            }
            catch (ObjectDisposedException)
            {
                error = null;
            }
            catch (IOException ex)
            {
                error = ex;
            }
            catch (Exception ex)
            {
                error = new IOException(ex.Message, ex);
            }
            finally
            {
                // Make sure to close the connection only after the _aborted flag is set.
                // Without this, the RequestsCanBeAbortedMidRead test will sometimes fail when
                // a BadHttpRequestException is thrown instead of a TaskCanceledException.
                _aborted = true;
                _trace.ConnectionWriteFin(ConnectionFeature.ConnectionId);
                _socket.Shutdown(SocketShutdown.Both);
            }

            return error;
        }

        private void CreateAssociatedClosing()
        {
            var inputTcs = new TaskCompletionSource<object>();
            var outputTcs = new TaskCompletionSource<object>();

            // The reason we don't fire events directly from these callbacks is because it seems
            // like the transport callbacks root the state object (even after it fires)

            _httpConnection.Context.ReceivePipe.Reader.OnWriterCompleted(_completeTcs, inputTcs);
            _httpConnection.Context.SendPipe.Writer.OnReaderCompleted(_completeTcs, outputTcs);

            inputTcs.Task.ContinueWith((task, state) =>
            {
                Abort(task.Exception?.InnerException);
            },
            _httpConnection, TaskContinuationOptions.ExecuteSynchronously);

            outputTcs.Task.ContinueWith((task, state) =>
            {
                OnConnectionClosed(task.Exception?.InnerException);
            },
            _httpConnection, TaskContinuationOptions.ExecuteSynchronously);
        }

        private void Abort(Exception ex)
        {
            if (_initializing)
            {
                SendPipe.Reader.CancelPendingRead();

                _httpConnection.Context.ReceivePipe.Reader.Complete();
                _httpConnection.Context.ReceivePipe.Writer.Complete();
            }
            else
            {
                _httpConnection.Abort(ex);
            }
        }

        private void OnConnectionClosed(Exception ex)
        {
            _httpConnection.Abort(ex);

            _socketClosedTcs.TrySetResult(null);
        }

        private static void CompleteTcs(Exception error, object state)
        {
            var tcs = (TaskCompletionSource<object>)state;

            if (error != null)
            {
                tcs.TrySetException(error);
            }
            else
            {
                tcs.TrySetResult(null);
            }
        }
    }
}
