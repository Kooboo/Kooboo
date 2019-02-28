using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using LibSass.Compiler.Options;

namespace LibSass.Compiler.Context
{
    internal delegate void InternalPtrValidityEventHandler();

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    internal abstract partial class SassSafeContextHandle : SafeHandle
    {
        private readonly Dictionary<IntPtr, CustomImportDelegate> _importersCallbackDictionary = new Dictionary<IntPtr, CustomImportDelegate>();
        private readonly Dictionary<IntPtr, CustomFunctionDelegate> _functionsCallbackDictionary = new Dictionary<IntPtr, CustomFunctionDelegate>();
        internal event InternalPtrValidityEventHandler ValidityEvent = () => { };
        private readonly ISassOptions _sassOptions;

        internal SassSafeContextHandle(ISassOptions sassOptions, IntPtr method) :
            base(IntPtr.Zero, true)
        {
            handle = method;
            _sassOptions = sassOptions;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public SassResult CompileContext()
        {
            ValidityEvent?.Invoke();
            return CompileInternalContext();
        }

        protected abstract SassResult CompileInternalContext();

        public static string LibsassVersion()
        {
            return PtrToString(SassExterns.libsass_version());
        }
        public static string SassLanguageVersion()
        {
            return PtrToString(SassExterns.libsass_language_version());
        }

        protected SassResult GetResult()
        {
            return new SassResult
            {
                Output = PtrToString(SassExterns.sass_context_get_output_string(this)),
                ErrorColumn = SassExterns.sass_context_get_error_column(this),
                ErrorFile = PtrToString(SassExterns.sass_context_get_error_file(this)),
                ErrorJson = PtrToString(SassExterns.sass_context_get_error_json(this)),
                ErrorLine = SassExterns.sass_context_get_error_line(this),
                ErrorMessage = PtrToString(SassExterns.sass_context_get_error_message(this)),
                //ErrorSource = PtrToString(sass_context_get_error_src(this)),
                ErrorStatus = SassExterns.sass_context_get_error_status(this),
                ErrorText = PtrToString(SassExterns.sass_context_get_error_text(this)),
                SourceMap = PtrToString(SassExterns.sass_context_get_source_map_string(this)),
                IncludedFiles = PtrToStringArray(SassExterns.sass_context_get_included_files(this))
            };
        }
    }
}
