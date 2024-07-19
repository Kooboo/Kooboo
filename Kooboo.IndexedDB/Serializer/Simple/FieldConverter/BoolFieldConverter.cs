//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class BoolFieldConverter<T> : IFieldConverter<T>
    {
        Func<T, bool> getValue;
        Action<T, bool> setValue;

        public BoolFieldConverter(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, bool>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, bool>(FieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(T Value)
        {
            bool fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class BoolFieldConverter : IFieldConverter
    {
        Func<object, bool> getValue;
        Action<object, bool> setValue;

        public BoolFieldConverter(string FieldName, Type ObjectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<bool>(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetFieldValue<bool>(FieldName, ObjectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(object Value)
        {
            bool fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }

    }
}
