//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class CollectionFieldConverter<T> : IFieldConverter<T>
    {

        private Type DataType;
        private Type CollectionType;

        private int FieldLength;

        private Func<object, byte[]> GetObjectBytes;
        private Func<byte[], object> GetObjectValue;

        private Func<T, object> GetFieldValue;
        private Action<T, object> SetFieldValue;

        private string FieldName { get; set; }

        public CollectionFieldConverter(Type CollectionType, string FieldName)
        {
            this.CollectionType = CollectionType;
            this.DataType = ObjectHelper.GetEnumberableType(CollectionType);
            this.FieldName = FieldName;
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
            this.FieldLength = ConverterHelper.GetTypeLength(this.DataType);

            this.GetFieldValue = ObjectHelper.GetGetObjectValue<T>(FieldName);
            this.SetFieldValue = ObjectHelper.GetSetObjectValue<T>(FieldName, this.CollectionType);

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
            var GenericHashSet = typeof(CollectionWrapper<>).MakeGenericType(DataType);

            var OriginalInstance = Activator.CreateInstance(this.CollectionType);

            var list = Activator.CreateInstance(GenericHashSet, OriginalInstance) as System.Collections.IList;

            int startposition = 0;
            int totallength = bytes.Length;

            while (true)
            {
                if (this.FieldLength > 0)
                {

                    byte[] FieldValueBytes = new byte[this.FieldLength];
                    System.Buffer.BlockCopy(bytes, startposition, FieldValueBytes, 0, this.FieldLength);
                    startposition += this.FieldLength;
                    var objectvalue = this.GetObjectValue(FieldValueBytes);
                    list.Add(objectvalue);
                }
                else
                {
                    int len = BitConverter.ToInt32(bytes, startposition);
                    startposition += 4;

                    if (len > 0)
                    {
                        byte[] FieldValueBytes = new byte[len];
                        System.Buffer.BlockCopy(bytes, startposition, FieldValueBytes, 0, len);
                        startposition += len;
                        var objectvalue = this.GetObjectValue(FieldValueBytes);
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

            this.SetFieldValue(value, OriginalInstance);

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
                        totallen += 4;
                    }
                }
            }

            byte[] BackValue = new byte[totallen];
            int currentposition = 0;

            foreach (var item in results)
            {
                int len = item.Length;
                System.Buffer.BlockCopy(item, 0, BackValue, currentposition, len);
                currentposition += len;
            }

            return BackValue;

        }

    }

    public class CollectionFieldConverter : IFieldConverter
    {

        private Type DataType;
        private Type CollectionType;

        private int FieldLength;

        private CollectionConverter converter;

        private Func<object, object> GetFieldValue;
        private Action<object, object> SetFieldValue;

        private string FieldName { get; set; }

        public CollectionFieldConverter(Type ObjectType, Type CollectionType, string FieldName)
        {
            this.CollectionType = CollectionType;
            this.DataType = ObjectHelper.GetEnumberableType(CollectionType);
            this.FieldName = FieldName;
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
            this.FieldLength = ConverterHelper.GetTypeLength(this.DataType);

            this.GetFieldValue = ObjectHelper.GetGetObjectValue(FieldName, ObjectType);
            this.SetFieldValue = ObjectHelper.GetSetObjectValue(FieldName, ObjectType, this.CollectionType);

            this.converter = new CollectionConverter(this.CollectionType);

            if (this.converter == null)
            {
                throw new Exception(this.CollectionType.Name + " is not yet supported.");
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

        public void SetByteValues(object value, byte[] bytes)
        {
            object fieldvalue = converter.FromBytes(bytes);
            SetFieldValue(value, fieldvalue);
        }

        public byte[] ToBytes(object value)
        {
            object fieldvalue = this.GetFieldValue(value);
            return converter.ToBytes(fieldvalue);
        }
    }

}
