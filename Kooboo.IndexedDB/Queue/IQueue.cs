using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Queue
{
    public interface IQueue
    {
        void Close();

        void DelSelf();
    }
}
