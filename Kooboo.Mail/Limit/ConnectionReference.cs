using System;

namespace Kooboo.Mail
{
    public class ConnectionReference
    {
        private readonly WeakReference<IManagedConnection> _weakReference;

        public ConnectionReference(IManagedConnection connection)
        {
            _weakReference = new WeakReference<IManagedConnection>(connection);
            ConnectionId = connection.Id;
        }

        public long ConnectionId { get; }

        public bool TryGetConnection(out IManagedConnection connection)
        {
            return _weakReference.TryGetTarget(out connection);
        }
    }
}
