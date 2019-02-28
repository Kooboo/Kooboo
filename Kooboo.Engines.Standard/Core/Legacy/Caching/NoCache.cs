//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core.Caching
{
    using System;

    public class NoCache : ICompiledCache
    {
        public object GetOrAdd(string filename, Func<string, object> compilationDelegate, string mimeType)
        {
            return compilationDelegate(filename);
        }

        public void Clear()
        {
        }
    }
}