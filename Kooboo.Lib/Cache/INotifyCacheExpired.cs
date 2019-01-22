using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Cache
{
    public interface INotifyCacheExpired
    {
        void Notify(string objectCacheName, string key);
    }
}
