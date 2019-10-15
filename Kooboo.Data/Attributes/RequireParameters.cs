//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Attributes
{
    public class RequireParameters : Attribute
    {
        public List<string> Parameters;

        public RequireParameters(params string[] paras)
        {
            this.Parameters = new List<string>();
            foreach (var item in paras)
            {
                Parameters.Add(item);
            }
        }
    }
}