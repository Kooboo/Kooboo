//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Helper;
using Kooboo.IndexedDB.Serializer.Simple;
using System;

namespace Kooboo.IndexedDB.Columns
{
    public class EnumColumn<TValue> : IColumn<TValue>
    {
        internal Func<TValue, object> Get;
        internal Action<TValue, object> Set;

        private IByteConverter<Int32> byteConverter;

        /// <summary>
        ///  The enum class type.
        /// </summary>
        public Type DataType
        {
            get; set;
        }

        public EnumColumn(string fieldName, Type enumClassType)
        {
            this.FieldName = fieldName;
            this.Get = ObjectHelper.GetGetEnumObjectValue<TValue>(fieldName);
            this.Set = ObjectHelper.GetSetObjectValue<TValue>(fieldName, enumClassType);

            byteConverter = ObjectContainer.GetConverter<Int32>();

            this.DataType = enumClassType;

            this.FieldNameHash = Helper.ObjectHelper.GetHashCode(this.FieldName);
            var fieldnamehashbytes = BitConverter.GetBytes(FieldNameHash);

            this.IsString = false;

            this.FieldNameLengthBytes = new byte[8];
            var lenbytes = BitConverter.GetBytes(this.Length);
            System.Buffer.BlockCopy(fieldnamehashbytes, 0, this.FieldNameLengthBytes, 0, 4);
            System.Buffer.BlockCopy(lenbytes, 0, this.FieldNameLengthBytes, 4, 4);
        }

        public byte[] GetBytes(TValue input)
        {
            object fieldvalue = this.Get(input);
            int enumvalue = (int)fieldvalue;
            return ValueConverter.ToBytes(enumvalue);
        }

        public void SetBytes(TValue input, byte[] bytes)
        {
            if (this.Set != null)
            {
                int bytevalue = BitConverter.ToInt32(bytes, 0);
                var enumobject = System.Enum.ToObject(this.DataType, bytevalue);
                this.Set(input, bytevalue);
            }
        }

        public string FieldName
        {
            get;
            set;
        }

        public int Length
        {
            get
            {
                return 4;
            }
            set
            {
            }
        }

        public int relativePosition
        {
            get;
            set;
        }

        public bool IsString { get; set; }

        public byte[] FieldNameLengthBytes { get; set; }
        public int FieldNameHash { get; set; }
    }
}