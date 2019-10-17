using Kooboo.Data.Context;
using Kooboo.Sites.Engine;
using LibSass.Compiler;
using LibSass.Compiler.Options;

namespace Kooboo.Engines
{
    public class Sass : IEngine
    {
        public string Name => "sass";

        public bool KeepTag => true;

        public string Extension => "sass";

        public bool IsScript => false;

        public bool IsStyle => true;

        // Less Css..
        public string Execute(RenderContext context, string input)
        {
            var sassOptions = new SassOptions
            {
                Data = input,
                IsIndentedSyntax = true //sass
            };
            var sass = new SassCompiler(sassOptions);
            var result = sass.Compile();

            return result.Output;
        }
    }
}