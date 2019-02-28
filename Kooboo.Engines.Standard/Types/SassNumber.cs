using System;
using LibSass.Compiler.Context;
using static LibSass.Compiler.Context.SassSafeContextHandle;
using static LibSass.Compiler.SassExterns;

namespace LibSass.Types
{
    public class SassNumber : ISassType, ISassExportableType
    {
        private IntPtr _cachedPtr;
        public double Value { get; set; }

        /// <summary>
        /// size units:
        /// In, Cm, Pc, Mm, Pt, Px,
        ///
        /// angle units
        /// Deg, Grad, Rad, Turn,
        ///
        /// time units 
        /// Sec, Msec,
        ///
        /// frequency units 
        /// Hertz, Khertz,
        ///
        /// resolutions units 
        /// Dpi, Dpcm, Dppx
        /// </summary>
        public string Unit { get; set; }

        public SassNumber(double value)
        {
            Value = value;
        }

        public SassNumber(double value, string unit)
        {
            Value = value;
            Unit = unit;
        }

        internal SassNumber(IntPtr rawPointer)
        {
            Value = sass_number_get_value(rawPointer);
            Unit = PtrToString(sass_number_get_unit(rawPointer));
        }

        public override string ToString()
        {
            return $"{Value}{Unit}";
        }

        IntPtr ISassExportableType.GetInternalTypePtr(InternalPtrValidityEventHandler validityEventHandler, bool dontCache)
        {
            if (_cachedPtr != default(IntPtr))
                return _cachedPtr;

            var value = sass_make_number(Value, new SassSafeStringHandle(Unit));

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
