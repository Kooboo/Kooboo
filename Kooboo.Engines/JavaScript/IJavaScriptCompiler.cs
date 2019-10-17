namespace SassAndCoffee.JavaScript
{
    using System;

    public interface IJavaScriptCompiler : IDisposable
    {
        string Compile(string source, params object[] args);
    }
}