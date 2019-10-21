//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Serializer.Simple
{
    public class ClassConverter
    {
        private Type ClassType;

        public ClassConverter(Type classType)
        {
            this.ClassType = classType;
            Items = new List<IFieldConverter>();
        }

        public void InitFields()
        {
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

        public object FromBytes(byte[] bytes)
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

            return fieldObject;
        }

        public byte[] ToBytes(object value)
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

        private List<IFieldConverter> Items
        {
            get; set;
        }
    }
}