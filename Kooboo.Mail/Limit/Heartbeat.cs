using System;
using System.Collections.Generic;
using System.Threading;

namespace Kooboo.Mail
{
    public class Heartbeat : IDisposable
    {
        public static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

        private Timer _timer;
        private int _executingOnHeartbeat;
        private List<ConnectionManager> _connectionManagers;
        private Action<IManagedConnection> _walk;
        private DateTime _utcNow;
        private bool _started;

        public static Heartbeat Instance = new Heartbeat();

        public Heartbeat()
        {
            _connectionManagers = new List<ConnectionManager>();
            _walk = Walk;
        }

        public void Add(ConnectionManager connectionManager)
        {
            _connectionManagers.Add(connectionManager);
        }

        public void Start()
        {
            if (_started)
                throw new InvalidOperationException("Heartbeat can only be started one time");

            _started = true;
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
                    foreach (var each in _connectionManagers)
                    {
                        each.Walk(_walk);
                    }
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
                Kooboo.Data.Log.Instance.Exception.Write("SMTP heartbeat slow " + utcNow.ToString());
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void Walk(IManagedConnection connection)
        {
            connection.CheckTimeout();
        }
    }
}
