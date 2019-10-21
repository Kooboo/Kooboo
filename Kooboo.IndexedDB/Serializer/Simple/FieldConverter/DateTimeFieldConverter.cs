//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class DateTimeFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, DateTime> getValue;
        private Action<T, DateTime> setValue;

        public DateTimeFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, DateTime>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, DateTime>(fieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get { return 8; }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(T value, byte[] bytes)
        {
            DateTime bytevalue = ValueConverter.ToDateTime(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T value)
        {
            DateTime fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class DateTimeFieldConverter : IFieldConverter
    {
        private Func<object, DateTime> getValue;
        private Action<object, DateTime> setValue;

        public DateTimeFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<DateTime>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<DateTime>(fieldName, objectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get { return 8; }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(object value, byte[] bytes)
        {
            DateTime bytevalue = ValueConverter.ToDateTime(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object value)
        {
            DateTime fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}