//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript.Uglify {
    using SassAndCoffee.Core;

    public class UglifyCompiler : JavaScriptCompilerBase {
        public override string CompilerLibraryResourceName {
            get { return "uglify.js"; }
        }

        public override string CompilationFunctionName {
            get { return "compilify_ujs"; }
        }

        public UglifyCompiler(IInstanceProvider<IJavaScriptRuntime> jsRuntimeProvider)
            : base(jsRuntimeProvider) { }
    }
}