//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Dynamic.Converter
{
    public class ObjectConverter
    {

        private string _primaryKey;
        private string PrimaryKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_primaryKey))
                {
                    _primaryKey = Constants.DefaultIdFieldName;
                }
                return _primaryKey;
            }
            set
            {
                _primaryKey = value;
            }
        }

        public ObjectConverter(List<TableColumn> Columns, string PrimaryKey)
        {
            this.PrimaryKey = PrimaryKey;

            foreach (var item in Columns.OrderBy(o => o.relativePosition))
            {
                var type = Helper.TypeHelper.GetType(item.DataType);
                if (type != null)
                {
                    FieldConverter converter = new FieldConverter();

                    converter.IsIncremental = item.IsIncremental;

                    converter.DataType = type;

                    converter.FieldName = item.Name;
                    converter.FieldNameHash = ObjectHelper.GetHashCode(item.Name);

                    converter.Length = item.Length;

                    converter.RelativePosition = item.relativePosition;

                    if (item.isComplex)
                    {
                        Serializer.Simple.SimpleConverter simple = new Serializer.Simple.SimpleConverter(type);
                        converter.ToBytes = simple.ToBytes;
                        converter.FromBytes = simple.FromBytes;
                        converter.IsComplex = true;
                        converter.Length = int.MaxValue;
                        converter.RelativePosition = int.MaxValue;
                    }
                    else
                    {
                        converter.ToBytes = ConverterHelper.GetToBytes(type);
                        converter.FromBytes = ConverterHelper.GetFromBytes(type);
                    }

                    this.Fields.Add(converter);
                }
            }

            foreach (var item in SystemFields.Fields)
            {
                if (!this.Fields.Exists(o => o.FieldName == item.FieldName))
                {
                    this.Fields.Add(item);
                }
            }

        }

        public byte[] ToBytes(Dictionary<string, object> preData)
        {
            List<byte[]> Results = new List<byte[]>();
            int TotalLength = 0;

            foreach (var item in this.Fields)
            {
                byte[] result = null;

                object rightValue = null;
                if (preData.ContainsKey(item.FieldName))
                {
                    var preValue = preData[item.FieldName];
                    rightValue = Dynamic.Accessor.ChangeType(preValue, item.DataType);
                }
                else
                {
                    if (item.SkipDefaultValue)
                    {
                        continue;
                    }

                    rightValue = IndexHelper.DefaultValue(item.DataType);
                }

                if (rightValue != null)
                {
                    result = item.ToBytes(rightValue);
                }

                if (result == null || result.Length == 0)
                {
                    if (item.Length != int.MaxValue)
                    {
                        result = new byte[item.Length];
                    }
                    else
                    {
                        continue;
                    }
                }

                if (item.Length != int.MaxValue)
                {
                    result = Kooboo.IndexedDB.Helper.KeyHelper.AppendToKeyLength(result, true, item.Length);
                }

                Results.Add(BitConverter.GetBytes(item.FieldNameHash));
                TotalLength += 4;

                int byteLen = item.Length;
                if (byteLen == int.MaxValue)
                {
                    byteLen = result.Length;
                }

                Results.Add(BitConverter.GetBytes(byteLen));
                TotalLength += 4;

                Results.Add(result);
                TotalLength += result.Length;
            }

            byte[] BackValue = new byte[TotalLength];
            int currentPosition = 0;


            foreach (var item in Results)
            {
                int len = item.Length;
                System.Buffer.BlockCopy(item, 0, BackValue, currentPosition, len);
                currentPosition += len;
            }

            return BackValue;
        }

        public IDictionary<string, object> FromBytes(byte[] bytes)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            int startPosition = 0;
            int totalLength = bytes.Length;
            while (true)
            {
                int FieldNameHash = BitConverter.ToInt32(bytes, startPosition);

                startPosition += 4;

                int len = BitConverter.ToInt32(bytes, startPosition);
                startPosition += 4;

                var item = Fields.Find(o => o.FieldNameHash == FieldNameHash);

                if (item != null)
                {
                    if (item.Length == len || item.DataType == typeof(string) || item.Length == int.MaxValue)
                    {
                        byte[] FieldValueBytes = new byte[len];
                        System.Buffer.BlockCopy(bytes, startPosition, FieldValueBytes, 0, len);

                        object obj = item.FromBytes(FieldValueBytes);

                        if (obj != null)
                        {
                            values[item.FieldName] = obj;
                        }
                    }
                }

                startPosition += len;

                if (startPosition + 8 >= totalLength)
                { break; }
            }

            return values;
        }

        public List<FieldConverter> Fields { get; set; } = new List<FieldConverter>();

        public T FromBytes<T>(byte[] bytes)
        {
            var type = typeof(T);
            var cls = Activator.CreateInstance<T>();

            var values = this.FromBytes(bytes, type);
            foreach (var item in values)
            {
                var setter = Accessor.GetSetter(type, item.Key);
                if (setter != null)
                {
                    setter(cls, item.Value);
                }
            }
            return cls;
        }

        private Dictionary<string, Type> fieldTypes = new Dictionary<string, Type>();

        private Type GetType(Type type, string fieldname)
        {
            string key = type.FullName + fieldname;
            if (fieldTypes.ContainsKey(key))
            {
                return fieldTypes[key];
            }
            else
            {
                var fieldType = Helper.TypeHelper.GetFieldType(type, fieldname);
                fieldTypes[key] = fieldType;
                return fieldType;
            }
        }

        private IDictionary<string, object> FromBytes(byte[] bytes, Type type)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            int startPosition = 0;
            int totalLength = bytes.Length;
            while (true)
            {
                int FieldNameHash = BitConverter.ToInt32(bytes, startPosition);

                startPosition += 4;

                int len = BitConverter.ToInt32(bytes, startPosition);
                startPosition += 4;

                var item = Fields.Find(o => o.FieldNameHash == FieldNameHash);

                if (item != null)
                {
                    byte[] FieldValueBytes = new byte[len];
                    System.Buffer.BlockCopy(bytes, startPosition, FieldValueBytes, 0, len);

                    object obj = item.FromBytes(FieldValueBytes);

                    if (obj != null)
                    {
                        if (item.IsIncremental)
                        {
                            // verify and change type.... when properly. 
                            var itemType = GetType(type, item.FieldName);
                            if (itemType != null)
                            {
                                var rightValue = Convert.ChangeType(obj, itemType);
                                values[item.FieldName] = rightValue;
                            }
                            else
                            {
                                values[item.FieldName] = obj;
                            }
                        }
                        else
                        {
                            values[item.FieldName] = obj;
                        }
                    }
                }
                startPosition += len;
                if (startPosition + 8 >= totalLength)
                { break; }
            }

            return values;
        }


    }
}
