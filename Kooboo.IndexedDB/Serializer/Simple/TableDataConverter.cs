using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.Helper;

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

            int KeyByteLen = BitConverter.ToInt32(bytes, 0);
            int ValueByteLen = BitConverter.ToInt32(bytes, 4);
            if (KeyByteLen == 0) { return null; }

            byte[] KeyTotalBytes = new byte[KeyByteLen];
            byte[] ValueTotalBytes = new byte[ValueByteLen];

            System.Buffer.BlockCopy(bytes, 8, KeyTotalBytes, 0, KeyByteLen);
            System.Buffer.BlockCopy(bytes, 8 + KeyByteLen, ValueTotalBytes, 0, ValueByteLen);

            int KeyStartPos = 0;

            while (true)
            {
                int len = BitConverter.ToInt32(KeyTotalBytes, KeyStartPos);
                KeyStartPos += 4;

                if (len > 0)
                {
                    byte[] onekey = new byte[len];
                    System.Buffer.BlockCopy(KeyTotalBytes, KeyStartPos, onekey, 0, len);
                    KeyStartPos += len;
                    keys.Add(System.Text.Encoding.UTF8.GetString(onekey));
                }
                else
                {
                    keys.Add(null);
                }
                if (KeyStartPos >= KeyByteLen)
                { break; }
            }

            int ValueStartPos = 0;

            while (true)
            {
                int len = BitConverter.ToInt32(ValueTotalBytes, ValueStartPos);
                ValueStartPos += 4;

                if (len > 0)
                {
                    byte[] ItemBytes = new byte[len];
                    System.Buffer.BlockCopy(ValueTotalBytes, ValueStartPos, ItemBytes, 0, len);
                    ValueStartPos += len;

                    // valuebytes.Add(OneValueBytes);
                    var LenTypeName = BitConverter.ToInt32(ItemBytes, 0);
                    var LenValueBytes = BitConverter.ToInt32(ItemBytes, 4);

                    var typenameBytes = new byte[LenTypeName];
                    System.Buffer.BlockCopy(ItemBytes, 8, typenameBytes, 0, LenTypeName);
                    var TypeName = System.Text.Encoding.UTF8.GetString(typenameBytes);

                    if (LenValueBytes > 0)
                    {
                        var valueBytes = new byte[LenValueBytes];
                        System.Buffer.BlockCopy(ItemBytes, 8 + LenTypeName, valueBytes, 0, LenValueBytes);
                        var value = BytesToValue(TypeName, valueBytes);
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

                if (ValueStartPos >= ValueByteLen)
                { break; }
            }

            Dictionary<string, object> Result = new Dictionary<string, object>();


            int count = keys.Count;

            for (int i = 0; i < count; i++)
            {
                var key = keys[i];
                var value = values[i];

                Result[key] = value;
            }

            return Result;
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

            byte[] KeyBytes = new byte[keytotallen];
            int currentposition = 0;

            foreach (var item in keyresults)
            {
                int len = item.Length;
                System.Buffer.BlockCopy(item, 0, KeyBytes, currentposition, len);
                currentposition += len;
            }

            List<byte[]> ValueResults = new List<byte[]>();

            int ValueTotalLen = 0;

            foreach (var item in dict.Values)
            {
                if (item == null)
                {
                    ValueResults.Add(BitConverter.GetBytes(0));
                    ValueTotalLen += 4;
                }
                else
                {
                    var type = item.GetType();
                    var bytes = GetValueBytes(type, item);

                    var typename = type.Name;
                    var typenamebytes = System.Text.Encoding.UTF8.GetBytes(typename);

                    ValueResults.Add(BitConverter.GetBytes(typenamebytes.Length + bytes.Length + 8));
                    ValueResults.Add(BitConverter.GetBytes(typenamebytes.Length));
                    ValueResults.Add(BitConverter.GetBytes(bytes.Length));

                    ValueResults.Add(typenamebytes);
                    ValueResults.Add(bytes);

                    ValueTotalLen += 4 + 4 + 4 + typenamebytes.Length + bytes.Length;

                }

            }

            byte[] ValueBytes = new byte[ValueTotalLen];

            int valuecurrentposition = 0;

            foreach (var item in ValueResults)
            {
                int len = item.Length;
                System.Buffer.BlockCopy(item, 0, ValueBytes, valuecurrentposition, len);
                valuecurrentposition += len;
            }

            int total = KeyBytes.Length + ValueBytes.Length + 8;

            byte[] totalbytes = new byte[total];

            System.Buffer.BlockCopy(BitConverter.GetBytes(KeyBytes.Length), 0, totalbytes, 0, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes(ValueBytes.Length), 0, totalbytes, 4, 4);
            if (KeyBytes.Length > 0)
            {
                System.Buffer.BlockCopy(KeyBytes, 0, totalbytes, 8, KeyBytes.Length);
            }
            if (ValueBytes.Length > 0)
            {
                System.Buffer.BlockCopy(ValueBytes, 0, totalbytes, 8 + KeyBytes.Length, ValueBytes.Length);
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

        public object BytesToValue(string typename, byte[] ValueBytes)
        {
            if (ConverterHelper.TypeNameConverter.ContainsKey(typename))
            {
                var convert = ConverterHelper.TypeNameConverter[typename];
                return convert(ValueBytes);
            }

            var type = TypeHelper.GetType(typename);
            if (type == null)
            {
                return null;
            }

            var converter = ConverterHelper.GetBytesToValue(type);
            if (converter != null)
            {
                return converter(ValueBytes);
            }
            else
            {
                return null;
            }
        }

    }

}
