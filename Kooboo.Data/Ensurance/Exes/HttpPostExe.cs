using Kooboo.Data.Ensurance.Model;

namespace Kooboo.Data.Ensurance.Exes
{
    public class HttpPostExe : IExecutor<HttpPost>
    {
        public bool Execute(string jsonModel)
        {
            var item = Lib.Helper.JsonHelper.Deserialize<HttpPost>(jsonModel);

            return Kooboo.Lib.Helper.HttpHelper.PostData(item.FullUrl, null, System.Text.Encoding.UTF8.GetBytes(item.Json));
        }
    }
}