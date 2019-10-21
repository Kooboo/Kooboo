//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class DecimalFieldConvertercs<T> : IFieldConverter<T>
    {
        private Func<T, decimal> getValue;
        private Action<T, decimal> setValue;

        public DecimalFieldConvertercs(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, decimal>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, decimal>(fieldName);
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
            decimal bytevalue = ValueConverter.ToDecimal(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T value)
        {
            decimal fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class DecimalFieldConverter : IFieldConverter
    {
        private Func<object, decimal> getValue;
        private Action<object, decimal> setValue;

        public DecimalFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<decimal>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<decimal>(fieldName, objectType);
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
            decimal bytevalue = ValueConverter.ToDecimal(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object value)
        {
            decimal fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}