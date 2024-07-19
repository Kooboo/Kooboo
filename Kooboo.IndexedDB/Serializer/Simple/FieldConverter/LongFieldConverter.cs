//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class LongFieldConverter<T> : IFieldConverter<T>
    {

        Func<T, long> getValue;
        Action<T, long> setValue;

        public LongFieldConverter(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, long>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, long>(FieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(T Value)
        {
            long fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }

    }

    public class LongFieldConverter : IFieldConverter
    {

        Func<object, long> getValue;
        Action<object, long> setValue;

        public LongFieldConverter(string FieldName, Type ObjectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<long>(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetFieldValue<long>(FieldName, ObjectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(object Value)
        {
            long fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

}
