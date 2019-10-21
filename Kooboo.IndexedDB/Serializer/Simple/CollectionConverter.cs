//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Serializer.Simple
{
    public class CollectionConverter
    {
        private Type _dataType;
        private Type _collectionType;

        private int _fieldLength;

        private Func<object, byte[]> _getObjectBytes;
        private Func<byte[], object> _getObjectValue;

        public CollectionConverter(Type collectionType)
        {
            this._collectionType = collectionType;
            this._dataType = ObjectHelper.GetEnumberableType(collectionType);

            this._fieldLength = ConverterHelper.GetTypeLength(this._dataType);

            this._getObjectBytes = ConverterHelper.GetValueToBytes(this._dataType);
            this._getObjectValue = ConverterHelper.GetBytesToValue(this._dataType);

            if (this._getObjectBytes == null || this._getObjectValue == null)
            {
                throw new Exception(this._dataType.Name + " is not yet supported.");
            }
        }

        public object FromBytes(byte[] bytes)
        {
            var genericHashSet = typeof(CollectionWrapper<>).MakeGenericType(_dataType);

            var originalInstance = Activator.CreateInstance(this._collectionType);

            var list = Activator.CreateInstance(genericHashSet, originalInstance) as System.Collections.IList;

            int startposition = 0;
            int totallength = bytes.Length;

            while (true)
            {
                if (this._fieldLength > 0)
                {
                    byte[] fieldValueBytes = new byte[this._fieldLength];
                    System.Buffer.BlockCopy(bytes, startposition, fieldValueBytes, 0, this._fieldLength);
                    startposition += this._fieldLength;

                    var objectvalue = this._getObjectValue(fieldValueBytes);

                    list.Add(objectvalue);
                }
                else
                {
                    int len = BitConverter.ToInt32(bytes, startposition);
                    startposition += 4;

                    if (len > 0)
                    {
                        byte[] fieldValueBytes = new byte[len];
                        System.Buffer.BlockCopy(bytes, startposition, fieldValueBytes, 0, len);
                        startposition += len;
                        var objectvalue = this._getObjectValue(fieldValueBytes);
                        list.Add(objectvalue);
                    }
                    else
                    {
                        list.Add(null);
                    }
                }

                if (startposition >= totallength)
                { break; }
            }

            return originalInstance;
        }

        public byte[] ToBytes(object value)
        {
            if (value == null)
            {
                return null;
            }

            List<byte[]> results = new List<byte[]>();

            int totallen = 0;

            foreach (var item in (IEnumerable)value)
            {
                var result = this._getObjectBytes(item);

                if (this._fieldLength > 0)
                {
                    results.Add(result);
                    totallen += this._fieldLength;
                }
                else
                {
                    if (result != null)
                    {
                        results.Add(BitConverter.GetBytes(result.Length));
                        results.Add(result);
                        totallen += 4 + result.Length;
                    }
                    else
                    {
                        results.Add(BitConverter.GetBytes(0));
                        totallen += 4 + result.Length;
                    }
                }
            }

            byte[] backValue = new byte[totallen];
            int currentposition = 0;

            foreach (var item in results)
            {
                int len = item.Length;
                System.Buffer.BlockCopy(item, 0, backValue, currentposition, len);
                currentposition += len;
            }

            return backValue;
        }
    }
}