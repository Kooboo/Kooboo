//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class FloatFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, float> getValue;
        private Action<T, float> setValue;

        public FloatFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, float>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, float>(fieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
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

        public byte[] ToBytes(T value)
        {
            float fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class FloatFieldConverter : IFieldConverter
    {
        private Func<object, float> getValue;
        private Action<object, float> setValue;

        public FloatFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<float>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<float>(fieldName, objectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
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

        public byte[] ToBytes(object value)
        {
            float fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}