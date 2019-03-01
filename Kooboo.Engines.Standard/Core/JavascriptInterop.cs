//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using Jurassic;

    using V8Bridge.Interface;

    class JSWorkItem
    {
        readonly internal ManualResetEventSlim _gate = new ManualResetEventSlim();

        public string Input { get; internal set; }
        public string Func { get; internal set; }
        public string Result { get; internal set; }

        public JSWorkItem(string func, string input)
        {
            Func = func;
            Input = input;
        }

        public string GetValueSync()
        {
            _gate.Wait();
            return Result;
        }
    }


    /* XXX: Why this crazy code is here:
     * 
     * After many, many painful debugging sessions, I am forced to conclude that 
     * the V8 JavaScript engine can only be used on one thread. Not only does that
     * mean that accesses to V8 must be synchronous and only one running at a time,
     * once you touch V8 from a thread, you must use *that thread* forever. i.e. it
     * is not only Thread-Unsafe, but also Thread Affinitized.
     */

    public class JavascriptBasedCompiler : IDisposable
    {
        static ConcurrentQueue<JSWorkItem> _workQueue = new ConcurrentQueue<JSWorkItem>();
        static readonly Thread _dispatcherThread;
        string _compileFuncName;
        static bool _shouldQuit;

        static JavascriptBasedCompiler()
        {
            _dispatcherThread = new Thread(() => {
                var engine = JS.CreateJavascriptCompiler();

                while(!_shouldQuit) {
                    if (_workQueue == null) {
                        break;
                    }

                    JSWorkItem item;
                    if (!_workQueue.TryDequeue(out item)) {
                        Thread.Sleep(100);

                        continue;
                    }

                    try {
                        if (item.Func == null) {
                            engine.InitializeLibrary(item.Input);
                            item.Result = "";
                        } else {
                            item.Result = engine.Compile(item.Func, item.Input);
                        }
                    } catch (Exception ex) {
                        // Note: You absolutely cannot let any exceptions bubble up, as it kills the app domain.

                        item.Result = String.Format("ENGINE FAULT - please report this if it happens frequently: {0}: {1}\n{2}", ex.GetType(), ex.Message, ex.StackTrace);

                        JS.V8FailureReason = ex;
                        if (Environment.Is64BitProcess == false) {
                            engine = new JurassicCompiler();
                        }                        
                    }

                    item._gate.Set();
                }
            });

            _dispatcherThread.Start();
        }
        
        internal static void shutdownJSThread()
        {
            _shouldQuit = true;
            _dispatcherThread.Join(TimeSpan.FromSeconds(10));
        }

        public JavascriptBasedCompiler(string resource, string compileFuncName)
        {
            _compileFuncName = compileFuncName;
            var workItem = new JSWorkItem(null, Utility.ResourceAsString(resource));

            _workQueue.Enqueue(workItem);
            workItem.GetValueSync();
        }

        public string Compile(string coffeeScriptCode)
        {
            var ret = new JSWorkItem(_compileFuncName, coffeeScriptCode);
            _workQueue.Enqueue(ret);
            return ret.GetValueSync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _workQueue = null;
        }
    }

    public class JurassicCompiler : IV8ScriptCompiler
    {
        ScriptEngine _engine;
        object _gate = 42;

        public JurassicCompiler()
        {
            _engine = new ScriptEngine();
        }

        public void InitializeLibrary(string libraryCode)
        {
            lock (_gate) {
                var t = new Thread(() => {
                    _engine.Execute(libraryCode);
                }, 10 * 1048576);
    
                t.Start();
                t.Join();
            }
        }

        public string Compile(string func, string code)
        {
            return _engine.CallGlobalFunction<string>(func, code);
        }
    }

#if FALSE
    public class IronJSCompiler : IV8ScriptCompiler
    {
        CSharp.Context _engine;
        Dictionary<string, Func<string, string>> _compilerFunc = new Dictionary<string, Func<string, string>>();

        public void InitializeLibrary(string libraryCode)
        {
            _engine = new CSharp.Context();
            _engine.Execute(libraryCode);
        }

        public string Compile(string function, string input)
        {
            lock(_engine) {
                if (!_compilerFunc.ContainsKey(function)) {
                    _compilerFunc[function] = _engine.GetFunctionAs<Func<string, string>>(function);
                }

                return _compilerFunc[function](input);
            }
        }
    }
#endif

    public static class JS
    {
        static Lazy<Type> _scriptCompilerImpl;
        static object _gate = 42;

        /* XXX: Why this crazy code is here
         *
         * V8 is a managed C++ DLL, which means that it must be 
         * architecture-specific, since it contains native code. However, our host
         * is *not* architecture specific, it is AnyCPU. To facilitate this, while
         * still following one of the S&C's core policies of "Don't Make The User
         * Think!", we actually embed V8Bridge.dll as a resource, then at runtime,
         * load it based on our actual architecture. *
         * 
         * If this can't be done (like if we're on ARM or something weird), we fall
         * back to the all-managed yet incredibly slow Jurassic engine */

        public static Exception V8FailureReason;
        static JS()
        {
            _scriptCompilerImpl = new Lazy<Type>(() => {
                if (InternetExplorerJavaScriptCompiler.IsSupported) {
                    return typeof(InternetExplorerJavaScriptCompiler);
                }

                string suffix = (Environment.Is64BitProcess ? "amd64" : "x86");
                string assemblyResource = (Environment.Is64BitProcess ?
                    "SassAndCoffee.Core.lib.amd64.V8Bridge.dll" : "SassAndCoffee.Core.lib.x86.V8Bridge.dll");

                var attemptedPaths = new[] {
                    Path.Combine(Path.GetTempPath(), String.Format("V8Bridge_{0}.dll", suffix)),
                    Path.Combine(Path.GetFullPath(@".\App_Data"), String.Format("V8Bridge_{0}.dll", suffix)),
                    Path.Combine(Path.GetFullPath("."), String.Format("V8Bridge_{0}.dll", suffix)),
                };

                string succeededPath = null;
                foreach(var path in attemptedPaths) {
                    try {
                        using (var of = File.OpenWrite(path)) {
                            Assembly.GetExecutingAssembly().GetManifestResourceStream(assemblyResource).CopyTo(of);
                        }

                        succeededPath = path;
                        break;
                    } catch (IOException) {
                    } catch (UnauthorizedAccessException) {
                    }
                }

                Assembly v8Assembly;
                try {
                    v8Assembly = Assembly.LoadFile(succeededPath);
                } catch (Exception ex) {
                    V8FailureReason = ex;

                    Console.Error.WriteLine("*** WARNING: You're on ARM, Mono, Itanium (heaven help you), or another architecture\n" +
                        "which isn't x86/amd64 on NT. Loading the Jurassic compiler, which is much slower.");

                    // Jurassic completely bites it on 64-bit, we just need to abort
                    if (Environment.Is64BitProcess == true) {
                        throw;
                    }

                    return typeof (JurassicCompiler);
                }

                return v8Assembly.GetTypes().FirstOrDefault(x => typeof (IV8ScriptCompiler).IsAssignableFrom(x)) ??
                    typeof (JurassicCompiler);
            }, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public static IV8ScriptCompiler CreateJavascriptCompiler()
        {
            lock(_gate) {
                return Activator.CreateInstance(_scriptCompilerImpl.Value) as IV8ScriptCompiler;
            }
        }

        public static void Shutdown()
        {
            JavascriptBasedCompiler.shutdownJSThread();
        }
    }
}
