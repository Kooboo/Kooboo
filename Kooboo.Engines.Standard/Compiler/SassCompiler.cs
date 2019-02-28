using System;
using System.Runtime.InteropServices;
using LibSass.Compiler.Context;
using LibSass.Compiler.Options;

namespace LibSass.Compiler
{
    public struct VersionInfo
    {
        public string LibSassNetVersion { get; set; }
        public string LibSassVersion { get; internal set; }
        public string SassLanguageVersion { get; internal set; }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr SassImporterDelegate(IntPtr currrentPath, IntPtr callback, IntPtr compiler);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate IntPtr SassFunctionDelegate(IntPtr currrentPath, IntPtr callback, IntPtr compiler);

    public class SassCompiler
    {
        public static readonly VersionInfo SassInfo;

        static SassCompiler()
        {
            SassInfo = new VersionInfo
            {
                LibSassNetVersion = typeof(SassCompiler).Assembly.GetName().Version.ToString(),
                LibSassVersion = SassSafeContextHandle.LibsassVersion(),
                SassLanguageVersion = SassSafeContextHandle.SassLanguageVersion()
            };
        }

        private readonly SassSafeContextHandle _internalContext;

        /// <summary>
        /// Provides an instance of LibSass wrapper class.
        /// </summary>
        /// <param name="sassOptions">Sass options object for compilation.</param>
        /// <remarks>
        /// Replicates LibSass behaviour for context resolution, that is;
        /// if data is provided, make data context and set input file as
        /// supplementary option. Otherwise make a file context.
        /// </remarks>
        public SassCompiler(ISassOptions sassOptions)
        {
            if (string.IsNullOrEmpty(sassOptions.Data))
            {
                _internalContext = new SassSafeFileContextHandle(sassOptions);
            }
            else
            {
                _internalContext = new SassSafeDataContextHandle(sassOptions);
            }

            _internalContext.SetOptions(sassOptions);
        }

        public SassResult Compile()
        {
            return _internalContext.CompileContext();
        }
    }
}
