using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint.Native;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render.Functions
{
    public class kScriptFunction : IFunction
    {
        public string Name => "kScript";

        public string FunctionName { get; set; }

        public List<IFunction> Parameters { get; set; }

        public object Render(RenderContext context)
        {
            var engine = Scripting.Manager.GetJsEngine(context);

            var prog = engine.GetValue(FunctionName);
             
            if (prog != null && prog != Jint.Native.JsValue.Undefined)
            {
                var paras = FunctionHelper.RenderParameter(context, this.Parameters);
                  
                var result = engine.Invoke(prog, paras.ToArray()); 

                if (result != null && result.IsNull()==false)
                { 
                    if (result.IsArray())
                    {
                        var items = result.AsArray();

                        var arrayResult = new List<object>();

                        var ownproperties = items.GetOwnProperties().Where(o=>o.Key!= "length").ToList(); 

                        foreach (Jint.Native.JsValue item in ownproperties.Select(p => p.Value.Value))
                        {
                            arrayResult.Add(item);
                        } 
                        return arrayResult; 
                    }
                    return result.ToString(); 
                }
            }
            return null; 
        }
    }
}
