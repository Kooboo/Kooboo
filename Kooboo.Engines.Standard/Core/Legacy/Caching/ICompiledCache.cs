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
