//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class EnumFieldConveter<T> : IFieldConverter<T>
    {
        private Func<T, object> getValue;
        private Action<T, object> setValue;

        private Type EnumClassType { get; set; }

        public EnumFieldConveter(string fieldName, Type enumClassType)
        {
            this.getValue = ObjectHelper.GetGetEnumObjectValue<T>(fieldName);
            this.setValue = ObjectHelper.GetSetObjectValue<T>(fieldName, enumClassType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
            this.EnumClassType = enumClassType;
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
            var enumobject = System.Enum.ToObject(this.EnumClassType, bytevalue);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(T value)
        {
            object fieldvalue = this.getValue(value);
            int enumvalue = (int)fieldvalue;
            return ValueConverter.ToBytes(enumvalue);
        }
    }

    public class EnumFieldConveter : IFieldConverter
    {
        private Func<object, object> getValue;
        private Action<object, object> setValue;

        private Type EnumType { get; set; }

        public EnumFieldConveter(string fieldName, Type objectType, Type enumFieldType)
        {
            this.getValue = ObjectHelper.GetGetObjectValue(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetObjectValue(fieldName, objectType, enumFieldType);

            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
            this.EnumType = enumFieldType;
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
            var enumobject = System.Enum.ToObject(this.EnumType, bytevalue);
            this.setValue(value, bytevalue);
        }

        public byte[] ToBytes(object value)
        {
            object fieldvalue = this.getValue(value);
            int enumvalue = (int)fieldvalue;
            return ValueConverter.ToBytes(enumvalue);
        }
    }
}