using Kooboo.Data.Ensurance.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Ensurance.Exes
{
    
    public class HttpGetExe : IExecutor<HttpGet>
    {
        public bool Execute(string JsonModel)
        {
            var item = Lib.Helper.JsonHelper.Deserialize<HttpGet>(JsonModel);

            return Kooboo.Lib.Helper.HttpHelper.Get<bool>(item.FullUrl, item.Parameters);
        }
    } 

}
