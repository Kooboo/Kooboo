using System;
using LibSass.Compiler.Context;
using static LibSass.Compiler.Context.SassSafeContextHandle;
using static LibSass.Compiler.SassExterns;

namespace LibSass.Types
{
    public class SassString : ISassType, ISassExportableType
    {
        private IntPtr _cachedPtr;
        public string Value { get; set; }

        public SassString(string value)
        {
            Value = value;
        }

        internal SassString(IntPtr rawPointer)
        {
            var rawValue = sass_string_get_value(rawPointer);
            Value = PtrToString(rawValue);
        }

        public override string ToString()
        {
            return Value;
        }

        IntPtr ISassExportableType.GetInternalTypePtr(InternalPtrValidityEventHandler validityEventHandler, bool dontCache)
        {
            if (_cachedPtr != default(IntPtr))
                return _cachedPtr;

            var value = sass_make_string(new SassSafeStringHandle(EncodeAsUtf8String(Value)));

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
