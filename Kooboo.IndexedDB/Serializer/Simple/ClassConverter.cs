//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple
{
    public class ClassConverter
    {
        Type ClassType;

        public ClassConverter(Type ClassType)
        {
            this.ClassType = ClassType;
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

        private void AddFields(Type ClassType, Type FieldType, string FieldName)
        {
            var converter = ConverterHelper.GetFieldConverter(ClassType, FieldType, FieldName);

            if (converter != null)
            {
                Items.Add(converter);
            }
        }

        public object FromBytes(byte[] bytes)
        {
            var FieldObject = Activator.CreateInstance(this.ClassType);

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
                    item.SetByteValues(FieldObject, FieldValueBytes);
                }

                startposition += len;

                if (startposition + 8 >= totallength)
                { break; }
            }

            return FieldObject;
        }

        public byte[] ToBytes(object Value)
        {

            List<byte[]> Results = new List<byte[]>();
            int TotalLength = 0;

            foreach (var item in this.Items)
            {
                byte[] result = item.ToBytes(Value);
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

        private List<IFieldConverter> Items
        {
            get; set;
        }

    }
}
