//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class FloatFieldConverter<T> : IFieldConverter<T>
    {
        Func<T, float> getValue;
        Action<T, float> setValue;

        public FloatFieldConverter(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, float>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, float>(FieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
        }

        public int ByteLength
        {
            get { return 4; }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(T value, byte[] bytes)
        {
            float bytevalue = BitConverter.ToSingle(bytes, 0);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T Value)
        {
            float fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }

    }

    public class FloatFieldConverter : IFieldConverter
    {
        Func<object, float> getValue;
        Action<object, float> setValue;

        public FloatFieldConverter(string FieldName, Type ObjectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<float>(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetFieldValue<float>(FieldName, ObjectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
        }

        public int ByteLength
        {
            get { return 4; }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(object value, byte[] bytes)
        {
            float bytevalue = BitConverter.ToSingle(bytes, 0);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object Value)
        {
            float fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }

    }
}
