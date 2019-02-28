//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript {
    using SassAndCoffee.Core;

    public class JavaScriptCompilerProxy : ProxyBase<IJavaScriptCompiler>, IJavaScriptCompiler {

        public JavaScriptCompilerProxy() { }

        public JavaScriptCompilerProxy(IJavaScriptCompiler compiler)
            : base(compiler) { }

        public string Compile(string source, params object[] args) {
            return WrappedItem.Compile(source, args);
        }
    }
}

