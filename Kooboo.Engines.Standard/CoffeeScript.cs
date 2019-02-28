//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Engine;
using Kooboo.Data.Context;
using SassAndCoffee.Core;
using SassAndCoffee.JavaScript;
using SassAndCoffee.JavaScript.CoffeeScript;

namespace Kooboo.Engines
{
    public class CoffeeScript : IEngine
    {
        public string Name { get { return "coffeescript"; } }

        public bool KeepTag { get { return true; } }

        public string Extension => "coffee";

        public bool IsScript => true;

        public bool IsStyle => false;

        // Execute coffee script and combine into regular js..
        public string Execute(RenderContext context, string input)
        {
            var jsRuntimeProvider = new InstanceProvider<IJavaScriptRuntime>(
                () => new IEJavaScriptRuntime());

            
            var instance= new InstanceProvider<IJavaScriptCompiler>(
               () => new CoffeeScriptCompiler(jsRuntimeProvider));
            return instance.GetInstance().Compile(input);
            
        }
    }
}
