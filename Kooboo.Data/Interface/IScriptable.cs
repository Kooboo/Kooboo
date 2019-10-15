//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Data.Interface
{
    // make sure that all IScriptable also implement ITextObject.
    public interface IScriptable
    {
        // the k.request.paraname;
        List<string> RequestParas { get; set; }
    }
}