//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.TAL.Functions;

namespace Kooboo.TAL.Extended
{
 public   class JoinStringFunction : IFunction
    {
        public string Name
        {
            get { return "join"; }
        }

        public string Description
        {
            get {  return "combine and return the string"; }
        }

        public object Execute(params object[] paras)
        {
            string returnstring = string.Empty;

            foreach (var item in paras)
            {
                returnstring += item.ToString();   
            } 
            return returnstring; 
        }
    }
}
