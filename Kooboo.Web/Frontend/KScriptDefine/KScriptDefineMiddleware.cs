using Kooboo.Data.Context;
using Kooboo.Data.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Frontend.KScriptDefine
{
    public class KScriptDefineMiddleware : IKoobooMiddleWare
    {
        private readonly string _defineContent;

        public KScriptDefineMiddleware()
        {
            _defineContent = new KScriptToTsDefineConventer().Convent(typeof(KScript));
        }
        public IKoobooMiddleWare Next
        {
            get; set;
        }

        public async Task Invoke(RenderContext context)
        {
            if (context.Request.Path.ToLower() == "/_admin/scripts/components/manacoservice/kscript.d.ts")
            {
                context.Response.AppendString(_defineContent);
                return;
            }

            await Next.Invoke(context);
        }
    }
}
