using System.Collections.Generic;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Dynamic
{
    public static class SystemFields
    {
        private static List<FieldConverter> _fields;
        public static List<FieldConverter> Fields
        {
            get
            {
                if (_fields == null)
                {
                    var initRecords = new List<FieldConverter>();

                    initRecords.Add(version());

                    _fields = initRecords;
                }
                return _fields;
            }
        }



        private static FieldConverter version()
        {
            FieldConverter converter = new FieldConverter();

            converter.DataType = typeof(long);
            converter.FieldName = "_version";
            converter.FieldNameHash = ObjectHelper.GetHashCode(converter.FieldName);
            converter.Length = 8;

            converter.RelativePosition = int.MaxValue;

            converter.SkipDefaultValue = true;

            //if (item.isComplex)
            //{
            //    Serializer.Simple.SimpleConverter simple = new Serializer.Simple.SimpleConverter(type);
            //    converter.ToBytes = simple.ToBytes;
            //    converter.FromBytes = simple.FromBytes;
            //    converter.IsComplex = true;
            //    converter.Length = int.MaxValue;
            //    converter.RelativePosition = int.MaxValue;
            //}

            converter.ToBytes = ConverterHelper.GetToBytes(typeof(long));
            converter.FromBytes = ConverterHelper.GetFromBytes(typeof(long));

            return converter;

        }


    }
}
