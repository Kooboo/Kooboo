//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Engine;
using Kooboo.Data.Context;
using SassAndCoffee.Core;
using SassAndCoffee.JavaScript;
using SassAndCoffee.JavaScript.ActiveScript;

namespace Kooboo.Engines
{
    public class ActiveScript:IEngine
    {
        public string Name { get { return "activescript"; } }

        public bool KeepTag { get { return true; } }

        public string Extension => "as";

        public bool IsScript => true;

        public bool IsStyle => false;

        public string Execute(RenderContext context, string input)
        {
            var wrapper = new ActiveScriptParseWrapper();

            var jsRuntimeProvider = new InstanceProvider<IJavaScriptRuntime>(
                () => new IEJavaScriptRuntime());


            var instance = new InstanceProvider<IJavaScriptCompiler>(
               () => new (jsRuntimeProvider));
            return instance.GetInstance().Compile(input);

        }
    }
}
