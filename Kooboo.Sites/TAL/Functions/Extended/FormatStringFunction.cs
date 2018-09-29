//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.TAL.Functions;

namespace Kooboo.TAL.Extended
{
   public class FormatStringFunction : IFunction
    {
        public string Name
        {
            get { return "formatstring"; }
        }

        public string Description
        {
            get
            {
                return "formate a string according to the formating syntax";
            }
        }

        public object Execute(params object[] paras)
        {
            List<object> list = paras.ToList();

            string formate = list[0].ToString();
            list.RemoveAt(0);
            return string.Format(formate, list.ToArray());
        }
 
   }
}
