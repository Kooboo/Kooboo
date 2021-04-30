using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using System.Collections.Generic;
using System.Linq;

namespace KScript.Api
{
    public static class Helpers
    {
        public static void CheckRequired(string[] properties, IDictionary<string, object>[] metas)
        {
            var requiredNames = metas.Where(w => w.ContainsKey("name")
                                                && w.ContainsKey("required")
                                                && true.Equals(w["required"])
                                           )
                                     .Select(s => s["name"]);

            var required = requiredNames.Except(properties).FirstOrDefault();
            if (required != null) throw new RequiredException(new[] { required.ToString() });
        }

        public static IDictionary<string, object>[] NamedMetas(IDictionary<string, object>[] metas)
        {
            return metas?.Where(w => w.ContainsKey("name"))?.ToArray();
        }

        public static object FormToObject(RenderContext context)
        {
            var result = new Dictionary<string, object>();

            foreach (var item in context.Request.Forms.AllKeys)
            {
                result[item] = context.Request.Forms.Get(item);
            }

            return result;
        }

        public static string ToJson(object obj)
        {
            if (obj == null) return null;
            return JsonHelper.SerializeCaseSensitive(obj, new IntJsonConvert());
        }
    }
}
