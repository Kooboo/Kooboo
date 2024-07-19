//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Permission;

namespace Kooboo.Api
{
    public class ApiMethod
    {
        public Action<object, object[]> Void;
        public Func<object, object[], object> Func;

        public bool IsVoid { get; set; }
        public Type DeclareType { get; set; }
        public Type ReturnType { get; set; }
        public string MethodName { get; set; }
        public Type RequireModelType { get; set; }

        public List<string> RequireParas { get; set; }

        public Object ClassInstance { get; set; }

        public string AliasOf { get; set; }

        public Action<object, ApiCall> SetCall { get; set; }

        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        public List<PermissionAttribute> Permissions { get; set; }
    }

    public class Parameter
    {
        public string Name { get; set; }

        public Type ClrType { get; set; }

    }

}
