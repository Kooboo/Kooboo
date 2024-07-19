//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.Serializer.Simple;

namespace Kooboo.IndexedDB.Dynamic
{
    public static class ConverterHelper
    {
        public static Func<object, byte[]> GetToBytes(Type type)
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
            else
            {
                throw new Exception(type.Name + " Value type not supported");
            }
        }

        public static Func<byte[], object> GetFromBytes(Type type)
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
            else
            {
                throw new Exception(type.Name + " Value type not supported");
            }
        }

    }
}
