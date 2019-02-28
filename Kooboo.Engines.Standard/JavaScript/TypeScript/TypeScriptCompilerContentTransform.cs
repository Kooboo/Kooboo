//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Text.RegularExpressions;
using SassAndCoffee.Core;
using SassAndCoffee.JavaScript;

namespace SassAndCoffee.JavaScript.TypeScript
{

    public class TypeScriptCompilerContentTransform : JavaScriptCompilerContentTransformBase
    {
        public const string StateKey = "TypeScript_Bare";

        public static readonly Regex BareModeDetection = new Regex(
            @"\.bare(\.min)?\.js$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public override string InputMimeType {
            get { return "text/typescript"; }
        }

        public override string OutputMimeType {
            get { return "text/javascript"; }
        }

        public override void PreExecute(ContentTransformState state) {
            if (BareModeDetection.IsMatch(state.Path)) {
                state.Items.Add(StateKey, true);
                var newPath = state.Path
                    .ToLowerInvariant()
                    .Replace(".bare.js", ".js")
                    .Replace(".bare.min.js", ".min.js");
                state.RemapPath(newPath);
            }
            base.PreExecute(state);
        }

        public override void Execute(ContentTransformState state) {
            bool bare = false;
            if (state.Items.ContainsKey(StateKey))
                bare = true;

            base.Execute(state, bare);
        }

        protected override IInstanceProvider<IJavaScriptCompiler> CreateCompilerProvider(
            IInstanceProvider<IJavaScriptRuntime> jsRuntimeProvider) {
            return new InstanceProvider<IJavaScriptCompiler>(
                () => new TypeScriptCompiler(jsRuntimeProvider));
        }
    }
}
