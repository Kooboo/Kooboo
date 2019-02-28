//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Engine;
using Kooboo.Data.Context;
using SassAndCoffee.Core;
using SassAndCoffee.JavaScript;
using SassAndCoffee.JavaScript.TypeScript;


namespace Kooboo.Engines
{
    public class TypeScript: IEngine
    {
        public string Name { get { return "typpescript"; } }

        public bool KeepTag { get { return true; } }

        public string Extension => "ts";

        public bool IsScript => true;

        public bool IsStyle => false;

        public string Execute(RenderContext context, string input)
        {
            var jsRuntimeProvider = new InstanceProvider<IJavaScriptRuntime>(
                () => new IEJavaScriptRuntime());


            var instance = new InstanceProvider<IJavaScriptCompiler>(
               () => new TypeScriptCompiler(jsRuntimeProvider));
            return instance.GetInstance().Compile(input);

        }
    }
}
