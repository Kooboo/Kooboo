//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using SassAndCoffee.Core;

namespace SassAndCoffee.JavaScript.TypeScript 
{

    public class TypeScriptCompiler : JavaScriptCompilerBase {
        public override string CompilerLibraryResourceName {
            get { return "typescript.js"; }
        }

        public override string CompilationFunctionName {
            get { return "compilify_ts"; }
        }

        public TypeScriptCompiler(IInstanceProvider<IJavaScriptRuntime> jsRuntimeProvider)
            : base(jsRuntimeProvider) { }
    }
}
