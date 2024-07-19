//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{


    public class EnumFieldConveter<T> : IFieldConverter<T>
    {

        Func<T, object> getValue;
        Action<T, object> setValue;

        private Type EnumClassType { get; set; }
        public EnumFieldConveter(string FieldName, Type EnumClassType)
        {
            this.getValue = ObjectHelper.GetGetEnumObjectValue<T>(FieldName);
            this.setValue = ObjectHelper.GetSetObjectValue<T>(FieldName, EnumClassType);
            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
            this.EnumClassType = EnumClassType;

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

        public byte[] ToBytes(T Value)
        {
            object fieldvalue = this.getValue(Value);
            int enumvalue = (int)fieldvalue;
            return ValueConverter.ToBytes(enumvalue);
        }

    }

    public class EnumFieldConveter : IFieldConverter
    {

        Func<object, object> getValue;
        Action<object, object> setValue;

        private Type EnumType { get; set; }

        public EnumFieldConveter(string FieldName, Type ObjectType, Type EnumFieldType)
        {
            this.getValue = ObjectHelper.GetGetObjectValue(FieldName, ObjectType);
            this.setValue = ObjectHelper.GetSetObjectValue(FieldName, ObjectType, EnumFieldType);

            this.FieldNameHash = ObjectHelper.GetHashCode(FieldName);
            this.EnumType = EnumFieldType;
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

        public byte[] ToBytes(object Value)
        {
            object fieldvalue = this.getValue(Value);
            int enumvalue = (int)fieldvalue;
            return ValueConverter.ToBytes(enumvalue);
        }

    }

}
