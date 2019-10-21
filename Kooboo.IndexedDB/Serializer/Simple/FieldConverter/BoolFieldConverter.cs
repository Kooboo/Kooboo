//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class BoolFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, bool> getValue;
        private Action<T, bool> setValue;

        public BoolFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, bool>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, bool>(fieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get
            {
                return 1;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(T value, byte[] bytes)
        {
            bool bytevalue = ValueConverter.ToBool(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T value)
        {
            bool fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class BoolFieldConverter : IFieldConverter
    {
        private Func<object, bool> getValue;
        private Action<object, bool> setValue;

        public BoolFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<bool>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<bool>(fieldName, objectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get
            {
                return 1;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(object value, byte[] bytes)
        {
            bool bytevalue = ValueConverter.ToBool(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object value)
        {
            bool fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}