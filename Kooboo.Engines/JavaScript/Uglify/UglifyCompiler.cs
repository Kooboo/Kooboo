namespace SassAndCoffee.JavaScript.Uglify
{
    using SassAndCoffee.Core;

    public class UglifyCompiler : JavaScriptCompilerBase
    {
        public override string CompilerLibraryResourceName => "uglify.js";

        public override string CompilationFunctionName => "compilify_ujs";

        public UglifyCompiler(IInstanceProvider<IJavaScriptRuntime> jsRuntimeProvider)
            : base(jsRuntimeProvider) { }
    }
}