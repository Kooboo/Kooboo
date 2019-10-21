//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class LongFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, long> getValue;
        private Action<T, long> setValue;

        public LongFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, long>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, long>(fieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get
            {
                return 8;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(T value, byte[] bytes)
        {
            if (bytes.Length >= 8)
            {
                long bytevalue = BitConverter.ToInt64(bytes, 0);
                this.setValue(value, bytevalue);
            }
            else
            {
                if (bytes.Length == 4)
                {
                    long bytevalue = (long)BitConverter.ToInt32(bytes, 0);
                    this.setValue(value, bytevalue);
                }
                else if (bytes.Length == 2)
                {
                    long bytevalue = (long)BitConverter.ToInt16(bytes, 0);
                    this.setValue(value, bytevalue);
                }
            }
        }

        public byte[] ToBytes(T value)
        {
            long fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class LongFieldConverter : IFieldConverter
    {
        private Func<object, long> getValue;
        private Action<object, long> setValue;

        public LongFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<long>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<long>(fieldName, objectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get
            {
                return 8;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(object value, byte[] bytes)
        {
            long bytevalue = BitConverter.ToInt64(bytes, 0);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object value)
        {
            long fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}