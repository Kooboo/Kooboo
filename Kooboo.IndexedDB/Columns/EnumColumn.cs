//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Helper;
using Kooboo.IndexedDB.Serializer.Simple;

namespace Kooboo.IndexedDB.Columns
{
    public class EnumColumn<TValue> : IColumn<TValue>
    {

        internal Func<TValue, object> Get;
        internal Action<TValue, object> Set;

        IByteConverter<Int32> byteConverter;

        /// <summary>
        ///  The enum class type. 
        /// </summary>
        public Type DataType
        {
            get; set;
        }

        public EnumColumn(string FieldName, Type EnumClassType)
        {
            this.FieldName = FieldName;
            this.Get = ObjectHelper.GetGetEnumObjectValue<TValue>(FieldName);
            this.Set = ObjectHelper.GetSetObjectValue<TValue>(FieldName, EnumClassType);

            byteConverter = ObjectContainer.GetConverter<Int32>();

            this.DataType = EnumClassType;

            this.FieldNameHash = Helper.ObjectHelper.GetHashCode(this.FieldName);
            var fieldnamehashbytes = BitConverter.GetBytes(FieldNameHash);

            this.IsLenVaries = false;

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

        public int RelativePosition
        {
            get;
            set;
        }

        public bool IsLenVaries { get; set; }

        public byte[] FieldNameLengthBytes { get; set; }
        public int FieldNameHash { get; set; }
    }
}
