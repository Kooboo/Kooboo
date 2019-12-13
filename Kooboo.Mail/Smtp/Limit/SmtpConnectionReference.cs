using System;

namespace Kooboo.Mail.Smtp
{
    public class SmtpConnectionReference
    {
        private readonly WeakReference<SmtpConnector> _weakReference;

        public SmtpConnectionReference(SmtpConnector connection)
        {
            _weakReference = new WeakReference<SmtpConnector>(connection);
            ConnectionId = connection.Id;
        }

        public long ConnectionId { get; }

        public bool TryGetConnection(out SmtpConnector connection)
        {
            return _weakReference.TryGetTarget(out connection);
        }
    }
}
