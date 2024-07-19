//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Reflection;

namespace Kooboo.Api.Methods
{
    public interface IDynamicApi
    {
        DynamicApi GetMethod(string name);
    }

    public class DynamicApi
    {
        public Type Type { get; set; }

        public MethodInfo Method { get; set; }
    }
}
