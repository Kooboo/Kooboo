//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Interface
{
    public interface IDynamic
    {
        Object GetValue(string FieldName);

        Object GetValue(string FieldName, RenderContext Context);

        void SetValue(string FieldName, Object Value);

        Dictionary<string, object> Values { get; }
    }
}
