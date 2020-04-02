using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Mail
{
    public interface IManagedConnection
    {
        long Id { get; set; }

        void CheckTimeout();
    }
}
