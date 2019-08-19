using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipelines;
using System.Threading;

namespace Kooboo.HttpServer.Http
{
    public class HttpConnection : ITimeoutControl
    {
        private readonly object _protocolSelectionLock = new object();
        private ProtocolSelectionState _protocolSelectionState = ProtocolSelectionState.Initializing;
        private IRequestProcessor _requestProcessor;
        private Http1Connection _http1Connection;

        private long _lastTimestamp;
        private long _timeoutTimestamp = long.MaxValue;
        private TimeoutAction _timeoutAction;

        private readonly object _readTimingLock = new object();
        private bool _readTimingEnabled;
        private bool _readTimingPauseRequested;
        private long _readTimingElapsedTicks;
        private long _readTimingBytesRead;

        private readonly object _writeTimingLock = new object();
        private int _writeTimingWrites;
        private long _writeTimingTimeoutTimestamp;

        private Task _lifetimeTask;

        internal bool RequestTimedOut { get; private set; }

        public string ConnectionId => Context.ConnectionId;

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
            try
            {
                IRequestProcessor requestProcessor = null;
                Context.ServiceContext.ConnectionManager.AddConnection(Context.Features.Connection.HttpConnectionId, this);
                _lastTimestamp = Context.ServiceContext.SystemClock.UtcNow.Ticks;

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
                        _http1Connection = new Http1Connection(new Http1ConnectionContext
                        {
                            TimeoutControl = this,
                            Context = Context,
                            ToTransport = new PipeConnection(Context.ReceivePipe.Reader, Context.SendPipe.Writer),
                            ToApplication = new PipeConnection(Context.SendPipe.Reader, Context.ReceivePipe.Writer),
                        });
                        //_http1Connection = requestProcessor;
                        requestProcessor = _http1Connection;
                        _protocolSelectionState = ProtocolSelectionState.Selected;

                        _requestProcessor = requestProcessor;
                    }
                }

                if (requestProcessor != null)
                {
                    await requestProcessor.ProcessRequestsAsync(Context.ServiceContext.Application);
                }
            }
            catch(Exception ex)
            {
                Context.ServiceContext.Log.LogError(ex.Message, ex);
            }
            finally
            {
                Context.ServiceContext.ConnectionManager.RemoveConnection(Context.Features.Connection.HttpConnectionId);
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

        public void Tick(DateTimeOffset now)
        {
            var timestamp = now.Ticks;

            CheckForTimeout(timestamp);

            // HTTP/2 rate timeouts are not yet supported.
            if (_http1Connection != null)
            {
                CheckForReadDataRateTimeout(timestamp);
                CheckForWriteDataRateTimeout(timestamp);
            }

            Interlocked.Exchange(ref _lastTimestamp, timestamp);
        }


        private void CheckForTimeout(long timestamp)
        {
            // TODO: Use PlatformApis.VolatileRead equivalent again
            if (timestamp > Interlocked.Read(ref _timeoutTimestamp))
            {
                CancelTimeout();

                switch (_timeoutAction)
                {
                    case TimeoutAction.StopProcessingNextRequest:
                        // Http/2 keep-alive timeouts are not yet supported.
                        _http1Connection?.StopProcessingNextRequest();
                        break;
                    case TimeoutAction.SendTimeoutResponse:
                        // HTTP/2 timeout responses are not yet supported.
                        if (_http1Connection != null)
                        {
                            RequestTimedOut = true;
                            _http1Connection.SendTimeoutResponse();
                        }
                        break;
                    case TimeoutAction.AbortConnection:
                        // This is actually supported with HTTP/2!
                        Abort(new TimeoutException());
                        break;
                }
            }
        }

        private void CheckForReadDataRateTimeout(long timestamp)
        {
            
            // The only time when both a timeout is set and the read data rate could be enforced is
            // when draining the request body. Since there's already a (short) timeout set for draining,
            // it's safe to not check the data rate at this point.
            if (Interlocked.Read(ref _timeoutTimestamp) != long.MaxValue)
            {
                return;
            }

            lock (_readTimingLock)
            {
                if (_readTimingEnabled)
                {
                    // Reference in local var to avoid torn reads in case the min rate is changed via IHttpMinRequestBodyDataRateFeature
                    var minRequestBodyDataRate = _http1Connection.MinRequestBodyDataRate;

                    _readTimingElapsedTicks += timestamp - _lastTimestamp;

                    if (minRequestBodyDataRate?.BytesPerSecond > 0 && _readTimingElapsedTicks > minRequestBodyDataRate.GracePeriod.Ticks)
                    {
                        var elapsedSeconds = (double)_readTimingElapsedTicks / TimeSpan.TicksPerSecond;
                        var rate = Interlocked.Read(ref _readTimingBytesRead) / elapsedSeconds;

                        if (rate < minRequestBodyDataRate.BytesPerSecond )
                        {
                            RequestTimedOut = true;
                            _http1Connection.SendTimeoutResponse();
                        }
                    }

                    // PauseTimingReads() cannot just set _timingReads to false. It needs to go through at least one tick
                    // before pausing, otherwise _readTimingElapsed might never be updated if PauseTimingReads() is always
                    // called before the next tick.
                    if (_readTimingPauseRequested)
                    {
                        _readTimingEnabled = false;
                        _readTimingPauseRequested = false;
                    }
                }
            }
        }

        private void CheckForWriteDataRateTimeout(long timestamp)
        {

            lock (_writeTimingLock)
            {
                if (_writeTimingWrites > 0 && timestamp > _writeTimingTimeoutTimestamp)
                {
                    RequestTimedOut = true;
                    Abort(new TimeoutException());
                }
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
            Interlocked.Exchange(ref _timeoutTimestamp, long.MaxValue);
        }

        private void AssignTimeout(long ticks, TimeoutAction timeoutAction)
        {
            _timeoutAction = timeoutAction;

            // Add Heartbeat.Interval since this can be called right before the next heartbeat.
            Interlocked.Exchange(ref _timeoutTimestamp, _lastTimestamp + ticks + Heartbeat.Interval.Ticks);
        }

        public void StartTimingReads()
        {
            lock (_readTimingLock)
            {
                _readTimingElapsedTicks = 0;
                _readTimingBytesRead = 0;
                _readTimingEnabled = true;
            }
        }

        public void StopTimingReads()
        {
            lock (_readTimingLock)
            {
                _readTimingEnabled = false;
            }
        }

        public void PauseTimingReads()
        {
            lock (_readTimingLock)
            {
                _readTimingPauseRequested = true;
            }
        }

        public void ResumeTimingReads()
        {
            lock (_readTimingLock)
            {
                _readTimingEnabled = true;

                // In case pause and resume were both called between ticks
                _readTimingPauseRequested = false;
            }
        }

        public void BytesRead(long count)
        {
            Interlocked.Add(ref _readTimingBytesRead, count);
        }

        public void StartTimingWrite(long size)
        {

            lock (_writeTimingLock)
            {
                var minResponseDataRate = _http1Connection.MinResponseDataRate;

                if (minResponseDataRate != null)
                {
                    var timeoutTicks = Math.Max(
                        minResponseDataRate.GracePeriod.Ticks,
                        TimeSpan.FromSeconds(size / minResponseDataRate.BytesPerSecond).Ticks);

                    if (_writeTimingWrites == 0)
                    {
                        // Add Heartbeat.Interval since this can be called right before the next heartbeat.
                        _writeTimingTimeoutTimestamp = _lastTimestamp + Heartbeat.Interval.Ticks;
                    }

                    _writeTimingTimeoutTimestamp += timeoutTicks;
                    _writeTimingWrites++;
                }
            }
        }

        public void StopTimingWrite()
        {
            lock (_writeTimingLock)
            {
                _writeTimingWrites--;
            }
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
