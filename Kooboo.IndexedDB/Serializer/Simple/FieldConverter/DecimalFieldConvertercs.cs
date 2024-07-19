//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class DecimalFieldConvertercs<T> : IFieldConverter<T>
    {

        Func<T, decimal> getValue;
        Action<T, decimal> setValue;

        public DecimalFieldConvertercs(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, decimal>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, decimal>(FieldName);
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
            decimal bytevalue = ValueConverter.ToDecimal(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T Value)
        {
            decimal fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }

    }


    public class DecimalFieldConverter : IFieldConverter
    {
        Func<object, decimal> getValue;
        Action<object, decimal> setValue;

        public DecimalFieldConverter(string FieldName, Type ObjectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<decimal>(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetFieldValue<decimal>(FieldName, ObjectType);
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
            decimal bytevalue = ValueConverter.ToDecimal(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object Value)
        {
            decimal fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}
