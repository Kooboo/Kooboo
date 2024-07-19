//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class StringFieldConverter<T> : IFieldConverter<T>
    {
        Func<T, string> getValue;
        Action<T, string> setValue;

        public StringFieldConverter(string FieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, string>(FieldName);
            this.setValue = ObjectHelper.GetSetValue<T, string>(FieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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
        Func<object, string> getValue;
        Action<object, string> setValue;

        public StringFieldConverter(string FieldName, Type ObjectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<string>(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetFieldValue<string>(FieldName, ObjectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
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

        public byte[] ToBytes(object Value)
        {
            string fieldvalue = this.getValue(Value);

            if (string.IsNullOrEmpty(fieldvalue))
            {
                return null;
            }
            return GlobalSettings.DefaultEncoding.GetBytes(fieldvalue);
        }

    }

}
