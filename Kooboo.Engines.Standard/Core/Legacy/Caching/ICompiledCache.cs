//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core.Caching
{
    using System;

    // TODO: Document me
    public interface ICompiledCache
    {
        object GetOrAdd(string filename, Func<string, object> compilationDelegate, string mimeType);

        void Clear();
    }
}
