using Kooboo.Data.Context;
using Kooboo.Data.Server;
using Kooboo.Sites.Scripting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Frontend.KScriptDefine
{
    public class KScriptDefineMiddleware : IKoobooMiddleWare
    {
        private readonly Lazy<string> _defineContent;

        public KScriptDefineMiddleware()
        {
            _defineContent = new Lazy<string>(() =>
            {
                return new KScriptToTsDefineConventer().Convent(typeof(k), ExtensionContainer.List);
            }, true);
        }

        public IKoobooMiddleWare Next
        {
            get; set;
        }

        public async Task Invoke(RenderContext context)
        {
            if (context.Request.Path.ToLower() == "/_admin/scripts/components/manacoservice/kscript.d.ts")
            {
                context.Response.AppendString(_defineContent.Value);
                return;
            }

            await Next.Invoke(context);
        }
    }
}
