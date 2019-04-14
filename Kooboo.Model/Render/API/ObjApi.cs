using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Api;

namespace Kooboo.Model.Render.API
{
    public class ObjApi : IKApi
    {
        public ApiModel Get(string api,IApiProvider provider)
        {
            var model= Kooboo.Lib.Helper.JsonHelper.Deserialize<ApiModel>(api);
            if (model != null)
            {
                model.Provider = provider;
            }
            return model;
        }

        public bool isMatch(string api)
        {
            return api.IndexOf("}")>-1;
        }
    }
}
