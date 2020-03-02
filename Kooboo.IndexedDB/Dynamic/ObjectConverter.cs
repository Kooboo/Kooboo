//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.Dynamic.Converter
{
    public class ObjectConverter
    {

        private string _primarykey;
        private string PrimaryKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_primarykey))
                {
                    _primarykey = Constants.DefaultIdFieldName;
                }
                return _primarykey;
            }
            set
            {
                _primarykey = value;
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

                    converter.ClrType = type;

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
        }

        public byte[] ToBytes(Dictionary<string, object> predata)
        {
            List<byte[]> Results = new List<byte[]>();
            int TotalLength = 0;
             
            foreach (var item in this.Fields)
            {
                byte[] result = null;

                object rightvalue = null;
                if (predata.ContainsKey(item.FieldName))            
                {
                    var prevalue = predata[item.FieldName];    
                    rightvalue = Dynamic.Accessor.ChangeType(prevalue, item.ClrType);   
                }
                else
                {
                    rightvalue = IndexHelper.DefaultValue(item.ClrType); 
                }
                
                if (rightvalue != null)
                {
                    result = item.ToBytes(rightvalue);
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

                int bytelen = item.Length;
                if (bytelen == int.MaxValue)
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
          
        public IDictionary<string, object> FromBytes(byte[] bytes)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            int startposition = 0;
            int totallength = bytes.Length;
            while (true)
            {
                int FieldNameHash = BitConverter.ToInt32(bytes, startposition);

                startposition += 4;

                int len = BitConverter.ToInt32(bytes, startposition);
                startposition += 4;

                var item = Fields.Find(o => o.FieldNameHash == FieldNameHash);

                if (item != null)
                {
                     if (item.Length == len || item.ClrType == typeof(string) || item.Length == int.MaxValue)
                     {
                        byte[] FieldValueBytes = new byte[len];
                        System.Buffer.BlockCopy(bytes, startposition, FieldValueBytes, 0, len);

                        object obj = item.FromBytes(FieldValueBytes);

                        if (obj != null)
                        {
                            values[item.FieldName] = obj;
                        }
                     }   
                }

                startposition += len;

                if (startposition + 8 >= totallength)
                { break; }
            }

            return values;
        }

        public List<FieldConverter> Fields { get; set; } = new List<FieldConverter>();

        public T FromBytes<T>(byte[] bytes)
        {
            var returnobj = Activator.CreateInstance<T>();

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

        private Dictionary<string, Type> fieldtypes = new Dictionary<string, Type>();

        private Type GetType(Type type, string fieldname)
        {
            string key = type.FullName + fieldname;
            if (fieldtypes.ContainsKey(key))
            {
                return fieldtypes[key];
            }
            else
            {
                var fieldtype = Helper.TypeHelper.GetFieldType(type, fieldname);
                fieldtypes[key] = fieldtype;
                return fieldtype; 
            }
        }

        private IDictionary<string, object> FromBytes(byte[] bytes, Type type)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            int startposition = 0;
            int totallength = bytes.Length;
            while (true)
            {
                int FieldNameHash = BitConverter.ToInt32(bytes, startposition);

                startposition += 4;

                int len = BitConverter.ToInt32(bytes, startposition);
                startposition += 4;

                var item = Fields.Find(o => o.FieldNameHash == FieldNameHash);

                if (item != null)
                {
                    byte[] FieldValueBytes = new byte[len];
                    System.Buffer.BlockCopy(bytes, startposition, FieldValueBytes, 0, len);

                    object obj = item.FromBytes(FieldValueBytes);

                    if (obj != null)
                    {
                        if (item.IsIncremental)
                        {
                            // verify and change type.... when properly. 
                            var itemtype = GetType(type, item.FieldName);
                            if (itemtype != null)
                            {
                                var rightvalue = Convert.ChangeType(obj, itemtype);
                                values[item.FieldName] = rightvalue;
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
                startposition += len;
                if (startposition + 8 >= totallength)
                { break; }
            }

            return values;
        }


    }
}
