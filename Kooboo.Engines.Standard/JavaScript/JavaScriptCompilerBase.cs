//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript {
    using System;
    using SassAndCoffee.Core;

    public abstract class JavaScriptCompilerBase : IJavaScriptCompiler {

        public abstract string CompilerLibraryResourceName { get; }
        public abstract string CompilationFunctionName { get; }

        private IInstanceProvider<IJavaScriptRuntime> _jsRuntimeProvider;
        private IJavaScriptRuntime _js;
        private bool _initialized = false;
        private object _lock = new object();

        public JavaScriptCompilerBase(IInstanceProvider<IJavaScriptRuntime> jsRuntimeProvider) {
            _jsRuntimeProvider = jsRuntimeProvider;
        }

        public string Compile(string source, params object[] args) {
            if (source == null)
                throw new ArgumentException("source cannot be null.", "source");

            object[] compileArgs = null;
            if (args != null && args.Length > 0) {
                compileArgs = new object[args.Length + 1];
                compileArgs[0] = source;
                args.CopyTo(compileArgs, 1);
            } else {
                compileArgs = new object[] { source };
            }

            lock (_lock) {
                Initialize();
                return _js.ExecuteFunction<string>(CompilationFunctionName, compileArgs);
            }
        }

        private void Initialize() {
            if (!_initialized) {
                _js = _jsRuntimeProvider.GetInstance();
                _js.Initialize();
                _js.LoadLibrary(Utility.ResourceAsString(CompilerLibraryResourceName, this.GetType()));
                _initialized = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_js != null) {
                    _js.Dispose();
                    _js = null;
                }
            }
        }
    }
}
