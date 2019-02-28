//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript {
    using System;

    public interface IJavaScriptCompiler : IDisposable {
        string Compile(string source, params object[] args);
    }
}
