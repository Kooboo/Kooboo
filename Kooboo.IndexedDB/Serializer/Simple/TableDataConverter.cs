using Kooboo.IndexedDB.Helper;
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Serializer.Simple
{
    // TEMP, this is used for table log, which is dictionary<string, object>.
    public class TableDataLogConverter
    {
        private static TableDataLogConverter _instance;

        public static TableDataLogConverter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TableDataLogConverter();
                }
                return _instance;
            }
        }

        public Dictionary<string, object> FromBytes(byte[] bytes)
        {
            List<string> keys = new List<string>();

            List<object> values = new List<object>();

            List<byte[]> valuebytes = new List<byte[]>();

            int keyByteLen = BitConverter.ToInt32(bytes, 0);
            int valueByteLen = BitConverter.ToInt32(bytes, 4);
            if (keyByteLen == 0) { return null; }

            byte[] keyTotalBytes = new byte[keyByteLen];
            byte[] valueTotalBytes = new byte[valueByteLen];

            System.Buffer.BlockCopy(bytes, 8, keyTotalBytes, 0, keyByteLen);
            System.Buffer.BlockCopy(bytes, 8 + keyByteLen, valueTotalBytes, 0, valueByteLen);

            int keyStartPos = 0;

            while (true)
            {
                int len = BitConverter.ToInt32(keyTotalBytes, keyStartPos);
                keyStartPos += 4;

                if (len > 0)
                {
                    byte[] onekey = new byte[len];
                    System.Buffer.BlockCopy(keyTotalBytes, keyStartPos, onekey, 0, len);
                    keyStartPos += len;
                    keys.Add(System.Text.Encoding.UTF8.GetString(onekey));
                }
                else
                {
                    keys.Add(null);
                }
                if (keyStartPos >= keyByteLen)
                { break; }
            }

            int valueStartPos = 0;

            while (true)
            {
                int len = BitConverter.ToInt32(valueTotalBytes, valueStartPos);
                valueStartPos += 4;

                if (len > 0)
                {
                    byte[] itemBytes = new byte[len];
                    System.Buffer.BlockCopy(valueTotalBytes, valueStartPos, itemBytes, 0, len);
                    valueStartPos += len;

                    // valuebytes.Add(OneValueBytes);
                    var lenTypeName = BitConverter.ToInt32(itemBytes, 0);
                    var lenValueBytes = BitConverter.ToInt32(itemBytes, 4);

                    var typenameBytes = new byte[lenTypeName];
                    System.Buffer.BlockCopy(itemBytes, 8, typenameBytes, 0, lenTypeName);
                    var typeName = System.Text.Encoding.UTF8.GetString(typenameBytes);

                    if (lenValueBytes > 0)
                    {
                        var valueBytes = new byte[lenValueBytes];
                        System.Buffer.BlockCopy(itemBytes, 8 + lenTypeName, valueBytes, 0, lenValueBytes);
                        var value = BytesToValue(typeName, valueBytes);
                        values.Add(value);
                    }
                    else
                    {
                        values.Add(null);
                    }
                }
                else
                {
                    values.Add(null);
                }

                if (valueStartPos >= valueByteLen)
                { break; }
            }

            Dictionary<string, object> result = new Dictionary<string, object>();

            int count = keys.Count;

            for (int i = 0; i < count; i++)
            {
                var key = keys[i];
                var value = values[i];

                result[key] = value;
            }

            return result;
        }

        public byte[] ToBytes(object value)
        {
            if (value == null)
            { return null; }
            var dict = value as Dictionary<string, object>;

            return ToBytes(dict);
        }

        public byte[] ToBytes(Dictionary<string, object> dict)
        {
            if (dict == null)
            {
                return null;
            }

            List<byte[]> keyresults = new List<byte[]>();

            int keytotallen = 0;

            foreach (var item in dict.Keys)
            {
                var keyresult = System.Text.Encoding.UTF8.GetBytes(item);
                keyresults.Add(BitConverter.GetBytes(keyresult.Length));
                keyresults.Add(keyresult);
                keytotallen += 4 + keyresult.Length;
            }

            byte[] keyBytes = new byte[keytotallen];
            int currentposition = 0;

            foreach (var item in keyresults)
            {
                int len = item.Length;
                System.Buffer.BlockCopy(item, 0, keyBytes, currentposition, len);
                currentposition += len;
            }

            List<byte[]> valueResults = new List<byte[]>();

            int valueTotalLen = 0;

            foreach (var item in dict.Values)
            {
                if (item == null)
                {
                    valueResults.Add(BitConverter.GetBytes(0));
                    valueTotalLen += 4;
                }
                else
                {
                    var type = item.GetType();
                    var bytes = GetValueBytes(type, item);

                    var typename = type.Name;
                    var typenamebytes = System.Text.Encoding.UTF8.GetBytes(typename);

                    valueResults.Add(BitConverter.GetBytes(typenamebytes.Length + bytes.Length + 8));
                    valueResults.Add(BitConverter.GetBytes(typenamebytes.Length));
                    valueResults.Add(BitConverter.GetBytes(bytes.Length));

                    valueResults.Add(typenamebytes);
                    valueResults.Add(bytes);

                    valueTotalLen += 4 + 4 + 4 + typenamebytes.Length + bytes.Length;
                }
            }

            byte[] valueBytes = new byte[valueTotalLen];

            int valuecurrentposition = 0;

            foreach (var item in valueResults)
            {
                int len = item.Length;
                System.Buffer.BlockCopy(item, 0, valueBytes, valuecurrentposition, len);
                valuecurrentposition += len;
            }

            int total = keyBytes.Length + valueBytes.Length + 8;

            byte[] totalbytes = new byte[total];

            System.Buffer.BlockCopy(BitConverter.GetBytes(keyBytes.Length), 0, totalbytes, 0, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes(valueBytes.Length), 0, totalbytes, 4, 4);
            if (keyBytes.Length > 0)
            {
                System.Buffer.BlockCopy(keyBytes, 0, totalbytes, 8, keyBytes.Length);
            }
            if (valueBytes.Length > 0)
            {
                System.Buffer.BlockCopy(valueBytes, 0, totalbytes, 8 + keyBytes.Length, valueBytes.Length);
            }
            return totalbytes;
        }

        public byte[] GetValueBytes(Type type, object value)
        {
            var converter = ConverterHelper.GetValueToBytes(type);
            if (converter != null)
            {
                return converter(value);
            }
            else
            {
                return new byte[0];
            }
        }

        public object BytesToValue(string typename, byte[] valueBytes)
        {
            if (ConverterHelper.TypeNameConverter.ContainsKey(typename))
            {
                var convert = ConverterHelper.TypeNameConverter[typename];
                return convert(valueBytes);
            }

            var type = TypeHelper.GetType(typename);
            if (type == null)
            {
                return null;
            }

            var converter = ConverterHelper.GetBytesToValue(type);
            return converter?.Invoke(valueBytes);
        }
    }
}