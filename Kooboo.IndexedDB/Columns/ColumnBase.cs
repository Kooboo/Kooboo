//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.IndexedDB.ByteConverter;

namespace Kooboo.IndexedDB.Columns
{
    public class ColumnBase<TValue, TColumn> : IColumn<TValue>
    {
        public Func<TValue, TColumn> Get;
        public Action<TValue, TColumn> Set;
        protected virtual IByteConverter<TColumn> ByteConverter { get; set; }

        private Type _datatype;
        public Type DataType
        {
            get
            {
                if (_datatype == null)
                {
                    _datatype = typeof(TColumn);
                }
                return _datatype;
            }
            set { _datatype = value; }
        }

        public ColumnBase(string FieldName, int len)
        {
            init(FieldName, len);
        }

        public ColumnBase(string FieldName)
        {
            int len;
            if (this.DataType == typeof(string))
            {
                len = 50;
            }
            else
            {
                len = Helper.KeyHelper.GetKeyLen(this.DataType);
            }
            init(FieldName, len);
        }

        private void init(string FieldName, int len)
        {
            if (len <= 0)
            {
                throw new Exception("Len must be at least 1");
            }

            this.FieldName = FieldName;
            this.Length = len;

            this.FieldNameHash = Helper.ObjectHelper.GetHashCode(this.FieldName);
            var fieldnamehashbytes = BitConverter.GetBytes(FieldNameHash);

            this.Get = Helper.ObjectHelper.GetGetValue<TValue, TColumn>(FieldName);
            this.Set = Helper.ObjectHelper.GetSetValue<TValue, TColumn>(FieldName);
            ByteConverter = ObjectContainer.GetConverter<TColumn>();

            this.IsLenVaries = Helper.KeyHelper.IsKeyLenVar(this.DataType);

            this.FieldNameLengthBytes = new byte[8];
            var lenbytes = BitConverter.GetBytes(this.Length);
            System.Buffer.BlockCopy(fieldnamehashbytes, 0, this.FieldNameLengthBytes, 0, 4);
            System.Buffer.BlockCopy(lenbytes, 0, this.FieldNameLengthBytes, 4, 4);
        }

        public string FieldName
        {
            get;
            set;
        }

        public byte[] GetBytes(TValue input)
        {
            if (this.Get != null)
            {
                var value = this.Get(input);
                if (value != null)
                {
                    var bytes = ByteConverter.ToByte(Get(input));

                    if (this.IsLenVaries)
                    {
                        return Helper.KeyHelper.AppendToKeyLength(bytes, true, this.Length);
                    }
                    else
                    {
                        return bytes;
                    }
                }
            }
            return new byte[this.Length];
        }

        public void SetBytes(TValue input, byte[] bytes)
        {
            if (this.Set != null)
            {
                var value = ByteConverter.FromByte(bytes);
                Set(input, value);
            }
        }

        public int Length { get; set; }

        public int RelativePosition { get; set; }
        public byte[] FieldNameLengthBytes
        {
            get; set;
        }
        public int FieldNameHash { get; set; }
        public bool IsLenVaries { get; set; }
    }
}
