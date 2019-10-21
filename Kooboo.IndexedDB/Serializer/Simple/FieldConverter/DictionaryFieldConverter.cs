//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class DictionaryFieldConverter<T> : IFieldConverter<T>
    {
        private Type _keyType;
        private Type _valueType;
        private Type _dictionaryType;

        private int _keyLength;
        private int _valuelength;

        private Func<object, byte[]> _getKeyObjectBytes;
        private Func<byte[], object> _getKeyObjectValue;

        private Func<object, byte[]> _getValueObjectBytes;
        private Func<byte[], object> _getValueObjectValue;

        private Func<T, object> _getFieldValue;
        private Action<T, object> _setFieldValue;

        private string FieldName { get; set; }

        private bool IsIgnoreCase { get; set; }

        public DictionaryFieldConverter(Type dictionaryType, string fieldName)
        {
            this.IsIgnoreCase = Helper.TypeHelper.IsDictIgnoreCase(typeof(T), fieldName);

            this._dictionaryType = dictionaryType;
            _keyType = ObjectHelper.GetDictionaryKeyType(dictionaryType);
            _valueType = ObjectHelper.GetDictionaryValueType(dictionaryType);

            this.FieldName = fieldName;
            FieldNameHash = ObjectHelper.GetHashCode(fieldName);

            _keyLength = ConverterHelper.GetTypeLength(_keyType);
            _valuelength = ConverterHelper.GetTypeLength(_valueType);

            _getFieldValue = ObjectHelper.GetGetObjectValue<T>(fieldName);
            _setFieldValue = ObjectHelper.GetSetObjectValue<T>(fieldName, this._dictionaryType);

            _getKeyObjectBytes = ConverterHelper.GetValueToBytes(_keyType);
            _getKeyObjectValue = ConverterHelper.GetBytesToValue(_keyType);

            _getValueObjectBytes = ConverterHelper.GetValueToBytes(_valueType);
            _getValueObjectValue = ConverterHelper.GetBytesToValue(_valueType);

            if (_getKeyObjectBytes == null || _getKeyObjectValue == null)
            {
                throw new Exception(_keyType.Name + " is not yet supported.");
            }

            if (_getValueObjectBytes == null || _getValueObjectValue == null)
            {
                throw new Exception(_valueType.Name + " is not yet supported.");
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

            int keyByteLen = BitConverter.ToInt32(bytes, 0);
            int valueByteLen = BitConverter.ToInt32(bytes, 4);
            if (keyByteLen == 0) { return; }

            byte[] keyTotalBytes = new byte[keyByteLen];
            byte[] valueTotalBytes = new byte[valueByteLen];

            System.Buffer.BlockCopy(bytes, 8, keyTotalBytes, 0, keyByteLen);
            System.Buffer.BlockCopy(bytes, 8 + keyByteLen, valueTotalBytes, 0, valueByteLen);

            int keystartposition = 0;

            while (true)
            {
                if (this._keyLength > 0)
                {
                    byte[] onekeybytes = new byte[this._keyLength];
                    System.Buffer.BlockCopy(keyTotalBytes, keystartposition, onekeybytes, 0, this._keyLength);
                    keystartposition += this._keyLength;
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
                    {
                        keybytes.Add(null);
                    }
                }

                if (keystartposition >= keyByteLen)
                { break; }
            }

            int valuestartposition = 0;

            while (true)
            {
                if (this._valuelength > 0)
                {
                    byte[] onebytes = new byte[this._valuelength];
                    System.Buffer.BlockCopy(valueTotalBytes, valuestartposition, onebytes, 0, this._valuelength);
                    valuestartposition += this._valuelength;
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
                dict = Activator.CreateInstance(this._dictionaryType, para.ToArray()) as System.Collections.IDictionary;
            }
            else
            {
                dict = Activator.CreateInstance(this._dictionaryType) as System.Collections.IDictionary;
            }

            int count = keybytes.Count;

            for (int i = 0; i < count; i++)
            {
                var keybyte = keybytes[i];
                var valuebyte = valuebytes[i];
                if (keybyte != null)
                {
                    var dictkey = this._getKeyObjectValue(keybyte);
                    if (valuebyte == null)
                    {
                        dict.Add(dictkey, null);
                    }
                    else
                    {
                        var dictvalue = this._getValueObjectValue(valuebyte);
                        dict.Add(dictkey, dictvalue);
                    }
                }
                else if (_keyType == typeof(string))
                {
                    if (valuebyte == null)
                    {
                        dict.Add(string.Empty, null);
                    }
                    else
                    {
                        var dictvalue = this._getValueObjectValue(valuebyte);
                        dict.Add(string.Empty, dictvalue);
                    }
                }
            }

            this._setFieldValue(value, dict);
        }

        public byte[] ToBytes(T value)
        {
            var dictvale = _getFieldValue(value);

            var dict = dictvale as System.Collections.IDictionary;
            if (dict == null)
            {
                return null;
            }

            List<byte[]> keyresults = new List<byte[]>();
            int keytotallen = 0;

            foreach (var item in dict.Keys)
            {
                var keyresult = this._getKeyObjectBytes(item);

                if (this._keyLength > 0)
                {
                    keyresults.Add(keyresult);
                    keytotallen += this._keyLength;
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

            List<byte[]> valueResults = new List<byte[]>();
            int valueTotalLen = 0;

            foreach (var item in dict.Values)
            {
                var valueResult = this._getValueObjectBytes(item);

                if (this._valuelength > 0)
                {
                    valueResults.Add(valueResult);
                    valueTotalLen += this._valuelength;
                }
                else
                {
                    if (valueResult == null)
                    {
                        valueResults.Add(BitConverter.GetBytes(0));
                        //ValueResults.Add(ValueResult);
                        valueTotalLen += 4;
                    }
                    else
                    {
                        valueResults.Add(BitConverter.GetBytes(valueResult.Length));
                        valueResults.Add(valueResult);
                        valueTotalLen += 4 + valueResult.Length;
                    }
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

            int total = KeyBytes.Length + valueBytes.Length + 8;

            byte[] totalbytes = new byte[total];

            System.Buffer.BlockCopy(BitConverter.GetBytes(KeyBytes.Length), 0, totalbytes, 0, 4);
            System.Buffer.BlockCopy(BitConverter.GetBytes(valueBytes.Length), 0, totalbytes, 4, 4);
            if (KeyBytes.Length > 0)
            {
                System.Buffer.BlockCopy(KeyBytes, 0, totalbytes, 8, KeyBytes.Length);
            }
            if (valueBytes.Length > 0)
            {
                System.Buffer.BlockCopy(valueBytes, 0, totalbytes, 8 + KeyBytes.Length, valueBytes.Length);
            }
            return totalbytes;
        }
    }

    public class DictionaryFieldConverter : IFieldConverter
    {
        private Func<object, object> getValue;
        private Action<object, object> setValue;
        private DictionaryConverter converter;

        public DictionaryFieldConverter(string fieldName, Type objectType, Type dictionaryType)
        {
            this.getValue = ObjectHelper.GetGetObjectValue(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetObjectValue(fieldName, objectType, dictionaryType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
            converter = new DictionaryConverter(dictionaryType, Helper.TypeHelper.IsDictIgnoreCase(objectType, fieldName));
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

        public byte[] ToBytes(object value)
        {
            object fieldvalue = this.getValue(value);
            return converter.ToBytes(fieldvalue);
        }
    }
}