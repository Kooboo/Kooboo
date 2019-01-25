//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using dotless.Core;

namespace Kooboo.Sites.Engine
{
    public class Less : IEngine
    {
        public string Name { get { return "less"; } }

        public bool KeepTag { get { return true;  } }

        public string Extension => "less";

        public bool IsScript => false;

        public bool IsStyle => true;

        // Less Css..   
        public string Execute(RenderContext context, string input)
        {
            return LessEngine.Instance.Parse(input);
        }
    }
}
