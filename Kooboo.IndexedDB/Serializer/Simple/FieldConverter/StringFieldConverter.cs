//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class StringFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, string> getValue;
        private Action<T, string> setValue;

        public StringFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, string>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, string>(fieldName);
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
            string fieldvalue = GlobalSettings.DefaultEncoding.GetString(bytes).TrimEnd('\0');
            setValue(value, fieldvalue);
        }

        public byte[] ToBytes(T value)
        {
            string fieldvalue = this.getValue(value);

            if (string.IsNullOrEmpty(fieldvalue))
            {
                return null;
            }
            return GlobalSettings.DefaultEncoding.GetBytes(fieldvalue);
        }
    }

    public class StringFieldConverter : IFieldConverter
    {
        private Func<object, string> getValue;
        private Action<object, string> setValue;

        public StringFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<string>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<string>(fieldName, objectType);
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
            string fieldvalue = GlobalSettings.DefaultEncoding.GetString(bytes).TrimEnd('\0');
            setValue(value, fieldvalue);
        }

        public byte[] ToBytes(object value)
        {
            string fieldvalue = this.getValue(value);

            if (string.IsNullOrEmpty(fieldvalue))
            {
                return null;
            }
            return GlobalSettings.DefaultEncoding.GetBytes(fieldvalue);
        }
    }
}