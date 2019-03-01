//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Sites.Engine;


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
            if (Kooboo.Lib.Helper.RuntimeSystemHelper.IsWindow())
            {
                var sassOptions = new LibSass.Compiler.Options.SassOptions
                {
                    Data = input
                };
                var sass = new LibSass.Compiler.SassCompiler(sassOptions);
                var result = sass.Compile();

                return result.Output;
            }
            if (Kooboo.Lib.Helper.RuntimeSystemHelper.IsLinux())
            {
                return SassHelper.Compile(input);
            }
            //mac template don't support
            return string.Empty;
            
        }
    }
}
