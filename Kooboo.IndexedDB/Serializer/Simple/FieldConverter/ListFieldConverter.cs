//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class ListFieldConverter<T> : IFieldConverter<T>
    {
        private Type DataType;
        private Type ListType;

        private int FieldLength;

        private Func<object, byte[]> GetObjectBytes;
        private Func<byte[], object> GetObjectValue;

        private Func<T, object> GetFieldValue;
        private Action<T, object> SetFieldValue;

        private string FieldName { get; set; }

        public ListFieldConverter(Type listType, string fieldName)
        {
            this.ListType = listType;
            this.DataType = ObjectHelper.GetEnumberableType(listType);
            this.FieldName = fieldName;
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
            this.FieldLength = ConverterHelper.GetTypeLength(this.DataType);

            this.GetFieldValue = ObjectHelper.GetGetObjectValue<T>(fieldName);
            this.SetFieldValue = ObjectHelper.GetSetObjectValue<T>(fieldName, this.ListType);

            this.GetObjectBytes = ConverterHelper.GetValueToBytes(this.DataType);
            this.GetObjectValue = ConverterHelper.GetBytesToValue(this.DataType);

            if (this.GetObjectBytes == null || this.GetObjectValue == null)
            {
                throw new Exception(this.DataType.Name + " is not yet supported.");
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
            var list = Activator.CreateInstance(this.ListType) as System.Collections.IList;

            int startposition = 0;
            int totallength = bytes.Length;

            while (true)
            {
                if (this.FieldLength > 0)
                {
                    byte[] fieldValueBytes = new byte[this.FieldLength];
                    System.Buffer.BlockCopy(bytes, startposition, fieldValueBytes, 0, this.FieldLength);
                    startposition += this.FieldLength;
                    var objectvalue = this.GetObjectValue(fieldValueBytes);
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
                        var objectvalue = this.GetObjectValue(fieldValueBytes);
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

            this.SetFieldValue(value, list);
        }

        public byte[] ToBytes(T value)
        {
            var listvalue = GetFieldValue(value);

            if (listvalue == null)
            {
                return null;
            }

            List<byte[]> results = new List<byte[]>();

            int totallen = 0;

            foreach (var item in (IEnumerable)listvalue)
            {
                var result = this.GetObjectBytes(item);

                if (this.FieldLength > 0)
                {
                    results.Add(result);
                    totallen += this.FieldLength;
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

    public class ListFieldConverter : IFieldConverter
    {
        private Func<object, object> getValue;
        private Action<object, object> setValue;

        private ListConverter converter;

        public ListFieldConverter(string fieldName, Type objectType, Type listType)
        {
            this.getValue = ObjectHelper.GetGetObjectValue(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetObjectValue(fieldName, objectType, listType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
            converter = new ListConverter(listType);
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