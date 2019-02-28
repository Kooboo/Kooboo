//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript {
    using SassAndCoffee.Core;

    public abstract class JavaScriptCompilerContentTransformBase : ContentTransformBase {

        private IInstanceProvider<IJavaScriptRuntime> _jsRuntimeProvider;
        private IInstanceProvider<IJavaScriptCompiler> _jsCompilerProvider;

        public abstract string InputMimeType { get; }
        public abstract string OutputMimeType { get; }

        public JavaScriptCompilerContentTransformBase() {
            _jsRuntimeProvider = new InstanceProvider<IJavaScriptRuntime>(
                () => new IEJavaScriptRuntime());
            _jsCompilerProvider = CreateCompilerProvider(_jsRuntimeProvider);
        }

        public override void Execute(ContentTransformState state) {
            Execute(state);
        }

        protected virtual void Execute(ContentTransformState state, params object[] args) {
            // If input is empty or the wrong type, do nothing
            if (state.Content == null || state.MimeType != InputMimeType)
                return;

            string result = null;
            using (var compiler = _jsCompilerProvider.GetInstance()) {
                result = compiler.Compile(state.Content, args);
            }

            if (result != null) {
                state.ReplaceContent(new ContentResult() {
                    Content = result,
                    MimeType = OutputMimeType,
                });
            }
        }


        protected abstract IInstanceProvider<IJavaScriptCompiler> CreateCompilerProvider(
            IInstanceProvider<IJavaScriptRuntime> jsRuntimeProvider);
    }
}
