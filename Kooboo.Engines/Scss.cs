using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Sites.Engine;
using LibSass.Compiler.Options;
using LibSass.Compiler;


namespace Kooboo.Engines
{
    public class Scss:IEngine
    {
        public string Name { get { return "scss"; } }

        public bool KeepTag { get { return true; } }

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
