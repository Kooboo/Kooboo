//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.IndexedDB.ByteConverter
{
    public class Int64Converter : IByteConverter<Int64>
    {

        public byte[] ToByte(long input)
        {
            return BitConverter.GetBytes(input);
        }

        public long FromByte(byte[] inputbytes)
        {
            return BitConverter.ToInt64(inputbytes, 0);
        }
    }
}
