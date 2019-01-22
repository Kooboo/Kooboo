using System;
using System.Collections.Generic;
using System.Linq;
using LibSass.Compiler.Context;
using static LibSass.Compiler.SassExterns;
using static LibSass.Types.TypeFactory;

namespace LibSass.Types
{
    public class SassList : ISassType, ISassExportableType
    {
        private bool _ensured;
        private IntPtr _cachedPtr;

        public List<ISassType> Values { get; set; }
        public SassListSeparator Separator { get; set; } = SassListSeparator.Space;

        public SassList()
        {
            Values = new List<ISassType>();
        }

        internal SassList(IntPtr rawPointer)
        {
            Values = GetSassArguments(rawPointer).ToList();

            var sep = sass_list_get_separator(rawPointer);
            Separator = (SassListSeparator)Enum.GetValues(typeof(SassListSeparator)).GetValue(sep);
        }

        /// <summary>
        /// Recursively ensures:
        /// * Arbitrary ISassType implementation.
        /// * Circular reference on each "listy" item stored in the values.
        /// </summary>
        /// <param name="lists">List containing instances of SassList</param>
        private void WalkAndEnsureDependencies(List<SassList> lists)
        {
            // Prevent from re-entrance.
            if (_ensured)
                return;

            // FIXME: Should we instead loop through the array and
            //        report the exact index which violates this rule?
            if (!Values.All(v => v is ISassExportableType))
                throw new SassTypeException(SassTypeException.ArbitraryInterfaceImplmentationMessage);

            // Detect the circular-referencing values.
            lists.Add(this);

            var filteredValues = Values.OfType<SassList>().ToList();

            if (filteredValues.Any(lists.Contains))
                throw new SassTypeException(SassTypeException.CircularReferenceMessage);

            filteredValues.ForEach(v => v.WalkAndEnsureDependencies(lists));

            _ensured = true;
        }

        public override string ToString()
        {
            return string.Join(Separator.ToEnumString(),
                               Values.Select(v => v.ToString()));
        }

        IntPtr ISassExportableType.GetInternalTypePtr(InternalPtrValidityEventHandler validityEventHandler, bool dontCache)
        {
            if (_cachedPtr != default(IntPtr))
                return _cachedPtr;

            WalkAndEnsureDependencies(new List<SassList>());

            var list = sass_make_list(Values.Count, Separator);

            for (int index = 0; index < Values.Count; ++index)
            {
                var exportableValue = (ISassExportableType)Values[index];

                sass_list_set_value(list, index, exportableValue.GetInternalTypePtr(validityEventHandler));
            }

            if (dontCache)
                return list;

            validityEventHandler += (this as ISassExportableType).OnInvalidated;
            return _cachedPtr = list;
        }

        void ISassExportableType.OnInvalidated()
        {
            _cachedPtr = default(IntPtr);
        }
    }
}
