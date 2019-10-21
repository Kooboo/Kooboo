//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class ShortFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, short> getValue;
        private Action<T, short> setValue;

        public ShortFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, short>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, short>(fieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get
            {
                return 2;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(T value, byte[] bytes)
        {
            short bytevalue = BitConverter.ToInt16(bytes, 0);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T value)
        {
            short fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }

    public class ShortFieldConverter : IFieldConverter
    {
        private Func<object, short> getValue;
        private Action<object, short> setValue;

        public ShortFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<short>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<short>(fieldName, objectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get
            {
                return 2;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(object value, byte[] bytes)
        {
            short bytevalue = BitConverter.ToInt16(bytes, 0);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object value)
        {
            short fieldvalue = this.getValue(value);
            return ValueConverter.ToBytes(fieldvalue);
        }
    }
}