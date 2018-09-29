//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.TAL.Functions;
using System.Linq;

namespace Kooboo.TAL.Extended
{
    class FormatDateFunction : IFunction
    {
        public string Name
        {
            get { return "formatdate"; }
        }

        public string Description
        {
            get { return "accept one format string and one datetime, and return the formatted string"; }
        }

        public object Execute(params object[] paras)
        { 
            List<object> list = paras.ToList();

            string format = list[0].ToString();
            list.RemoveAt(0);
            return  Convert.ToDateTime(list[0]).ToString(format);
        }
    }
}
