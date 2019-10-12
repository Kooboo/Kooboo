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

        public GuidFieldConverter(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, Guid>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, Guid>(FieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(T Value)
        {
            Guid fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class GuidFieldConverter : IFieldConverter
    {
        private Func<object, Guid> getValue;
        private Action<object, Guid> setValue;

        public GuidFieldConverter(string FieldName, Type ObjectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<Guid>(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetFieldValue<Guid>(FieldName, ObjectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(object Value)
        {
            Guid fieldvalue = this.getValue(Value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}