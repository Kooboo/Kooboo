//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Serializer.Simple
{
    public class DictionaryConverter
    {
        private Type KeyType;
        private Type ValueType;
        private Type DictionaryType;

        private int KeyLength;
        private int Valuelength;

        private Func<object, byte[]> GetKeyObjectBytes;
        private Func<byte[], object> GetKeyObjectValue;

        private Func<object, byte[]> GetValueObjectBytes;
        private Func<byte[], object> GetValueObjectValue;

        private bool IsIgnoreCase { get; set; }

        public DictionaryConverter(Type dictionaryType, bool keyIgnoreCase = false)
        {
            this.IsIgnoreCase = keyIgnoreCase;

            this.DictionaryType = dictionaryType;
            KeyType = ObjectHelper.GetDictionaryKeyType(dictionaryType);
            ValueType = ObjectHelper.GetDictionaryValueType(dictionaryType);

            KeyLength = ConverterHelper.GetTypeLength(KeyType);
            Valuelength = ConverterHelper.GetTypeLength(ValueType);

            GetKeyObjectBytes = ConverterHelper.GetValueToBytes(KeyType);
            GetKeyObjectValue = ConverterHelper.GetBytesToValue(KeyType);

            GetValueObjectBytes = ConverterHelper.GetValueToBytes(ValueType);
            GetValueObjectValue = ConverterHelper.GetBytesToValue(ValueType);

            if (GetKeyObjectBytes == null || GetKeyObjectValue == null)
            {
                throw new Exception(KeyType.Name + " is not yet supported.");
            }

            if (GetValueObjectBytes == null || GetValueObjectValue == null)
            {
                throw new Exception(ValueType.Name + " is not yet supported.");
            }
        }

        public object FromBytes(byte[] bytes)
        {
            List<byte[]> keybytes = new List<byte[]>();
            List<byte[]> valuebytes = new List<byte[]>();

            int keyByteLen = BitConverter.ToInt32(bytes, 0);
            int valueByteLen = BitConverter.ToInt32(bytes, 4);
            if (keyByteLen == 0) { return null; }

            byte[] keyTotalBytes = new byte[keyByteLen];
            byte[] valueTotalBytes = new byte[valueByteLen];

            System.Buffer.BlockCopy(bytes, 8, keyTotalBytes, 0, keyByteLen);
            System.Buffer.BlockCopy(bytes, 8 + keyByteLen, valueTotalBytes, 0, valueByteLen);

            int keystartposition = 0;

            while (true)
            {
                if (this.KeyLength > 0)
                {
                    byte[] onekeybytes = new byte[this.KeyLength];
                    System.Buffer.BlockCopy(keyTotalBytes, keystartposition, onekeybytes, 0, this.KeyLength);
                    keystartposition += this.KeyLength;
                    keybytes.Add(onekeybytes);
                }
                else
                {
                    int len = BitConverter.ToInt32(keyTotalBytes, keystartposition);
                    keystartposition += 4;

                    if (len > 0)
                    {
                        byte[] onekeybytes = new byte[len];
                        System.Buffer.BlockCopy(keyTotalBytes, keystartposition, onekeybytes, 0, len);
                        keystartposition += len;
                        keybytes.Add(onekeybytes);
                    }
                    else
                    {// TODO: Maybe need to check keytype = string and insert string.empty now.
                        keybytes.Add(null);
                    }
                }

                if (keystartposition >= keyByteLen)
                { break; }
            }

            int valuestartposition = 0;

            while (true)
            {
                if (this.Valuelength > 0)
                {
                    byte[] onebytes = new byte[this.Valuelength];
                    System.Buffer.BlockCopy(valueTotalBytes, valuestartposition, onebytes, 0, this.Valuelength);
                    valuestartposition += this.Valuelength;
                    valuebytes.Add(onebytes);
                }
                else
                {
                    int len = BitConverter.ToInt32(valueTotalBytes, valuestartposition);
                    valuestartposition += 4;

                    if (len > 0)
                    {
                        byte[] onebytes = new byte[len];
                        System.Buffer.BlockCopy(valueTotalBytes, valuestartposition, onebytes, 0, len);
                        valuestartposition += len;
                        valuebytes.Add(onebytes);
                    }
                    else
                    {
                        valuebytes.Add(null);
                    }
                }

                if (valuestartposition >= valueByteLen)
                { break; }
            }

            System.Collections.IDictionary dict = null;
            if (this.IsIgnoreCase)
            {
                List<object> para = new List<object> {StringComparer.OrdinalIgnoreCase};
                dict = Activator.CreateInstance(this.DictionaryType, para.ToArray()) as System.Collections.IDictionary;
            }
            else
            {
                dict = Activator.CreateInstance(this.DictionaryType) as System.Collections.IDictionary;
            }

            int count = keybytes.Count;

            for (int i = 0; i < count; i++)
            {
                var keybyte = keybytes[i];
                var valuebyte = valuebytes[i];
                if (keybyte != null)
                {
                    var dictkey = this.GetKeyObjectValue(keybyte);
                    if (valuebyte == null)
                    {
                        dict.Add(dictkey, null);
                    }
                    else
                    {
                        var dictvalue = this.GetValueObjectValue(valuebyte);
                        dict.Add(dictkey, dictvalue);
                    }
                }
                else if (KeyType == typeof(string))
                {
                    if (valuebyte == null)
                    {
                        dict.Add(string.Empty, null);
                    }
                    else
                    {
                        var dictvalue = this.GetValueObjectValue(valuebyte);
                        dict.Add(string.Empty, dictvalue);
                    }
                }
            }

            return dict;
        }

        public byte[] ToBytes(object value)
        {
            if (value == null)
            { return null; }
            var dict = value as System.Collections.IDictionary;
            if (dict == null)
            {
                return null;
            }

            List<byte[]> keyresults = new List<byte[]>();
            int keytotallen = 0;

            foreach (var item in dict.Keys)
            {
                var keyresult = this.GetKeyObjectBytes(item);

                if (this.KeyLength > 0)
                {
                    keyresults.Add(keyresult);
                    keytotallen += this.KeyLength;
                }
                else
                {
                    keyresults.Add(BitConverter.GetBytes(keyresult.Length));
                    keyresults.Add(keyresult);
                    keytotallen += 4 + keyresult.Length;
                }
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
                var valueResult = this.GetValueObjectBytes(item);

                if (this.Valuelength > 0)
                {
                    valueResults.Add(valueResult);
                    valueTotalLen += this.Valuelength;
                }
                else
                {
                    int len = this.Valuelength;
                    if (len == 0 && valueResult != null)
                    {
                        len = valueResult.Length;
                    }
                    valueResults.Add(BitConverter.GetBytes(len));
                    valueResults.Add(valueResult);
                    valueTotalLen += 4 + len;
                }
            }

            byte[] valueBytes = new byte[valueTotalLen];
            int valuecurrentposition = 0;

            foreach (var item in valueResults)
            {
                int len = this.Valuelength;
                if (len == 0 && item != null)
                {
                    len = item.Length;
                }
                if (len > 0)
                {
                    System.Buffer.BlockCopy(item, 0, valueBytes, valuecurrentposition, len);
                    valuecurrentposition += len;
                }
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
    }
}