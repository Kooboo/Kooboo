//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DefaultValue : Attribute
    {
        public string Value { get; set; }

        public DefaultValue(string input)
        {
            Value = input;
        }
    }
}