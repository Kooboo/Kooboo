using SassAndCoffee.Core;
using System.Text.RegularExpressions;

namespace SassAndCoffee.JavaScript.TypeScript
{
    public class TypeScriptCompilerContentTransform : JavaScriptCompilerContentTransformBase
    {
        public const string StateKey = "TypeScript_Bare";

        public static readonly Regex BareModeDetection = new Regex(
            @"\.bare(\.min)?\.js$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public override string InputMimeType => "text/typescript";

        public override string OutputMimeType => "text/javascript";

        public override void PreExecute(ContentTransformState state)
        {
            if (BareModeDetection.IsMatch(state.Path))
            {
                state.Items.Add(StateKey, true);
                var newPath = state.Path
                    .ToLowerInvariant()
                    .Replace(".bare.js", ".js")
                    .Replace(".bare.min.js", ".min.js");
                state.RemapPath(newPath);
            }
            base.PreExecute(state);
        }

        public override void Execute(ContentTransformState state)
        {
            bool bare = state.Items.ContainsKey(StateKey);

            base.Execute(state, bare);
        }

        protected override IInstanceProvider<IJavaScriptCompiler> CreateCompilerProvider(
            IInstanceProvider<IJavaScriptRuntime> jsRuntimeProvider)
        {
            return new InstanceProvider<IJavaScriptCompiler>(
                () => new TypeScriptCompiler(jsRuntimeProvider));
        }
    }
}