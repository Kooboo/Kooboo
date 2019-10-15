using Kooboo.Data.Ensurance.Model;

namespace Kooboo.Data.Ensurance.Exes
{
    public class HttpGetBoolExe : IExecutor<HttpGetBool>
    {
        public bool Execute(string jsonModel)
        {
            var item = Lib.Helper.JsonHelper.Deserialize<HttpGetBool>(jsonModel);

            return Kooboo.Lib.Helper.HttpHelper.Get<bool>(item.FullUrl, item.Parameters);
        }
    }
}