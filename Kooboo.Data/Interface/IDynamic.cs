//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Interface
{
    public interface IDynamic
    {
        Object GetValue(string fieldName);

        Object GetValue(string fieldName, RenderContext context);

        void SetValue(string fieldName, Object Value);

        Dictionary<string, object> Values { get; }
    }
}