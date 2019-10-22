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

        public Heartbeat(SmtpConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public void Start()
        {
            _timer = new Timer(OnHeartbeat, state: this, dueTime: Interval, period: Interval);
        }

        private static void OnHeartbeat(object state)
        {
            ((Heartbeat)state).OnHeartbeat();
        }

        // Called by the Timer (background) thread
        internal void OnHeartbeat()
        {
            var now = DateTime.UtcNow;

            if (Interlocked.Exchange(ref _executingOnHeartbeat, 1) == 0)
            {
                try
                {
                    _connectionManager.Walk(o => o.CheckTimeout(now));
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
                Kooboo.Data.Log.Instance.Exception.Write(DateTime.UtcNow.ToString() + " SMTP heartbeat slow " + now.ToString());
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
