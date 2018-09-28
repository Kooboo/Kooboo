using Kooboo.Data.Context;
using SassAndCoffee.JavaScript.CoffeeScript;

namespace Kooboo.Sites.Engine
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
            return CoffeeScriptCompiler.Instance.Compile(input);
        }
    }
}
