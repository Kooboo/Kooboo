//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render.Functions
{
    public class JoinFunction : IFunction
    {
        public string Name
        {
            get
            {
                return "Join"; 
            }
        }

        public List<IFunction> Parameters
        {
            get;set;
        }

        public object Render(RenderContext context)
        {
            var paras = FunctionHelper.RenderParameter(context, this.Parameters);

            string result = string.Empty;

            foreach (var item in paras)
            {
               if (item !=null)
                { result += item.ToString();  }
            }
            return result; 
        }
    }
}
