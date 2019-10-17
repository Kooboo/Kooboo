using Kooboo.Data.Context;
using Kooboo.Sites.Engine;
using LibSass.Compiler;
using LibSass.Compiler.Options;

namespace Kooboo.Engines
{
    public class Scss : IEngine
    {
        public string Name => "scss";

        public bool KeepTag => true;

        public string Extension => "scss";

        public bool IsScript => false;

        public bool IsStyle => true;

        public string Execute(RenderContext context, string input)
        {
            var sassOptions = new SassOptions
            {
                Data = input
            };
            var sass = new SassCompiler(sassOptions);
            var result = sass.Compile();

            return result.Output;
        }
    }
}