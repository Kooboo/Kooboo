namespace SassAndCoffee.JavaScript.Uglify
{
    using SassAndCoffee.Core;
    using System;

    public class UglifyCompilerContentTransform : JavaScriptCompilerContentTransformBase
    {
        public const string StateKey = "Uglify";

        public override string InputMimeType => "text/javascript";

        public override string OutputMimeType => "text/javascript";

        public override void PreExecute(ContentTransformState state)
        {
            if (state.Path.EndsWith(".min.js", StringComparison.OrdinalIgnoreCase))
            {
                state.Items.Add(StateKey, true);
                var newPath = state.Path
                    .ToLowerInvariant()
                    .Replace(".min.js", ".js");
                state.RemapPath(newPath);
            }
            base.PreExecute(state);
        }

        public override void Execute(ContentTransformState state)
        {
            if (!state.Items.ContainsKey(StateKey))
                return;

            base.Execute(state);
        }

        protected override IInstanceProvider<IJavaScriptCompiler> CreateCompilerProvider(
            IInstanceProvider<IJavaScriptRuntime> jsRuntimeProvider)
        {
            return new InstanceProvider<IJavaScriptCompiler>(
                () => new UglifyCompiler(jsRuntimeProvider));
        }
    }
}