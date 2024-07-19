//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Serializer.Simple
{
    public class CollectionConverter
    {
        private Type DataType;
        private Type CollectionType;

        private int FieldLength;

        private Func<object, byte[]> GetObjectBytes;
        private Func<byte[], object> GetObjectValue;

        public CollectionConverter(Type CollectionType)
        {
            this.CollectionType = CollectionType;
            this.DataType = ObjectHelper.GetEnumberableType(CollectionType);

            this.FieldLength = ConverterHelper.GetTypeLength(this.DataType);

            this.GetObjectBytes = ConverterHelper.GetValueToBytes(this.DataType);
            this.GetObjectValue = ConverterHelper.GetBytesToValue(this.DataType);

            if (this.GetObjectBytes == null || this.GetObjectValue == null)
            {
                throw new Exception(this.DataType.Name + " is not yet supported.");
            }
        }

        public object FromBytes(byte[] bytes)
        {
            var GenericHashSet = typeof(CollectionWrapper<>).MakeGenericType(DataType);

            var OriginalInstance = Activator.CreateInstance(this.CollectionType);

            var list = Activator.CreateInstance(GenericHashSet, OriginalInstance) as System.Collections.IList;

            int startposition = 0;
            int totallength = bytes.Length;

            while (true)
            {
                if (this.FieldLength > 0)
                {

                    byte[] FieldValueBytes = new byte[this.FieldLength];
                    System.Buffer.BlockCopy(bytes, startposition, FieldValueBytes, 0, this.FieldLength);
                    startposition += this.FieldLength;

                    var objectvalue = this.GetObjectValue(FieldValueBytes);


                    list.Add(objectvalue);
                }
                else
                {
                    int len = BitConverter.ToInt32(bytes, startposition);
                    startposition += 4;

                    if (len > 0)
                    {
                        byte[] FieldValueBytes = new byte[len];
                        System.Buffer.BlockCopy(bytes, startposition, FieldValueBytes, 0, len);
                        startposition += len;
                        var objectvalue = this.GetObjectValue(FieldValueBytes);
                        list.Add(objectvalue);
                    }
                    else
                    {
                        list.Add(null);
                    }
                }

                if (startposition >= totallength)
                { break; }
            }

            return OriginalInstance;

        }

        public byte[] ToBytes(object value)
        {
            if (value == null)
            {
                return null;
            }

            List<byte[]> results = new List<byte[]>();


            int totallen = 0;

            foreach (var item in (IEnumerable)value)
            {
                var result = this.GetObjectBytes(item);

                if (this.FieldLength > 0)
                {
                    results.Add(result);
                    totallen += this.FieldLength;
                }
                else
                {
                    if (result != null)
                    {
                        results.Add(BitConverter.GetBytes(result.Length));
                        results.Add(result);
                        totallen += 4 + result.Length;
                    }
                    else
                    {
                        results.Add(BitConverter.GetBytes(0));
                        totallen += 4;
                    }
                }
            }

            byte[] BackValue = new byte[totallen];
            int currentposition = 0;

            foreach (var item in results)
            {
                int len = item.Length;
                System.Buffer.BlockCopy(item, 0, BackValue, currentposition, len);
                currentposition += len;
            }

            return BackValue;

        }
    }
}
