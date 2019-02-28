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