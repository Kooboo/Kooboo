using Kooboo.Data.Ensurance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Ensurance.Exes
{  
    public class HttpPostExe : IExecutor<HttpPost>
    {
        public bool Execute(string JsonModel)
        {
            var item = Lib.Helper.JsonHelper.Deserialize<HttpPost>(JsonModel);

            return Kooboo.Lib.Helper.HttpHelper.PostData(item.FullUrl, null,  System.Text.Encoding.UTF8.GetBytes(item.Json));
        }
    }
     
}
