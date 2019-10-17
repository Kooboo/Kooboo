using SassAndCoffee.Core;

namespace SassAndCoffee.JavaScript.TypeScript
{
    public class TypeScriptCompiler : JavaScriptCompilerBase
    {
        public override string CompilerLibraryResourceName => "typescript.js";

        public override string CompilationFunctionName => "compilify_ts";

        public TypeScriptCompiler(IInstanceProvider<IJavaScriptRuntime> jsRuntimeProvider)
            : base(jsRuntimeProvider) { }
    }
}