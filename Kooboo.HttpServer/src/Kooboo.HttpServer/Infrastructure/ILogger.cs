using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Kooboo.HttpServer.Http.Http2;
//using Kooboo.HttpServer.Http.Http2.HPack;

namespace Kooboo.HttpServer
{
    public interface ILogger
    {
        void ConnectionReadFin(string connectionId);

        void ConnectionWriteFin(string connectionId);

        void ConnectionError(string connectionId, Exception ex);

        void ConnectionReset(string connectionId);

        void ConnectionStart(string connectionId);

        void ConnectionStop(string connectionId);

        void ConnectionPause(string connectionId);

        void ConnectionResume(string connectionId);

        void ConnectionRejected(string connectionId);

        void ConnectionKeepAlive(string connectionId);

        void ConnectionDisconnect(string connectionId);

        void RequestProcessingError(string connectionId, Exception ex);

        void ConnectionHeadResponseBodyWrite(string connectionId, long count);

        void NotAllConnectionsClosedGracefully();

        void ConnectionBadRequest(string connectionId, BadHttpRequestException ex);

        void ApplicationError(string connectionId, string traceIdentifier, Exception ex);

        void NotAllConnectionsAborted();

        void HeartbeatSlow(TimeSpan interval, DateTimeOffset now);

        void ApplicationNeverCompleted(string connectionId);

        void RequestBodyStart(string connectionId, string traceIdentifier);

        void RequestBodyDone(string connectionId, string traceIdentifier);

        void RequestBodyMininumDataRateNotSatisfied(string connectionId, string traceIdentifier, double rate);

        void ResponseMininumDataRateNotSatisfied(string connectionId, string traceIdentifier);

        //void Http2ConnectionError(string connectionId, Http2ConnectionErrorException ex);

        //void Http2StreamError(string connectionId, Http2StreamErrorException ex);

        //void HPackDecodingError(string connectionId, int streamId, HPackDecodingException ex);

        void LogError(string message, Exception ex);

        void LogError(int eventId, Exception exception, string message, params object[] args);

        void LogTrace(string message, params object[] parameters);

        bool IsEnabled(LogLevel level);

    }

    public enum LogLevel
    {
        Trace = 0,

        Debug = 1,

        Information = 2,

        Warning = 3,

        Error = 4,

        Critical = 5,

        None = 6
    }
}
