
using System;
using System.Collections.Concurrent;

namespace Kooboo.Mail.Smtp
{
    public class SmtpConnectionManager
    {
        private readonly ConcurrentDictionary<long, SmtpConnectionReference> _connectionReferences = new ConcurrentDictionary<long, SmtpConnectionReference>();

        public SmtpConnectionManager(long? maxConnetions)
        {
            ConnectionCount = GetCounter(maxConnetions);
        }

        /// <summary>
        /// Connections that have been switched to a different protocol.
        /// </summary>
        public ResourceCounter ConnectionCount { get; set;  }

        public void AddConnection(SmtpConnector connection)
        {
            if (!_connectionReferences.TryAdd(connection.Id, new SmtpConnectionReference(connection)))
            {
                throw new ArgumentException(nameof(connection));
            }
        }

        public void RemoveConnection(long id)
        {
            if (!_connectionReferences.TryRemove(id, out _))
            {
                throw new ArgumentException(nameof(id));
            }
        }

        public void Walk(Action<SmtpConnector> callback)
        {
            foreach (var kvp in _connectionReferences)
            {
                var reference = kvp.Value;

                if (reference.TryGetConnection(out var connection))
                {
                    callback(connection);
                }
                else if (_connectionReferences.TryRemove(kvp.Key, out reference))
                {
                    // It's safe to modify the ConcurrentDictionary in the foreach.
                    // The connection reference has become unrooted because the application never completed.
                    //_trace.ApplicationNeverCompleted(reference.ConnectionId);
                }

                // If both conditions are false, the connection was removed during the heartbeat.
            }
        }

        private static ResourceCounter GetCounter(long? number)
            => number.HasValue
                ? ResourceCounter.Quota(number.Value)
                : ResourceCounter.Unlimited;
    }
}
