//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class DateTimeFieldConverter<T> : IFieldConverter<T>
    {
        Func<T, DateTime> getValue;
        Action<T, DateTime> setValue;

        public DateTimeFieldConverter(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, DateTime>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, DateTime>(FieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(T Value)
        {
            DateTime fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class DateTimeFieldConverter : IFieldConverter
    {
        Func<object, DateTime> getValue;
        Action<object, DateTime> setValue;

        public DateTimeFieldConverter(string FieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<DateTime>(FieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<DateTime>(FieldName, objectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(object Value)
        {
            DateTime fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}
