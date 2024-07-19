//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class DoubleFieldConverter<T> : IFieldConverter<T>
    {

        Func<T, double> getValue;
        Action<T, double> setValue;

        public DoubleFieldConverter(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, double>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, double>(FieldName);
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
            double bytevalue = BitConverter.ToDouble(bytes, 0);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T Value)
        {
            double fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }

    }

    public class DoubleFieldConverter : IFieldConverter
    {
        Func<object, double> getValue;
        Action<object, double> setValue;

        public DoubleFieldConverter(string FieldName, Type ObjectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<double>(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetFieldValue<double>(FieldName, ObjectType);
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
            double bytevalue = BitConverter.ToDouble(bytes, 0);
            this.setValue(value, bytevalue);
        }


        public byte[] ToBytes(object Value)
        {
            double fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }

    }
}
