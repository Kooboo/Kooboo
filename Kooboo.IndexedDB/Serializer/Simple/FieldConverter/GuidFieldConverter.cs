//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class GuidFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, Guid> getValue;
        private Action<T, Guid> setValue;

        public GuidFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, Guid>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, Guid>(fieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get { return 16; }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(T value, byte[] bytes)
        {
            Guid bytevalue = ValueConverter.ToGuid(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T value)
        {
            Guid fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class GuidFieldConverter : IFieldConverter
    {
        private Func<object, Guid> getValue;
        private Action<object, Guid> setValue;

        public GuidFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<Guid>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<Guid>(fieldName, objectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get { return 16; }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(object value, byte[] bytes)
        {
            Guid bytevalue = ValueConverter.ToGuid(bytes);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object value)
        {
            Guid fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}