//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class ClassFieldConverter<T> : IFieldConverter<T>
    {
        private Type ClassType;

        private Func<T, object> getValue;
        private Action<T, object> setValue;

        public ClassFieldConverter(string fieldName)
        {
            Type type = typeof(T);
            this.ClassType = ObjectHelper.GetFieldType(type, fieldName);

            this.Items = new List<IFieldConverter>();
            this.getValue = ObjectHelper.GetGetObjectValue<T>(fieldName);
            this.setValue = ObjectHelper.GetSetObjectValue<T>(fieldName, ClassType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);

            var allfields = TypeHelper.GetPublicPropertyOrFields(ClassType);

            foreach (var item in allfields)
            {
                AddFields(ClassType, item.Value, item.Key);
            }
        }

        private void AddFields(Type classType, Type fieldType, string fieldName)
        {
            var converter = ConverterHelper.GetFieldConverter(classType, fieldType, fieldName);

            if (converter != null)
            {
                Items.Add(converter);
            }
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
            var fieldObject = Activator.CreateInstance(this.ClassType);

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
                    item.SetByteValues(fieldObject, fieldValueBytes);
                }

                startposition += len;

                if (startposition + 8 >= totallength)
                { break; }
            }

            this.setValue(value, fieldObject);
        }

        public byte[] ToBytes(T value)
        {
            object fieldvalue = this.getValue(value);

            List<byte[]> results = new List<byte[]>();
            int totalLength = 0;

            if (fieldvalue != null)
            {
                foreach (var item in this.Items)
                {
                    byte[] result = item.ToBytes(fieldvalue);
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

        private List<IFieldConverter> Items
        {
            get; set;
        }
    }

    public class ClassFieldConverter : IFieldConverter
    {
        private Type ClassType;

        private Func<object, object> getValue;
        private Action<object, object> setValue;

        public ClassFieldConverter(Type objectType, string fieldName)
        {
            this.ClassType = ObjectHelper.GetFieldType(objectType, fieldName);
        }

        public ClassFieldConverter(Type objectType, Type classFieldType, string fieldName)
        {
            ClassType = classFieldType;
            Items = new List<IFieldConverter>();
            getValue = ObjectHelper.GetGetObjectValue(fieldName, objectType);
            setValue = ObjectHelper.GetSetObjectValue(fieldName, objectType, ClassType);
            FieldNameHash = ObjectHelper.GetHashCode(fieldName);

            foreach (var item in ClassType.GetProperties())
            {
                if (item.CanRead && item.CanWrite)
                {
                    AddFields(ClassType, item.PropertyType, item.Name);
                }
            }

            foreach (var item in ClassType.GetFields())
            {
                if (item.IsPublic && !item.IsStatic)
                {
                    AddFields(ClassType, item.FieldType, item.Name);
                }
            }
        }

        private void AddFields(Type classType, Type fieldType, string fieldName)
        {
            if (classType == fieldType)
            {
                Items.Add(this);
            }
            else
            {
                var converter = ConverterHelper.GetFieldConverter(classType, fieldType, fieldName);

                if (converter != null)
                {
                    Items.Add(converter);
                }
            }
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
            var fieldObject = Activator.CreateInstance(this.ClassType);

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
                    item.SetByteValues(fieldObject, fieldValueBytes);
                }

                startposition += len;

                if (startposition + 8 >= totallength)
                { break; }
            }

            this.setValue(value, fieldObject);
        }

        public byte[] ToBytes(object value)
        {
            object fieldvalue = this.getValue(value);

            List<byte[]> results = new List<byte[]>();
            int totalLength = 0;

            if (fieldvalue != null)
            {
                foreach (var item in this.Items)
                {
                    byte[] result = item.ToBytes(fieldvalue);
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

        private List<IFieldConverter> Items
        {
            get; set;
        }
    }
}