//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic; 
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render.Functions
{
    public class DateFormatFunction : IFunction
    {
        public string Name
        {
            get
            {
                return "DateFormat"; 
            }
        }

        public List<IFunction> Parameters
        {
            get;set;
        }  

        public object Render(RenderContext context)
        {
            var paras = FunctionHelper.RenderParameter(context, this.Parameters); 
            string formatestring = paras[1].ToString();
            var datetiem = Convert.ToDateTime(paras[0]); 
             
            return datetiem.ToString(formatestring);  
        }
    }

    public class FormatDateFunction : IFunction
    {
        public string Name
        {
            get
            {
                return "FormatDate";
            }
        }

        public List<IFunction> Parameters
        {
            get; set;
        }

        public object Render(RenderContext context)
        {
            var paras = FunctionHelper.RenderParameter(context, this.Parameters);
            string formatestring = paras[1].ToString();
            var datetiem = Convert.ToDateTime(paras[0]);

            return datetiem.ToString(formatestring);
        }
    }
}
