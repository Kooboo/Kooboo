//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.Helper;


namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class DictionaryFieldConverter<T> : IFieldConverter<T>
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

        private Func<T, object> GetFieldValue;
        private Action<T, object> SetFieldValue;

        private string FieldName { get; set; }

        private bool IsIgnoreCase { get; set; }

        public DictionaryFieldConverter(Type DictionaryType, string FieldName)
        {
            this.IsIgnoreCase = Helper.TypeHelper.IsDictIgnoreCase(typeof(T), FieldName);

            this.DictionaryType = DictionaryType;
            KeyType = ObjectHelper.GetDictionaryKeyType(DictionaryType);
            ValueType = ObjectHelper.GetDictionaryValueType(DictionaryType);

            this.FieldName = FieldName;
            FieldNameHash = ObjectHelper.GetHashCode(FieldName);

            KeyLength = ConverterHelper.GetTypeLength(KeyType);
            Valuelength = ConverterHelper.GetTypeLength(ValueType);

            GetFieldValue = ObjectHelper.GetGetObjectValue<T>(FieldName);
            SetFieldValue = ObjectHelper.GetSetObjectValue<T>(FieldName, this.DictionaryType);

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

        public int FieldNameHash
        {
            get; set;
        }

        public int ByteLength
        {
            get
            {
                return 0;
            }
        }

        public void SetByteValues(T value, byte[] bytes)
        {
            List<byte[]> keybytes = new List<byte[]>();
            List<byte[]> valuebytes = new List<byte[]>();

            int KeyByteLen = BitConverter.ToInt32(bytes, 0);
            int ValueByteLen = BitConverter.ToInt32(bytes, 4);
            if (KeyByteLen == 0) { return; }

            byte[] KeyTotalBytes = new byte[KeyByteLen];
            byte[] ValueTotalBytes = new byte[ValueByteLen];

            System.Buffer.BlockCopy(bytes, 8, KeyTotalBytes, 0, KeyByteLen);
            System.Buffer.BlockCopy(bytes, 8 + KeyByteLen, ValueTotalBytes, 0, ValueByteLen);

            int keystartposition = 0;

            while (true)
            {
                if (this.KeyLength > 0)
                {
                    byte[] onekeybytes = new byte[this.KeyLength];
                    System.Buffer.BlockCopy(KeyTotalBytes, keystartposition, onekeybytes, 0, this.KeyLength);
                    keystartposition += this.KeyLength;
                    keybytes.Add(onekeybytes);
                }
                else
                {
                    int len = BitConverter.ToInt32(KeyTotalBytes, keystartposition);
                    keystartposition += 4;

                    if (len > 0)
                    {
                        byte[] onekeybytes = new byte[len];
                        System.Buffer.BlockCopy(KeyTotalBytes, keystartposition, onekeybytes, 0, len);
                        keystartposition += len;
                        keybytes.Add(onekeybytes);
                    }
                    else
                    {
                        keybytes.Add(null);
                    }
                }

                if (keystartposition >= KeyByteLen)
                { break; }
            }

            int valuestartposition = 0;

            while (true)
            {
                if (this.Valuelength > 0)
                {
                    byte[] onebytes = new byte[this.Valuelength];
                    System.Buffer.BlockCopy(ValueTotalBytes, valuestartposition, onebytes, 0, this.Valuelength);
                    valuestartposition += this.Valuelength;
                    valuebytes.Add(onebytes);
                }
                else
                {
                    int len = BitConverter.ToInt32(ValueTotalBytes, valuestartposition);
                    valuestartposition += 4;

                    if (len > 0)
                    {
                        byte[] onebytes = new byte[len];
                        System.Buffer.BlockCopy(ValueTotalBytes, valuestartposition, onebytes, 0, len);
                        valuestartposition += len;
                        valuebytes.Add(onebytes);
                    }
                    else
                    {
                        valuebytes.Add(null);
                    }
                }

                if (valuestartposition >= ValueByteLen)
                { break; }
            }


            System.Collections.IDictionary dict = null;
            if (this.IsIgnoreCase)
            {
                List<object> para = new List<object>();
                para.Add(StringComparer.OrdinalIgnoreCase);
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

            this.SetFieldValue(value, dict);
        }

        public byte[] ToBytes(T value)
        {
            var dictvale = GetFieldValue(value);

            var dict = dictvale as System.Collections.IDictionary;
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
                var ValueResult = this.GetValueObjectBytes(item);

                if (this.Valuelength > 0)
                {
                    ValueResults.Add(ValueResult);
                    ValueTotalLen += this.Valuelength;
                }
                else
                {
                    if (ValueResult == null)
                    {
                        ValueResults.Add(BitConverter.GetBytes(0));
                        //ValueResults.Add(ValueResult);
                        ValueTotalLen += 4;
                    }
                    else
                    {
                        ValueResults.Add(BitConverter.GetBytes(ValueResult.Length));
                        ValueResults.Add(ValueResult);
                        ValueTotalLen += 4 + ValueResult.Length;
                    }
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

    }

    public class DictionaryFieldConverter : IFieldConverter
    {
        Func<object, object> getValue;
        Action<object, object> setValue;
        DictionaryConverter converter;


        public DictionaryFieldConverter(string FieldName, Type ObjectType, Type DictionaryType)
        {
            this.getValue = ObjectHelper.GetGetObjectValue(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetObjectValue(FieldName, ObjectType, DictionaryType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
            converter = new DictionaryConverter(DictionaryType, Helper.TypeHelper.IsDictIgnoreCase(ObjectType, FieldName));
        }

        public int ByteLength
        {
            get
            {
                return 0;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(object value, byte[] bytes)
        {
            object fieldvalue = converter.FromBytes(bytes);
            setValue(value, fieldvalue);
        }

        public byte[] ToBytes(object Value)
        {
            object fieldvalue = this.getValue(Value);
            return converter.ToBytes(fieldvalue);
        }
    }

}
