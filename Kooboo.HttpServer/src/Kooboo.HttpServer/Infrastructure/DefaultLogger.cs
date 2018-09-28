using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Kooboo.HttpServer.Http.Http2;
//using Kooboo.HttpServer.Http.Http2.HPack;

namespace Kooboo.HttpServer
{
    public class DefaultLogger : ILogger
    {
        public void ApplicationError(string connectionId, string traceIdentifier, Exception ex)
        {
            
        }

        public void ApplicationNeverCompleted(string connectionId)
        {
            
        }

        public void ConnectionBadRequest(string connectionId, BadHttpRequestException ex)
        {
            
        }

        public void ConnectionDisconnect(string connectionId)
        {
            
        }

        public void ConnectionError(string connectionId, Exception ex)
        {
            
        }

        public void ConnectionHeadResponseBodyWrite(string connectionId, long count)
        {
            
        }

        public void ConnectionKeepAlive(string connectionId)
        {
            
        }

        public void ConnectionPause(string connectionId)
        {
            
        }

        public void ConnectionReadFin(string connectionId)
        {
            
        }

        public void ConnectionRejected(string connectionId)
        {
            
        }

        public void ConnectionReset(string connectionId)
        {
            
        }

        public void ConnectionResume(string connectionId)
        {
            
        }

        public void ConnectionStart(string connectionId)
        {
            
        }

        public void ConnectionStop(string connectionId)
        {
            
        }

        public void ConnectionWriteFin(string connectionId)
        {
            
        }

        public void HeartbeatSlow(TimeSpan interval, DateTimeOffset now)
        {
            
        }

        //public void HPackDecodingError(string connectionId, int streamId, HPackDecodingException ex)
        //{
            
        //}

        //public void Http2ConnectionError(string connectionId, Http2ConnectionErrorException ex)
        //{
            
        //}

        //public void Http2StreamError(string connectionId, Http2StreamErrorException ex)
        //{
            
        //}

        public void LogError(string message, Exception ex)
        {
            Console.WriteLine(message);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        public void LogError(int eventId, Exception exception, string message, params object[] args)
        {

        }

        public void LogTrace(string message, params object[] parameters)
        {
            
        }

        public void NotAllConnectionsAborted()
        {
            
        }

        public void NotAllConnectionsClosedGracefully()
        {
            
        }

        public void RequestBodyDone(string connectionId, string traceIdentifier)
        {
            
        }

        public void RequestBodyMininumDataRateNotSatisfied(string connectionId, string traceIdentifier, double rate)
        {
            
        }

        public void RequestBodyStart(string connectionId, string traceIdentifier)
        {
            
        }

        public void RequestProcessingError(string connectionId, Exception ex)
        {
            
        }

        public void ResponseMininumDataRateNotSatisfied(string connectionId, string traceIdentifier)
        {
            
        }

        public bool IsEnabled(LogLevel level)
        {
            return false;
        }
    }
}
