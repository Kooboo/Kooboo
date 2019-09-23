using Kooboo.Data.Ensurance.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Ensurance
{
    public static class HttpEurance
    {

        public static async Task<string> GetAndEnsureBool(string url, Dictionary<string, string> parameters)
        {
            try
            {
                var result = await Kooboo.Lib.Helper.HttpHelper.GetStringAsync(url, parameters);
                return result;
            }
            catch (Exception ex)
            {
                EnsureManager.Add(new HttpGetBool() { FullUrl = url, Parameters = parameters });
            }

            return null;
        }


        public static void AddEnsureBool(string url, Dictionary<string, string> parameters=null)
        { 
            EnsureManager.Add(new HttpGetBool() { FullUrl = url, Parameters = parameters }); 
        }
    }
}
