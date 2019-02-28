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
    public class GetValueFunction : IFunction
    {
        public GetValueFunction()
        {

        }
        public GetValueFunction(string keyorexpression)
        {
            this.Key = keyorexpression; 
        }
        public string Name
        {
            get
            {
                return "GetValue"; 
            }
        }

        public string Key { get; set; }

        public List<IFunction> Parameters
        {
            get;set;
        }

        public object Render(RenderContext context)
        {
            return context.DataContext.GetValue(this.Key); 
        }
    }
}
