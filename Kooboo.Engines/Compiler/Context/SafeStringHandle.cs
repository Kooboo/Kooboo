using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace LibSass.Compiler.Context
{
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    internal sealed class SassSafeStringHandle : SafeHandle
    {
        internal SassSafeStringHandle(string optionValue) :
              base(IntPtr.Zero, true)
        {
            string encodedValue = SassSafeContextHandle.EncodeAsUtf8String(optionValue);
            handle = Marshal.StringToCoTaskMemAnsi(encodedValue);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            Marshal.FreeCoTaskMem(handle);
            return true;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
    }
}
