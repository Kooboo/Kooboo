//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Engine;

namespace Kooboo.Engines
{
    public class Sass:IEngine
    {
        public string Name { get { return "sass"; } }

        public bool KeepTag { get { return true; } }

        public string Extension => "sass";

        public bool IsScript => false;

        public bool IsStyle => true;

        // Less Css..   
        public string Execute(RenderContext context, string input)
        {
            if (Kooboo.Lib.Helper.RuntimeSystemHelper.IsWindow())
            {
                var sassOptions = new LibSass.Compiler.Options.SassOptions
                {
                    Data = input,
                    IsIndentedSyntax = true //sass
                };
                var sass = new LibSass.Compiler.SassCompiler(sassOptions);
                var result = sass.Compile();

                return result.Output;
            }
            if (Kooboo.Lib.Helper.RuntimeSystemHelper.IsLinux())
            {
                return SassHelper.Compile(input,true);
            }
            //mac don't support
            return string.Empty;
            
        }
    }
}
