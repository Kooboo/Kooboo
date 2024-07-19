//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.Helper;
using Kooboo.IndexedDB.Serializer.Simple.FieldConverter;

namespace Kooboo.IndexedDB.Serializer.Simple
{
    public static class ConverterHelper
    {


        public static Func<object, byte[]> GetValueToBytes(Type type)
        {
            if (type == typeof(int))
            {
                return ValueConverter.IntToBytes;
            }
            else if (type == typeof(string))
            {
                return ValueConverter.StringToBytes;
            }
            else if (type == typeof(bool))
            {
                return ValueConverter.BoolToBytes;
            }
            else if (type == typeof(DateTime))
            {
                return ValueConverter.DateTimeToBytes;
            }
            else if (type == typeof(Guid))
            {
                return ValueConverter.GuidToBytes;
            }
            else if (type == typeof(byte))
            {
                return ValueConverter.ByteToBytes;
            }
            else if (type == typeof(byte[]))
            {
                return ValueConverter.ByteArrayToBytes;
            }
            else if (type == typeof(decimal))
            {
                return ValueConverter.DecimalToBytes;
            }
            else if (type == typeof(double))
            {
                return ValueConverter.DoubleToBytes;
            }
            else if (type == typeof(float))
            {
                return ValueConverter.FloatToBytes;
            }
            else if (type == typeof(Int16))
            {
                return ValueConverter.ShortToBytes;
            }
            else if (type == typeof(Int64))
            {
                return ValueConverter.LongToBytes;
            }
            else if (type == typeof(System.Net.IPAddress))
            {
                return ValueConverter.IpAddressToBytes;
            }
            else if (type == typeof(object))
            {
                return ValueConverter.ObjectToTypes;
            }

            else if (ObjectHelper.IsDictionary(type))
            {
                return new DictionaryConverter(type).ToBytes;
            }
            else if (ObjectHelper.IsList(type))
            {
                return new ListConverter(type).ToBytes;
            }
            else if (ObjectHelper.IsCollection(type))
            {
                return new CollectionConverter(type).ToBytes;
            }
            else if (type.IsClass)
            {
                ClassConverter converter = ClassConverterCache.Get(type);
                if (converter == null)
                {
                    converter = new ClassConverter(type);
                    ClassConverterCache.Add(type, converter);
                    converter.InitFields();
                }

                return converter.ToBytes;
            }

            return null;
        }

        public static Func<byte[], object> GetBytesToValue(Type type)
        {
            if (type == typeof(int))
            {
                return ValueConverter.FromIntBytes;
            }
            else if (type == typeof(string))
            {
                return ValueConverter.FromStringBytes;
            }
            else if (type == typeof(bool))
            {
                return ValueConverter.FromBoolBytes;
            }
            else if (type == typeof(DateTime))
            {
                return ValueConverter.FromDateTimeBytes;
            }
            else if (type == typeof(Guid))
            {
                return ValueConverter.FromGuidBytes;
            }
            else if (type == typeof(byte))
            {
                return ValueConverter.FromByteBytes;
            }
            else if (type == typeof(byte[]))
            {
                return ValueConverter.FromByteArrayBytes;
            }
            else if (type == typeof(decimal))
            {
                return ValueConverter.FromDecimalBytes;
            }
            else if (type == typeof(double))
            {
                return ValueConverter.FromDoubleBytes;
            }
            else if (type == typeof(float))
            {
                return ValueConverter.FromFloatBytes;
            }
            else if (type == typeof(Int16))
            {
                return ValueConverter.FromShortBytes;
            }
            else if (type == typeof(Int64))
            {
                return ValueConverter.FromLongBytes;
            }
            else if (type == typeof(System.Net.IPAddress))
            {
                return ValueConverter.FromBytesToIpaddress;
            }
            else if (type == typeof(object))
            {
                return ValueConverter.FromObjectBytes;
            }

            else if (ObjectHelper.IsDictionary(type))
            {
                return new DictionaryConverter(type).FromBytes;
            }
            else if (ObjectHelper.IsList(type))
            {
                return new ListConverter(type).FromBytes;
            }
            else if (ObjectHelper.IsCollection(type))
            {
                return new CollectionConverter(type).FromBytes;
            }

            else if (type.IsClass)
            {
                ClassConverter converter = ClassConverterCache.Get(type);
                if (converter == null)
                {
                    converter = new ClassConverter(type);
                    ClassConverterCache.Add(type, converter);
                    converter.InitFields();
                }

                return converter.FromBytes;
            }
            return null;
        }

        public static bool IsValueType(Type type)
        {
            if (type == typeof(int))
            {
                return true;
            }
            else if (type == typeof(string))
            {
                return true;
            }
            else if (type == typeof(bool))
            {
                return true;
            }
            else if (type == typeof(DateTime))
            {
                return true;
            }
            else if (type == typeof(Guid))
            {
                return true;
            }
            else if (type == typeof(byte))
            {
                return true;
            }
            else if (type == typeof(byte[]))
            {
                return true;
            }
            else if (type == typeof(decimal))
            {
                return true;
            }
            else if (type == typeof(double))
            {
                return true;
            }
            else if (type == typeof(float))
            {
                return true;
            }
            else if (type == typeof(Int16))
            {
                return true;
            }
            else if (type == typeof(Int64))
            {
                return true;
            }
            else if (type == typeof(object))
            {
                return true;
            }

            return false;
        }

        public static int GetTypeLength(Type type)
        {
            if (type == typeof(string))
            {
                return 0;
            }
            else if (type == typeof(Int32))
            {
                return 4;
            }
            else if (type == typeof(Int64))
            {
                return 8;
            }
            else if (type == typeof(Int16))
            {
                return 2;
            }
            else if (type == typeof(decimal))
            {
                ///decimal is not available, will be converted to double directly.
                return 8;
            }
            else if (type == typeof(double))
            {
                return 8;
            }
            else if (type == typeof(float))
            {
                return 4;
            }
            else if (type == typeof(DateTime))
            {
                return 8;
            }
            else if (type == typeof(Guid))
            {
                return 16;
            }
            else if (type == typeof(byte))
            {
                return 1;
            }
            else if (type == typeof(bool))
            {
                return 1;
            }
            else if (type.IsEnum)
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }

        public static EnumValueType GetEnumType(Type type)
        {
            if (type == typeof(int))
            {
                return EnumValueType.Int;
            }
            else if (type == typeof(string))
            {
                return EnumValueType.String;
            }
            else if (type == typeof(bool))
            {
                return EnumValueType.Bool;
            }
            else if (type == typeof(DateTime))
            {
                return EnumValueType.DateTime;
            }
            else if (type == typeof(Guid))
            {
                return EnumValueType.Guid;
            }
            else if (type == typeof(byte))
            {
                return EnumValueType.Byte;
            }
            else if (type == typeof(byte[]))
            {
                return EnumValueType.Bytes;
            }
            else if (type == typeof(decimal))
            {
                return EnumValueType.Decimal;
            }
            else if (type == typeof(double))
            {
                return EnumValueType.Double;
            }
            else if (type == typeof(float))
            {
                return EnumValueType.Float;
            }
            else if (type == typeof(Int16))
            {
                return EnumValueType.Short;
            }
            else if (type == typeof(Int64))
            {
                return EnumValueType.Long;
            }
            else if (type == typeof(object))
            {
                return EnumValueType.Object;
            }
            return EnumValueType.Unknown;
        }

        public static Type GetTypeFromEnumType(EnumValueType enumvalue)
        {
            switch (enumvalue)
            {
                case EnumValueType.Byte:
                    return typeof(byte);

                case EnumValueType.Short:
                    return typeof(short);

                case EnumValueType.Int:
                    return typeof(int);

                case EnumValueType.Long:
                    return typeof(long);

                case EnumValueType.Bool:
                    return typeof(bool);

                case EnumValueType.DateTime:
                    return typeof(DateTime);

                case EnumValueType.Decimal:
                    return typeof(decimal);

                case EnumValueType.Float:
                    return typeof(float);

                case EnumValueType.Double:
                    return typeof(double);

                case EnumValueType.Guid:
                    return typeof(Guid);

                case EnumValueType.Bytes:
                    return typeof(byte[]);
                case EnumValueType.String:
                    return typeof(string);
                default:
                    return null;
            }

        }

        public static Func<byte[], object> GetBytesToValueFromEnum(EnumValueType enumType)
        {
            switch (enumType)
            {
                case EnumValueType.Byte:
                    return ValueConverter.FromByteBytes;
                case EnumValueType.Short:
                    return ValueConverter.FromShortBytes;
                case EnumValueType.Int:
                    return ValueConverter.FromIntBytes;
                case EnumValueType.Long:
                    return ValueConverter.FromLongBytes;
                case EnumValueType.Bool:
                    return ValueConverter.FromBoolBytes;
                case EnumValueType.DateTime:
                    return ValueConverter.FromDateTimeBytes;
                case EnumValueType.Decimal:
                    return ValueConverter.FromDecimalBytes;
                case EnumValueType.Float:
                    return ValueConverter.FromFloatBytes;
                case EnumValueType.Double:
                    return ValueConverter.FromDoubleBytes;
                case EnumValueType.Guid:
                    return ValueConverter.FromGuidBytes;
                case EnumValueType.Bytes:
                    return ValueConverter.FromByteArrayBytes;
                case EnumValueType.String:
                    return ValueConverter.FromStringBytes;
                case EnumValueType.Object:
                    return ValueConverter.FromObjectBytes;
                default:
                    return null;
            }
        }

        public static IFieldConverter<T> GetFieldConverter<T>(Type FieldType, string FieldName)
        {
            if (ObjectHelper.IsDictionary(FieldType))
            {
                return new DictionaryFieldConverter<T>(FieldType, FieldName);
            }

            else if (ObjectHelper.IsList(FieldType))
            {
                return new ListFieldConverter<T>(FieldType, FieldName);
            }
            else if (ObjectHelper.IsCollection(FieldType))
            {
                return new CollectionFieldConverter<T>(FieldType, FieldName);
            }

            else if (FieldType == typeof(int))
            {
                return new IntFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(string))
            {
                return new StringFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(bool))
            {
                return new BoolFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(DateTime))
            {
                return new DateTimeFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(Guid))
            {
                return new GuidFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(byte))
            {
                return new ByteFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(byte[]))
            {
                return new ByteArrayFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(decimal))
            {
                return new DecimalFieldConvertercs<T>(FieldName);
            }
            else if (FieldType == typeof(double))
            {
                return new DoubleFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(float))
            {
                return new FloatFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(Int16))
            {
                return new ShortFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(Int64))
            {
                return new LongFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(System.Net.IPAddress))
            {
                return new IpAddressFieldConverter<T>(FieldName);
            }
            else if (FieldType == typeof(object))
            {
                return new ObjectFieldConverter<T>(FieldName);
            }

            else if (FieldType.IsClass)
            {
                return new ClassFieldConverter<T>(FieldName);
            }

            else if (FieldType.IsEnum)
            {
                return new EnumFieldConveter<T>(FieldName, FieldType);
            }

            else
            {
                throw new Exception(FieldType.Name + " can not be identified.");
            }
        }

        public static IFieldConverter GetFieldConverter(Type ObjectType, Type FieldType, string FieldName)
        {
            if (ObjectHelper.IsDictionary(FieldType))
            {
                return new DictionaryFieldConverter(FieldName, ObjectType, FieldType);
            }
            else if (ObjectHelper.IsList(FieldType))
            {
                return new ListFieldConverter(FieldName, ObjectType, FieldType);
            }
            else if (ObjectHelper.IsCollection(FieldType))
            {
                return new CollectionFieldConverter(ObjectType, FieldType, FieldName);
            }

            else if (FieldType == typeof(int))
            {
                return new IntFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(string))
            {
                return new StringFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(bool))
            {
                return new BoolFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(DateTime))
            {
                return new DateTimeFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(Guid))
            {
                return new GuidFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(byte))
            {
                return new ByteFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(byte[]))
            {
                return new ByteArrayFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(decimal))
            {
                return new DecimalFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(double))
            {
                return new DoubleFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(float))
            {
                return new FloatFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(Int16))
            {
                return new ShortFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(Int64))
            {
                return new LongFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(System.Net.IPAddress))
            {
                return new IpAddressFieldConverter(FieldName, ObjectType);
            }
            else if (FieldType == typeof(object))
            {
                return new ObjectFieldConverter(FieldName, ObjectType);
            }

            else if (FieldType.IsClass)
            {
                return new ClassFieldConverter(ObjectType, FieldType, FieldName);
            }

            else if (FieldType.IsEnum)
            {
                return new EnumFieldConveter(FieldName, ObjectType, FieldType);
            }

            else
            {
                throw new Exception(FieldType.Name + " can not be identified.");
            }
        }


        private static Dictionary<string, Func<byte[], object>> _typenameconverter;
        public static Dictionary<string, Func<byte[], object>> TypeNameConverter
        {
            get
            {
                if (_typenameconverter == null)
                {
                    var result = new Dictionary<string, Func<byte[], object>>(StringComparer.OrdinalIgnoreCase);

                    result.Add(typeof(int).Name, ValueConverter.FromIntBytes);
                    result.Add(typeof(string).Name, ValueConverter.FromStringBytes);
                    result.Add(typeof(bool).Name, ValueConverter.FromBoolBytes);
                    result.Add(typeof(DateTime).Name, ValueConverter.FromDateTimeBytes);
                    result.Add(typeof(Guid).Name, ValueConverter.FromGuidBytes);
                    result.Add(typeof(byte).Name, ValueConverter.FromByteBytes);
                    result.Add(typeof(byte[]).Name, ValueConverter.FromByteArrayBytes);
                    result.Add(typeof(decimal).Name, ValueConverter.FromDecimalBytes);
                    result.Add(typeof(double).Name, ValueConverter.FromDoubleBytes);

                    result.Add(typeof(float).Name, ValueConverter.FromFloatBytes);
                    result.Add(typeof(Int16).Name, ValueConverter.FromShortBytes);
                    result.Add(typeof(Int64).Name, ValueConverter.FromLongBytes);

                    result.Add(typeof(System.Net.IPAddress).Name, ValueConverter.FromBytesToIpaddress);
                    result.Add(typeof(object).Name, ValueConverter.FromObjectBytes);

                    _typenameconverter = result;
                }
                return _typenameconverter;
            }
        }


    }
}
