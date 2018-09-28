using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipelines;

namespace Kooboo.HttpServer.Http
{
    public class HttpConnection : ITimeoutControl
    {
        private readonly object _protocolSelectionLock = new object();
        private ProtocolSelectionState _protocolSelectionState = ProtocolSelectionState.Initializing;
        private IRequestProcessor _requestProcessor;

        public HttpConnection(HttpConnectionContext context)
        {
            Context = context;
            Options = context.ServiceContext.ServerOptions;
        }

        public HttpConnectionContext Context { get; set; }

        public string ApplicationProtocol { get; set; }

        public HttpServerOptions Options { get; }

        public async Task Execute()
        {
            IRequestProcessor requestProcessor = null;

            lock (_protocolSelectionLock)
            {
                // Ensure that the connection hasn't already been stopped.
                if (_protocolSelectionState == ProtocolSelectionState.Initializing)
                {
                    //if (ApplicationProtocol=="h2")
                    //{
                    //    requestProcessor = new Http2.Http2Connection(new Http2.Http2ConnectionContext
                    //    {
                    //        Context = Context,
                    //        ToTransport = new PipeConnection(Context.ReceivePipe.Reader, Context.SendPipe.Writer),
                    //        ToApplication = new PipeConnection(Context.SendPipe.Reader, Context.ReceivePipe.Writer)
                    //    });
                    //}
                    //else
                    //{
                        requestProcessor = new Http1Connection(new Http1ConnectionContext
                        {
                            TimeoutControl = this,
                            Context = Context,
                            ToTransport = new PipeConnection(Context.ReceivePipe.Reader, Context.SendPipe.Writer),
                            ToApplication = new PipeConnection(Context.SendPipe.Reader, Context.ReceivePipe.Writer),
                        });
                    
                    _protocolSelectionState = ProtocolSelectionState.Selected;

                    _requestProcessor = requestProcessor;
                }
            }

            if (requestProcessor != null)
            {
                await requestProcessor.ProcessRequestsAsync(Context.ServiceContext.Application);
            }
        }

        public void Abort(Exception ex)
        {
            lock (_protocolSelectionLock)
            {
                switch (_protocolSelectionState)
                {
                    case ProtocolSelectionState.Initializing:
                        // CloseUninitializedConnection();
                        break;
                    case ProtocolSelectionState.Selected:
                    case ProtocolSelectionState.Stopping:
                        _requestProcessor.Abort(ex);
                        break;
                    case ProtocolSelectionState.Stopped:
                        break;
                }

                _protocolSelectionState = ProtocolSelectionState.Stopped;
            }
        }

        public void SetTimeout(long ticks, TimeoutAction timeoutAction)
        {
            AssignTimeout(ticks, timeoutAction);
        }

        public void ResetTimeout(long ticks, TimeoutAction timeoutAction)
        {
            AssignTimeout(ticks, timeoutAction);
        }

        public void CancelTimeout()
        {
        }

        private void AssignTimeout(long ticks, TimeoutAction timeoutAction)
        {
        }

        public void StartTimingReads()
        {
        }

        public void StopTimingReads()
        {
        }

        public void PauseTimingReads()
        {
        }

        public void ResumeTimingReads()
        {
        }

        public void BytesRead(long count)
        {
        }

        public void StartTimingWrite(long size)
        {
        }

        public void StopTimingWrite()
        {
        }

        private enum ProtocolSelectionState
        {
            Initializing,
            Selected,
            Stopping,
            Stopped
        }
    }
}
