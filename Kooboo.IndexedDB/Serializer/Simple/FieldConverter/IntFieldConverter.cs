//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class IntFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, int> getValue;
        private Action<T, int> setValue;

        public IntFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, int>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, int>(fieldName);
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
            int bytevalue = BitConverter.ToInt32(bytes, 0);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T value)
        {
            int fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class IntFieldConverter : IFieldConverter
    {
        private Func<object, int> getValue;
        private Action<object, int> setValue;

        public IntFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<int>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<int>(fieldName, objectType);
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
            int bytevalue = BitConverter.ToInt32(bytes, 0);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object value)
        {
            int fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}