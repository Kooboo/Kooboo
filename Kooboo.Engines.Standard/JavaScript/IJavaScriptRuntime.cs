//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript {
    using System;

    public interface IJavaScriptRuntime : IDisposable {
        void Initialize();
        void LoadLibrary(string libraryCode);
        //Task LoadLibraryAsync(string libraryCode);
        T ExecuteFunction<T>(string functionName, params object[] args);
        //Task<T> ExecuteFunctionAsync<T>(string functionName, params object[] args);
        //T ExecuteStatement<T>(string statement);
        //Task<T> ExecuteStatementAsync<T>(string statement);
        dynamic AsDynamic();
    }
}
