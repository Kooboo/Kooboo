//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class DoubleFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, double> getValue;
        private Action<T, double> setValue;

        public DoubleFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, double>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, double>(fieldName);
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
            double bytevalue = BitConverter.ToDouble(bytes, 0);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T value)
        {
            double fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class DoubleFieldConverter : IFieldConverter
    {
        private Func<object, double> getValue;
        private Action<object, double> setValue;

        public DoubleFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<double>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<double>(fieldName, objectType);
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
            double bytevalue = BitConverter.ToDouble(bytes, 0);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object value)
        {
            double fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}