using Kooboo.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine
{
    public static class KScriptGenerater
    {
        public static void Generate()
        {
            var file = new KScriptToTsDefineConventer().Convent(typeof(KScript));
            var path = Path.Combine(AppSettings.RootPath, "_Admin\\Scripts\\components\\manacoService\\", "kscript.d.ts");
            File.WriteAllText(path, file);
        }
    }
}
