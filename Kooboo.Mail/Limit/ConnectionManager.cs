
using System;
using System.Collections.Concurrent;

namespace Kooboo.Mail
{
    public class ConnectionManager
    {
        private readonly ConcurrentDictionary<long, ConnectionReference> _connectionReferences = new ConcurrentDictionary<long, ConnectionReference>();

        public ConnectionManager(long? maxConnetions)
        {
            ConnectionCount = GetCounter(maxConnetions);
        }

        /// <summary>
        /// Connections that have been switched to a different protocol.
        /// </summary>
        public ResourceCounter ConnectionCount { get; set;  }

        public void AddConnection(IManagedConnection connection)
        {
            if (!_connectionReferences.TryAdd(connection.Id, new ConnectionReference(connection)))
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

        public void Walk(Action<IManagedConnection> callback)
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
