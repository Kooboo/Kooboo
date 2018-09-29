using System;
using LibSass.Compiler.Context;
using static LibSass.Compiler.Context.SassSafeContextHandle;
using static LibSass.Compiler.SassExterns;

namespace LibSass.Types
{
    public class SassError : ISassType, ISassExportableType
    {
        private IntPtr _cachedPtr;
        public string Message { get; set; }

        public SassError(string message)
        {
            Message = message;
        }

        internal SassError(IntPtr rawPointer)
        {
            IntPtr rawValue = sass_error_get_message(rawPointer);
            Message = PtrToString(rawValue);
        }

        IntPtr ISassExportableType.GetInternalTypePtr(InternalPtrValidityEventHandler validityEventHandler, bool dontCache)
        {
            if (_cachedPtr != default(IntPtr))
                return _cachedPtr;

            var value = sass_make_error(new SassSafeStringHandle(Message));

            if (dontCache)
                return value;

            validityEventHandler += (this as ISassExportableType).OnInvalidated;

            return _cachedPtr = value;
        }

        void ISassExportableType.OnInvalidated()
        {
            _cachedPtr = default(IntPtr);
        }
    }
}
