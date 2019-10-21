//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Columns;
using Kooboo.IndexedDB.Helper;
using System;
using System.Collections.Generic;

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

        private void AddFields(Type valuetype, string fieldName)
        {
            var converter = ConverterHelper.GetFieldConverter<T>(valuetype, fieldName);

            if (converter != null)
            {
                Fields.Add(converter);
            }
        }

        public byte[] ToBytes(T value)
        {
            List<byte[]> results = new List<byte[]>();

            byte[][] byteArray = new byte[this.BytePartsCount][];
            int index = 0;

            int totalLength = 0;

            if (this.Columns != null)
            {
                foreach (var item in this.Columns)
                {
                    byte[] result = item.GetBytes(value);

                    byteArray[index] = item.FieldNameLengthBytes;
                    index += 1;
                    byteArray[index] = result;
                    index += 1;
                    totalLength += item.Length + 8;
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
                totalLength += result.Length + 8;
            }

            byte[] backValue = new byte[totalLength];
            int currentposition = 0;

            foreach (var item in byteArray)
            {
                if (item != null)
                {
                    int len = item.Length;
                    System.Buffer.BlockCopy(item, 0, backValue, currentposition, len);
                    currentposition += len;
                }
            }
            return backValue;
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
                int fieldNameHash = BitConverter.ToInt32(bytes, startposition);

                startposition += 4;

                int len = BitConverter.ToInt32(bytes, startposition);
                startposition += 4;

                if (len <= 0)
                {
                    break;
                }

                var item = Fields.Find(o => o.FieldNameHash == fieldNameHash);
                if (item != null)
                {
                    byte[] fieldValueBytes = new byte[len];
                    System.Buffer.BlockCopy(bytes, startposition, fieldValueBytes, 0, len);
                    item.SetByteValues(value, fieldValueBytes);
                }
                else
                {
                    var col = Columns?.Find(o => o.FieldNameHash == fieldNameHash);

                    if (col != null)
                    {
                        byte[] fieldValueBytes = new byte[len];
                        System.Buffer.BlockCopy(bytes, startposition, fieldValueBytes, 0, len);
                        col.SetBytes(value, fieldValueBytes);
                    }
                }
                startposition += len;
                if (startposition + 8 >= totallength)
                { break; }
            }
        }

        private void SetColByteValues(T value, byte[] colBytes)
        {
            if (this.Columns == null)
            {
                return;
            }

            int startposition = 0;
            int totallength = colBytes.Length;
            if (totallength == 0)
            {
                return;
            }
            while (true)
            {
                int fieldNameHash = BitConverter.ToInt32(colBytes, startposition);

                startposition += 4;

                int len = BitConverter.ToInt32(colBytes, startposition);
                startposition += 4;

                var col = this.Columns.Find(o => o.FieldNameHash == fieldNameHash);

                if (col != null)
                {
                    byte[] fieldValueBytes = new byte[len];
                    System.Buffer.BlockCopy(colBytes, startposition, fieldValueBytes, 0, len);
                    col.SetBytes(value, fieldValueBytes);
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

        public T FromColumnBytes(byte[] columnBytes)
        {
            T value = Activator.CreateInstance<T>();
            SetColByteValues(value, columnBytes);
            return value;
        }

        private List<IFieldConverter<T>> _fields;

        public List<IFieldConverter<T>> Fields
        {
            get { return _fields ?? (_fields = new List<IFieldConverter<T>>()); }
            set { _fields = value; }
        }

        public List<IColumn<T>> Columns { get; set; }

        public IColumn<T> GetColumn(string columnFieldName)
        {
            if (this.Columns == null)
            {
                return null;
            }

            var lower = columnFieldName.ToLower();

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
        private Type ObjectType { get; set; }

        public SimpleConverter(Type type)
        {
            this.ObjectType = type;

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

        private void AddFields(Type valuetype, string fieldName)
        {
            var converter = ConverterHelper.GetFieldConverter(this.ObjectType, valuetype, fieldName);

            if (converter != null)
            {
                Items.Add(converter);
            }
        }

        public byte[] ToBytes(Object value)
        {
            List<byte[]> results = new List<byte[]>();
            int totalLength = 0;

            foreach (var item in this.Items)
            {
                byte[] result = item.ToBytes(value);
                if (result == null || result.Length == 0)
                {
                    continue;
                }

                results.Add(BitConverter.GetBytes(item.FieldNameHash));
                totalLength += 4;

                int bytelen = item.ByteLength;
                if (bytelen == 0)
                {
                    bytelen = result.Length;
                }

                results.Add(BitConverter.GetBytes(bytelen));
                totalLength += 4;

                results.Add(result);
                totalLength += result.Length;
            }

            byte[] backValue = new byte[totalLength];
            int currentposition = 0;

            foreach (var item in results)
            {
                int len = item.Length;
                System.Buffer.BlockCopy(item, 0, backValue, currentposition, len);
                currentposition += len;
            }

            return backValue;
        }

        private void SetByteValues(object value, byte[] bytes)
        {
            int startposition = 0;
            int totallength = bytes.Length;
            while (true)
            {
                int fieldNameHash = BitConverter.ToInt32(bytes, startposition);

                startposition += 4;

                int len = BitConverter.ToInt32(bytes, startposition);
                startposition += 4;

                var item = Items.Find(o => o.FieldNameHash == fieldNameHash);

                if (item != null)
                {
                    byte[] fieldValueBytes = new byte[len];
                    System.Buffer.BlockCopy(bytes, startposition, fieldValueBytes, 0, len);
                    item.SetByteValues(value, fieldValueBytes);
                }

                startposition += len;

                if (startposition + 8 >= totallength)
                { break; }
            }
        }

        public Object FromBytes(byte[] bytes)
        {
            var value = Activator.CreateInstance(this.ObjectType);
            SetByteValues(value, bytes);
            return value;
        }

        private List<IFieldConverter> Items
        {
            get; set;
        }
    }
}