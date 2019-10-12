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

        public ByteArrayFieldConverter(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, byte[]>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, byte[]>(FieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(T Value)
        {
            return this.getValue(Value);
        }
    }

    public class ByteArrayFieldConverter : IFieldConverter
    {
        private Func<object, byte[]> getValue;
        private Action<object, byte[]> setValue;

        public ByteArrayFieldConverter(string FieldName, Type ObjectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<byte[]>(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetFieldValue<byte[]>(FieldName, ObjectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(object Value)
        {
            return this.getValue(Value);
        }
    }
}