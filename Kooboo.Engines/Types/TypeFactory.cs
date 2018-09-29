using System;
using LibSass.Compiler.Context;
using static LibSass.Compiler.SassExterns;

namespace LibSass.Types
{
    internal enum SassTag
    {
        SassBoolean,
        SassNumber,
        SassColor,
        SassString,
        SassList,
        SassMap,
        SassNull,
        SassError,
        SassWarning
    }

    internal class TypeFactory
    {
        internal static ISassType[] GetSassArguments(IntPtr rawPointer)
        {
            var length = sass_list_get_length(rawPointer);
            var values = new ISassType[length];

            for (int i = 0; i < length; ++i)
            {
                var rawValue = sass_list_get_value(rawPointer, i);
                var convertedValue = GetSassType(rawValue);

                values[i] = convertedValue;
            }

            return values;
        }

        internal static ISassType GetSassType(IntPtr rawPointer)
        {
            SassTag tag = sass_value_get_tag(rawPointer);

            switch (tag)
            {
                case SassTag.SassBoolean:
                    return SassBool.GetBoolValue(rawPointer);

                case SassTag.SassNumber:
                    return new SassNumber(rawPointer);

                case SassTag.SassColor:
                    return new SassColor(rawPointer);

                case SassTag.SassString:
                    return new SassString(rawPointer);

                case SassTag.SassList:
                    return new SassList(rawPointer);

                case SassTag.SassMap:
                    return new SassMap(rawPointer);

                case SassTag.SassNull:
                    return SassNull.Instance;

                case SassTag.SassError:
                    return new SassError(rawPointer);

                case SassTag.SassWarning:
                    return new SassWarning(rawPointer);
            }

            return null;
        }

        public static IntPtr GetRawPointer(ISassType returnedValue, InternalPtrValidityEventHandler onValidityEvent)
        {
            if (returnedValue == null)
                returnedValue = SassNull.Instance;

            var sassExportableType = returnedValue as ISassExportableType;

            if (sassExportableType != null)
                return sassExportableType.GetInternalTypePtr(onValidityEvent, true);

            // This should never happen. I could not find anyway to keep both
            // OOP and Resharper satisfied with the current design of ISassType
            // with public accessor and ISassExportableType with internal accessor
            // and all Sass types implementing both interfaces.
            return IntPtr.Zero;
        }
    }
}
