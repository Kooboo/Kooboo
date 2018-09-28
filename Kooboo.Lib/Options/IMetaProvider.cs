using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo
{
    public interface IMetaProvider<TMeta>
    {
        TMeta GetMeta();
    }
}
