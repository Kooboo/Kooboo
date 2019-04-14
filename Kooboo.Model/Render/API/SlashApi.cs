using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Api;

namespace Kooboo.Model.Render.API
{
    public class SlashApi : IKApi
    {
        public ApiModel Get(string api,IApiProvider provider)
        {
            var parts = api.Split('/').Where(a=>!string.IsNullOrEmpty(a)).ToArray();

            var model = new ApiModel() {Provider=provider };
            if (parts.Length >= 2)
            {
                model.Obj = parts[0];
                model.Api = parts[1];

                if (parts.Length > 2)
                {
                    model.Method = parts[2];
                }
            }

            return model;
        }

        public bool isMatch(string api)
        {
            return api.IndexOf("/") > -1;
        }
    }
}
