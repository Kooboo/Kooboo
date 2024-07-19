//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class IntFieldConverter<T> : IFieldConverter<T>
    {

        Func<T, int> getValue;
        Action<T, int> setValue;

        public IntFieldConverter(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, int>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, int>(FieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(T Value)
        {
            int fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }

    }

    public class IntFieldConverter : IFieldConverter
    {

        Func<object, int> getValue;
        Action<object, int> setValue;

        public IntFieldConverter(string FieldName, Type ObjectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<int>(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetFieldValue<int>(FieldName, ObjectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(object Value)
        {
            int fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }

    }
}
