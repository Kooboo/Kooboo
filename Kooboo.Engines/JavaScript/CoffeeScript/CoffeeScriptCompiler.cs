namespace SassAndCoffee.JavaScript.CoffeeScript
{
    using SassAndCoffee.Core;

    public class CoffeeScriptCompiler : JavaScriptCompilerBase
    {
        public override string CompilerLibraryResourceName => "coffee-script.js";

        public override string CompilationFunctionName => "compilify_cs";

        public CoffeeScriptCompiler(IInstanceProvider<IJavaScriptRuntime> jsRuntimeProvider)
            : base(jsRuntimeProvider) { }
    }
}