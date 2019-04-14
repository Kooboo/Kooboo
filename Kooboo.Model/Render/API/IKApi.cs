using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Api;

namespace Kooboo.Model.Render.API
{
    public interface IKApi
    {
        bool isMatch(string api);

        ApiModel Get(string api,IApiProvider provider);
   
    }
}
