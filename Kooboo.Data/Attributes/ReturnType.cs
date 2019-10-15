//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ReturnType : Attribute
    {
        public Type Type { get; set; }

        public ReturnType(Type returnType)
        {
            this.Type = returnType;
        }
    }
}