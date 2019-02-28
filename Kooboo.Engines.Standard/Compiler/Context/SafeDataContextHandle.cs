using System;
using System.Runtime.ConstrainedExecution;
using LibSass.Compiler.Options;
using static LibSass.Compiler.SassExterns;

namespace LibSass.Compiler.Context
{
    internal sealed class SassSafeDataContextHandle : SassSafeContextHandle
    {
        internal SassSafeDataContextHandle(ISassOptions sassOptions) :
            base(sassOptions, sass_make_data_context(EncodeAsUtf8IntPtr(sassOptions.Data)))
        { }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            sass_delete_data_context(handle);
            return true;
        }

        protected override SassResult CompileInternalContext()
        {
            sass_compile_data_context(this);
            return GetResult();
        }

        protected override void SetOverriddenOptions(IntPtr sassOptionsInternal, ISassOptions sassOptions)
        {
            if (!string.IsNullOrWhiteSpace(sassOptions.InputPath))
                sass_option_set_input_path(sassOptionsInternal, new SassSafeStringHandle(sassOptions.InputPath));
        }
    }
}
