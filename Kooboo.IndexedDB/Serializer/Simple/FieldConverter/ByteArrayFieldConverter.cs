//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class ByteArrayFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, byte[]> getValue;
        private Action<T, byte[]> setValue;

        public ByteArrayFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, byte[]>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, byte[]>(fieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
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

        public void SetByteValues(T value, byte[] bytes)
        {
            this.setValue(value, bytes);
        }

        public byte[] ToBytes(T value)
        {
            return this.getValue(value);
        }
    }

    public class ByteArrayFieldConverter : IFieldConverter
    {
        private Func<object, byte[]> getValue;
        private Action<object, byte[]> setValue;

        public ByteArrayFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<byte[]>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<byte[]>(fieldName, objectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
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
            this.setValue(value, bytes);
        }

        public byte[] ToBytes(object value)
        {
            return this.getValue(value);
        }
    }
}