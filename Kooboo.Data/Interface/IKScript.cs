//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Data.Interface
{
    public interface IkScript
    {
        string Name { get; }

        RenderContext Context { get; set; }
    }
}