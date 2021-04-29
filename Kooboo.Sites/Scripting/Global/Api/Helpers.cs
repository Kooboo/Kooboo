using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Api
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
    }
}
