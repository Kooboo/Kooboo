//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using SassAndCoffee.JavaScript.ActiveScript;
    using SassAndCoffee.JavaScript.JavaScriptEngines;

    public class IEJavaScriptRuntime : BaseActiveScriptSite, IJavaScriptRuntime, IDisposable {
        private IActiveScript _jsEngine;
        private IActiveScriptParseWrapper _jsParse;
        private object _jsDispatch;
        private Type _jsDispatchType;

        private Dictionary<string, object> _siteItems = new Dictionary<string, object>();

        private const string JavaScriptProgId = "JScript";
        public static bool IsSupported {
            get {
                return Type.GetTypeFromProgID(JavaScriptProgId) != null;
            }
        }

        public void Initialize() {
            try {
                // Prefer Chakra
                _jsEngine = new ChakraJavaScriptEngine() as IActiveScript;
            } catch {
                // TODO: Make catch more specific
                _jsEngine = null;
            }

            if (_jsEngine == null) {
                // No need to catch here - engine of last resort
                _jsEngine = new JavaScriptEngine() as IActiveScript;
            }

            _jsEngine.SetScriptSite(this);
            _jsParse = new ActiveScriptParseWrapper(_jsEngine);
            _jsParse.InitNew();
        }

        public void LoadLibrary(string libraryCode) {
            try {
                _jsParse.ParseScriptText(libraryCode, null, null, null, IntPtr.Zero, 0, ScriptTextFlags.IsVisible);
            } catch {
                var last = GetAndResetLastException();
                if (last != null)
                    throw last;
                else throw;
            }
            // Check for parse error
            var parseError = GetAndResetLastException();
            if (parseError != null)
                throw parseError;

            UpdateDispatch();
        }

        public T ExecuteFunction<T>(string functionName, params object[] args) {
            T result;
            try {
                result = (T)_jsDispatchType.InvokeMember(functionName, BindingFlags.InvokeMethod, null, _jsDispatch, args);
            } catch {
                ThrowError();
                throw;
            }

            ThrowError();

            // TODO: This is a hack, but I'm not sure how else to test for invalid statements
            //if (result == "this;")
            //    throw new ArgumentException(string.Format("{0}('{1}'); is not valid JavaScript.", function, input));

            return result;
        }

        public dynamic AsDynamic() {
            return _jsDispatch;
        }

        private void UpdateDispatch() {
            ComRelease(ref _jsDispatch);
            _jsEngine.GetScriptDispatch(null, out _jsDispatch);
            _jsDispatchType = _jsDispatch.GetType();
        }

        private void ThrowError() {
            var last = GetAndResetLastException();
            if (last != null)
                throw last;
        }

        public override object GetItem(string name) {
            lock (_siteItems) {
                object result = null;
                return _siteItems.TryGetValue(name, out result) ? result : null;
            }
        }

        public override IntPtr GetTypeInfo(string name) {
            lock (_siteItems) {
                if (!_siteItems.ContainsKey(name))
                    return IntPtr.Zero;
                return Marshal.GetITypeInfoForType(_siteItems[name].GetType());
            }
        }

        ~IEJavaScriptRuntime() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing) {
            ComRelease(ref _jsDispatch, !disposing);

            // For now these next two actually reference the same object, but it doesn't hurt to be explicit.
            ComRelease(ref _jsParse, !disposing);
            ComRelease(ref _jsEngine, !disposing);
        }

        private void ComRelease<T>(ref T o, bool final = false)
            where T : class {
            if (o != null && Marshal.IsComObject(o)) {
                if (final)
                    Marshal.FinalReleaseComObject(o);
                else
                    Marshal.ReleaseComObject(o);
            }
            o = null;
        }
    }
}
