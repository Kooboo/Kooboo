//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB.Columns;
using Kooboo.IndexedDB.Helper;
using System;
using System.Collections.Generic;
using System.Collections;


namespace Kooboo.IndexedDB.Serializer.Simple
{
    /// <summary>
    /// a type of complext type that contains multiple fields or properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleConverter<T>
    {
        public SimpleConverter()
        {
            init();
        }

        private int BytePartsCount { get; set; }

        public int ColumnByteLen { get; set; }

        public SimpleConverter(Dictionary<string, int> columns)
        {
            init(columns);
        }

        private void init(Dictionary<string, int> columns = null)
        {
            this.BytePartsCount = 0;
            Type type = typeof(T);
            if (type.IsAbstract || type.IsInterface)
            {
                throw new Exception("Abstract and Interface can not be initialized.");
            }

            var allfields = TypeHelper.GetPublicPropertyOrFields(type);

            if (columns == null || columns.Count == 0)
            {
                foreach (var item in allfields)
                {
                    AddFields(item.Value, item.Key);
                }
            }
            else
            {
                this.Columns = new List<IColumn<T>>();

                int relativepos = 0;

                foreach (var item in allfields)
                {
                    if (columns.ContainsKey(item.Key))
                    {
                        var collen = columns[item.Key];
                        IColumn<T> created = Helper.ColumnHelper.GetColumn<T>(item.Key, item.Value, collen);
                        created.relativePosition = relativepos;
                        relativepos += created.Length + 8; // 4 bytes for fieldnamehash and 4 bytes for lenmark. 
                        this.Columns.Add(created);
                    }
                    else
                    {
                        AddFields(item.Value, item.Key);
                    }
                }
            }

            this.BytePartsCount = CalculateBytePartsCount();

            if (this.Columns != null)
            {
                this.ColumnByteLen = 0;
                foreach (var item in this.Columns)
                {
                    this.ColumnByteLen += item.Length + 8;
                }
            }
        }

        private int CalculateBytePartsCount()
        {
            int count = 0;
            if (this.Columns != null)
            {
                count += this.Columns.Count * 2;
            }

            if (this.Fields != null)
            {
                count += this.Fields.Count * 3;
            }
            return count;
        }

        private void AddFields(Type valuetype, string FieldName)
        {
            var converter = ConverterHelper.GetFieldConverter<T>(valuetype, FieldName);

            if (converter != null)
            {
                Fields.Add(converter);
            }
        }

        public byte[] ToBytes(T value)
        {
            List<byte[]> Results = new List<byte[]>();

            byte[][] byteArray = new byte[this.BytePartsCount][];
            int index = 0;

            int TotalLength = 0;

            if (this.Columns != null)
            {
                foreach (var item in this.Columns)
                {
                    byte[] result = item.GetBytes(value);

                    byteArray[index] = item.FieldNameLengthBytes;
                    index += 1;
                    byteArray[index] = result;
                    index += 1;
                    TotalLength += item.Length + 8;
                }
            }

            foreach (var item in this.Fields)
            {
                byte[] result = item.ToBytes(value);
                if (result == null || result.Length == 0)
                {
                    continue;
                }

                byteArray[index] = BitConverter.GetBytes(item.FieldNameHash);
                index += 1;

                int bytelen = item.ByteLength;
                if (bytelen == 0)
                {
                    bytelen = result.Length;
                }

                byteArray[index] = BitConverter.GetBytes(bytelen);
                index += 1;

                byteArray[index] = result;
                index += 1;
                TotalLength += result.Length + 8;
            }

            byte[] BackValue = new byte[TotalLength];
            int currentposition = 0;

            foreach (var item in byteArray)
            {
                if (item != null)
                {
                    int len = item.Length;
                    System.Buffer.BlockCopy(item, 0, BackValue, currentposition, len);
                    currentposition += len;
                }
            }
            return BackValue;
        }

        private void SetByteValues(T value, byte[] bytes)
        {
            int startposition = 0;
            int totallength = bytes.Length;
            if (totallength == 0)
            {
                return;
            }
            while (true)
            {
                int FieldNameHash = BitConverter.ToInt32(bytes, startposition);

                startposition += 4;

                int len = BitConverter.ToInt32(bytes, startposition);
                startposition += 4;

                if (len <= 0)
                {
                    break;
                }

                var item = Fields.Find(o => o.FieldNameHash == FieldNameHash);
                if (item != null)
                {
                    byte[] FieldValueBytes = new byte[len];
                    if (totallength >= startposition + len)
                    {
                        System.Buffer.BlockCopy(bytes, startposition, FieldValueBytes, 0, len);
                        item.SetByteValues(value, FieldValueBytes);
                    }
                }
                else
                {
                    if (this.Columns != null)
                    {
                        var col = this.Columns.Find(o => o.FieldNameHash == FieldNameHash);

                        if (col != null)
                        {
                            byte[] FieldValueBytes = new byte[len];
                            if (totallength >= startposition + len)
                            {
                                System.Buffer.BlockCopy(bytes, startposition, FieldValueBytes, 0, len);
                                col.SetBytes(value, FieldValueBytes);
                            }
                        }
                    }
                }
                startposition += len;
                if (startposition + 8 >= totallength)
                { break; }
            }
        }

        private void SetColByteValues(T value, byte[] ColBytes)
        {
            if (this.Columns == null)
            {
                return;
            }

            int startposition = 0;
            int totallength = ColBytes.Length;
            if (totallength == 0)
            {
                return;
            }
            while (true)
            {
                int FieldNameHash = BitConverter.ToInt32(ColBytes, startposition);

                startposition += 4;

                int len = BitConverter.ToInt32(ColBytes, startposition);
                startposition += 4;


                var col = this.Columns.Find(o => o.FieldNameHash == FieldNameHash);

                if (col != null)
                {
                    byte[] FieldValueBytes = new byte[len];
                    System.Buffer.BlockCopy(ColBytes, startposition, FieldValueBytes, 0, len);
                    col.SetBytes(value, FieldValueBytes);
                }

                startposition += len;
                if (startposition + 8 >= totallength)
                { break; }
            }
        }

        public T FromBytes(byte[] bytes)
        {
            T value = Activator.CreateInstance<T>();
            SetByteValues(value, bytes);
            return value;
        }

        public T FromColumnBytes(byte[] ColumnBytes)
        {
            T value = Activator.CreateInstance<T>();
            SetColByteValues(value, ColumnBytes);
            return value;
        }

        private List<IFieldConverter<T>> _Fields;

        public List<IFieldConverter<T>> Fields
        {
            get
            {
                if (_Fields == null)
                {
                    _Fields = new List<IFieldConverter<T>>();
                }
                return _Fields;
            }
            set { _Fields = value; }
        }

        public List<IColumn<T>> Columns { get; set; }

        public IColumn<T> GetColumn(string ColumnFieldName)
        {
            if (this.Columns == null)
            {
                return null;
            }

            var lower = ColumnFieldName.ToLower();

            foreach (var item in this.Columns)
            {
                if (item.FieldName.ToLower() == lower)
                {
                    return item;
                }
            }

            return null;
        }

    }

    public class SimpleConverter
    {
        private Type objectType { get; set; }

        public SimpleConverter(Type type)
        {
            this.objectType = type;

            if (type.IsAbstract || type.IsInterface)
            {
                throw new Exception("Abstract and Interface can not be initialized.");
            }
            this.Items = new List<IFieldConverter>();

            var allfields = TypeHelper.GetPublicPropertyOrFields(type);

            foreach (var item in allfields)
            {
                AddFields(item.Value, item.Key);
            }

        }

        private void AddFields(Type valuetype, string FieldName)
        {
            var converter = ConverterHelper.GetFieldConverter(this.objectType, valuetype, FieldName);

            if (converter != null)
            {
                Items.Add(converter);
            }
        }

        public byte[] ToBytes(Object value)
        {
            List<byte[]> Results = new List<byte[]>();
            int TotalLength = 0;

            foreach (var item in this.Items)
            {
                byte[] result = item.ToBytes(value);
                if (result == null || result.Length == 0)
                {
                    continue;
                }

                Results.Add(BitConverter.GetBytes(item.FieldNameHash));
                TotalLength += 4;

                int bytelen = item.ByteLength;
                if (bytelen == 0)
                {
                    bytelen = result.Length;
                }

                Results.Add(BitConverter.GetBytes(bytelen));
                TotalLength += 4;

                Results.Add(result);
                TotalLength += result.Length;
            }

            byte[] BackValue = new byte[TotalLength];
            int currentposition = 0;

            foreach (var item in Results)
            {
                int len = item.Length;
                System.Buffer.BlockCopy(item, 0, BackValue, currentposition, len);
                currentposition += len;
            }

            return BackValue;
        }

        private void SetByteValues(object value, byte[] bytes)
        {
            int startposition = 0;
            int totallength = bytes.Length;
            while (true)
            {

                int FieldNameHash = BitConverter.ToInt32(bytes, startposition);

                startposition += 4;

                int len = BitConverter.ToInt32(bytes, startposition);
                startposition += 4;

                var item = Items.Find(o => o.FieldNameHash == FieldNameHash);

                if (item != null)
                {
                    byte[] FieldValueBytes = new byte[len];
                    System.Buffer.BlockCopy(bytes, startposition, FieldValueBytes, 0, len);
                    item.SetByteValues(value, FieldValueBytes);
                }

                startposition += len;

                if (startposition + 8 >= totallength)
                { break; }
            }
        }

        public Object FromBytes(byte[] bytes)
        {
            var value = Activator.CreateInstance(this.objectType);
            SetByteValues(value, bytes);
            return value;
        }

        private List<IFieldConverter> Items
        {
            get; set;
        }

    }

}
