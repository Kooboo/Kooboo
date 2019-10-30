using Kooboo.Data.Context;
using System.Collections.Generic;

namespace Kooboo.Sites.Render.Functions
{
    public class kScriptFunction : IFunction
    {
        public string Name => "kScript";

        public string FunctionName { get; set; }

        public List<IFunction> Parameters { get; set; }

        // TODO, improve performance.
        public object Render(RenderContext context)
        {
            var engine = Scripting.Manager.GetJsEngine(context);

            var prog = engine.GetValue(FunctionName);

            if (prog != null)
            {
                var paras = FunctionHelper.RenderParameter(context, this.Parameters);

                var result = engine.Invoke(prog, paras.ToArray());

                if (result != null)
                {
                    return result.ToString();
                }
            }
            return null;
        }
    }
}