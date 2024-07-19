//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Kooboo.IndexedDB.ByteConverter
{
    class BinaryConverter<T> : IByteConverter<T>
    {

        BinaryFormatter bf;

        public BinaryConverter()
        {

            bf = new BinaryFormatter();
        }

        public byte[] ToByte(T input)
        {
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, input);
            return ms.ToArray();
        }

        public T FromByte(byte[] inputbytes)
        {
            int length = inputbytes.Length;
            if (length == 0)
            {
                return default(T);
            }
            MemoryStream memStream = new MemoryStream();
            memStream.Write(inputbytes, 0, inputbytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            T obj = (T)bf.Deserialize(memStream);
            return obj;
        }

    }
}
