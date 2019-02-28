using System;
using LibSass.Compiler.Context;
using static LibSass.Compiler.SassExterns;

namespace LibSass.Types
{
    public class SassColor : ISassType, ISassExportableType
    {
        private IntPtr _cachedPtr;

        public double Red { get; set; }
        public double Green { get; set; }
        public double Blue { get; set; }
        public double Alpha { get; set; }

        public SassColor()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
            Alpha = 1;
        }

        public SassColor(double red, double green, double blue, double alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        internal SassColor(IntPtr rawPointer)
        {
            Red = sass_color_get_r(rawPointer);
            Green = sass_color_get_g(rawPointer);
            Blue = sass_color_get_b(rawPointer);
            Alpha = sass_color_get_a(rawPointer);
        }

        public override string ToString()
        {
            double red = Math.Min(Math.Max(Red, 0), 255);
            double green = Math.Min(Math.Max(Green, 0), 255);
            double blue = Math.Min(Math.Max(Blue, 0), 255);
            double alpha = Math.Min(Math.Max(Alpha, 0), 1.0);

            return $"rgba({red},{green},{blue},{alpha})";
        }

        IntPtr ISassExportableType.GetInternalTypePtr(InternalPtrValidityEventHandler validityEventHandler, bool dontCache)
        {
            if (_cachedPtr != default(IntPtr))
                return _cachedPtr;

            var value = sass_make_color(Red, Green, Blue, Alpha); ;

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
