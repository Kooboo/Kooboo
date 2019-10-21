//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class ObjectFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, object> getValue;
        private Action<T, object> setValue;

        public ObjectFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, object>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, object>(fieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
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
            object bytevalue = ValueConverter.FromObjectBytes(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T value)
        {
            object fieldvalue = this.getValue(value);
            return ValueConverter.ObjectToTypes(fieldvalue);
        }
    }

    public class ObjectFieldConverter : IFieldConverter
    {
        private Func<object, object> getValue;
        private Action<object, object> setValue;

        public ObjectFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<object>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<object>(fieldName, objectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
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
            object bytevalue = ValueConverter.FromObjectBytes(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object value)
        {
            object fieldvalue = this.getValue(value);
            return ValueConverter.ObjectToTypes(fieldvalue);
        }
    }
}