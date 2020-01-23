using System;
using System.Threading;

namespace Kooboo.Mail.Smtp
{
    public class Heartbeat : IDisposable
    {
        public static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

        private Timer _timer;
        private int _executingOnHeartbeat;
        private SmtpConnectionManager _connectionManager;
        private Action<SmtpConnector> _walk;
        private DateTime _utcNow;

        public Heartbeat(SmtpConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _walk = Walk;
        }

        public void Start()
        {
            _timer = new Timer(OnHeartbeat, state: this, dueTime: Interval, period: Interval);
        }

        public DateTime UtcNow => _utcNow;

        private static void OnHeartbeat(object state)
        {
            ((Heartbeat)state).OnHeartbeat();
        }

        // Called by the Timer (background) thread
        internal void OnHeartbeat()
        {
            var utcNow = DateTime.UtcNow;
            if (Interlocked.Exchange(ref _executingOnHeartbeat, 1) == 0)
            {
                _utcNow = utcNow;
                try
                {
                    _connectionManager.Walk(_walk);
                }
                catch (Exception ex)
                {
                    Kooboo.Data.Log.Instance.Exception.Write(DateTime.UtcNow.ToString() + " " + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.Source);
                }
                finally
                {
                    Interlocked.Exchange(ref _executingOnHeartbeat, 0);
                }
            }
            else
            {
                Kooboo.Data.Log.Instance.Exception.Write(DateTime.UtcNow.ToString() + " SMTP heartbeat slow " + utcNow.ToString());
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void Walk(SmtpConnector connection)
        {
            connection.CheckTimeout();
        }
    }
}
