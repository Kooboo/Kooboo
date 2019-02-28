using System.Runtime.ConstrainedExecution;
using LibSass.Compiler.Options;
using static LibSass.Compiler.SassExterns;

namespace LibSass.Compiler.Context
{
    internal sealed class SassSafeFileContextHandle : SassSafeContextHandle
    {
        internal SassSafeFileContextHandle(ISassOptions sassOptions) :
            base(sassOptions, sass_make_file_context(new SassSafeStringHandle(sassOptions.InputPath)))
        { }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            sass_delete_file_context(handle);
            return true;
        }

        protected override SassResult CompileInternalContext()
        {
            sass_compile_file_context(this);
            return GetResult();
        }
    }
}
