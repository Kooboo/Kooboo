//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;
using System.Net;

namespace Kooboo.IndexedDB.Serializer.Simple
{
    public static class ValueConverter
    {
        public static byte[] ToBytes(object value)
        {
            throw new NotImplementedException();
        }

        public static byte[] ObjectToTypes(object value)
        {
            if (value == null)
            {
                return null;
            }
            Type type = value.GetType();

            var tobytes = ConverterHelper.GetValueToBytes(type);

            if (tobytes == null)
            {
                throw new Exception("sorry, only support Value Type, like Int, guid, datetime, string as the object value now.");
            }
            else
            {
                var valueBytes = tobytes(value);
                var enumtype = ConverterHelper.GetEnumType(type);
                byte enumbyte = (byte)enumtype;

                byte[] result = new byte[valueBytes.Length + 1];

                result[0] = enumbyte;
                System.Buffer.BlockCopy(valueBytes, 0, result, 1, valueBytes.Length);

                return result;
            }
        }

        public static object FromObjectBytes(byte[] bytes)
        {
            byte indicatorByte = bytes[0];

            var enumtype = (EnumValueType)indicatorByte;

            var converter = ConverterHelper.GetBytesToValueFromEnum(enumtype);

            if (converter != null)
            {
                int valuelen = bytes.Length - 1;
                byte[] valueBytes = new byte[valuelen];
                System.Buffer.BlockCopy(bytes, 1, valueBytes, 0, valuelen);
                return converter(valueBytes);
            }

            return null;
        }

        public static byte[] ToBytes(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            return GlobalSettings.DefaultEncoding.GetBytes(value);
        }

        public static string ToString(byte[] bytes)
        {
            return GlobalSettings.DefaultEncoding.GetString(bytes).TrimEnd('\0');
        }

        public static byte[] StringToBytes(object value)
        {
            if (value == null)
            {
                return GlobalSettings.DefaultEncoding.GetBytes(string.Empty);
            }
            return GlobalSettings.DefaultEncoding.GetBytes(value as string);
        }

        public static object FromStringBytes(byte[] bytes)
        {
            return GlobalSettings.DefaultEncoding.GetString(bytes).TrimEnd('\0');
        }

        public static byte[] ToBytes(long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static long ToLong(byte[] bytes)
        {
            return BitConverter.ToInt64(bytes, 0);
        }

        public static byte[] LongToBytes(object value)
        {
            return BitConverter.GetBytes(Convert.ToInt64(value));
        }

        public static object FromLongBytes(byte[] bytes)
        {
            return BitConverter.ToInt64(bytes, 0);
        }

        public static byte[] IpAddressToBytes(object value)
        {
            if (value is IPAddress ipaddress)
            {
                return ipaddress.GetAddressBytes();
            }
            return null;
        }

        public static object FromBytesToIpaddress(byte[] bytes)
        {
            var ipaddress = new System.Net.IPAddress(bytes);

            return ipaddress;
        }

        public static byte[] ToBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static int ToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        public static byte[] IntToBytes(object value)
        {
            return BitConverter.GetBytes(Convert.ToInt32(value));
        }

        public static object FromIntBytes(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        public static byte[] ToBytes(short value)
        {
            return BitConverter.GetBytes(value);
        }

        public static short ToShort(byte[] bytes)
        {
            return BitConverter.ToInt16(bytes, 0);
        }

        public static byte[] ShortToBytes(object value)
        {
            return BitConverter.GetBytes(Convert.ToInt16(value));
        }

        public static object FromShortBytes(byte[] bytes)
        {
            return BitConverter.ToInt16(bytes, 0);
        }

        public static byte[] ToBytes(Guid value)
        {
            return value.ToByteArray();
        }

        public static Guid ToGuid(byte[] bytes)
        {
            Guid newguid = new Guid(bytes);
            return newguid;
        }

        public static byte[] GuidToBytes(object value)
        {
            string guidstring = Convert.ToString(value);
            return System.Guid.TryParse(guidstring, out var guidvalue) ? guidvalue.ToByteArray() : default(Guid).ToByteArray();
        }

        public static object FromGuidBytes(byte[] bytes)
        {
            Guid newguid = new Guid(bytes);
            return newguid;
        }

        public static byte[] ToBytes(float value)
        {
            return BitConverter.GetBytes(value);
        }

        public static float ToFloat(byte[] bytes)
        {
            // float == single
            return BitConverter.ToSingle(bytes, 0);
        }

        public static byte[] FloatToBytes(object value)
        {
            float floatvalue = Convert.ToSingle(value);
            return BitConverter.GetBytes(floatvalue);
        }

        public static object FromFloatBytes(byte[] bytes)
        {
            // float == single
            return BitConverter.ToSingle(bytes, 0);
        }

        public static byte[] ToBytes(double value)
        {
            return BitConverter.GetBytes(value);
        }

        public static Double ToDouble(byte[] bytes)
        {
            return BitConverter.ToDouble(bytes, 0);
        }

        public static byte[] DoubleToBytes(object value)
        {
            double doublevalue = Convert.ToDouble(value);
            return BitConverter.GetBytes(doublevalue);
        }

        public static object FromDoubleBytes(byte[] bytes)
        {
            return BitConverter.ToDouble(bytes, 0);
        }

        public static byte[] ToBytes(decimal value)
        {
            double doublenumber = Convert.ToDouble(value);
            return BitConverter.GetBytes(doublenumber);
        }

        public static decimal ToDecimal(byte[] bytes)
        {
            double doublenumber = BitConverter.ToDouble(bytes, 0);
            decimal decimalnumber = Convert.ToDecimal(doublenumber);
            return decimalnumber;
        }

        public static byte[] DecimalToBytes(object value)
        {
            double doublenumber = Convert.ToDouble(value);
            return BitConverter.GetBytes(doublenumber);
        }

        public static object FromDecimalBytes(byte[] bytes)
        {
            double doublenumber = BitConverter.ToDouble(bytes, 0);
            decimal decimalnumber = Convert.ToDecimal(doublenumber);
            return decimalnumber;
        }

        public static byte[] ToBytes(DateTime value)
        {
            return DateTimeUtcHelper.ToBytes(value);
        }

        public static DateTime ToDateTime(byte[] bytes)
        {
            return DateTimeUtcHelper.ToDateTime(bytes);
        }

        public static byte[] DateTimeToBytes(object value)
        {
            DateTime datetime = Convert.ToDateTime(value);
            return DateTimeUtcHelper.ToBytes(datetime);
        }

        public static object FromDateTimeBytes(byte[] bytes)
        {
            return DateTimeUtcHelper.ToDateTime(bytes);
        }

        public static byte[] ToBytes(byte value)
        {
            byte[] newbytes = new byte[1];
            newbytes[0] = value;
            return newbytes;
        }

        public static byte ToByte(byte[] bytes)
        {
            return bytes[0];
        }

        public static byte[] ByteToBytes(object value)
        {
            byte bytevalue = Convert.ToByte(value);
            byte[] newbytes = new byte[1];
            newbytes[0] = bytevalue;
            return newbytes;
        }

        public static object FromByteBytes(byte[] bytes)
        {
            return bytes[0];
        }

        public static byte[] ToBytes(bool value)
        {
            byte[] bytes = new byte[1];
            if (value)
            {
                bytes[0] = 1;
            }
            return bytes;
        }

        public static bool ToBool(byte[] bytes)
        {
            if (bytes[0] > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static byte[] BoolToBytes(object value)
        {
            bool boolvaule = Convert.ToBoolean(value);
            byte[] bytes = new byte[1];
            if (boolvaule)
            {
                bytes[0] = 1;
            }
            return bytes;
        }

        public static object FromBoolBytes(byte[] bytes)
        {
            if (bytes[0] > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static byte[] ToBytes(byte[] value)
        {
            return value;
        }

        public static byte[] ToByteArray(byte[] bytes)
        {
            return bytes;
        }

        public static byte[] ByteArrayToBytes(object value)
        {
            return value as byte[];
        }

        public static object FromByteArrayBytes(byte[] bytes)
        {
            return bytes;
        }
    }
}