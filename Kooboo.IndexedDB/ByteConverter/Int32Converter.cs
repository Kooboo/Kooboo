//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;


namespace Kooboo.IndexedDB.ByteConverter
{
    public class Int32Converter : IByteConverter<int>
    {
        public byte[] ToByte(int input)
        {
            return BitConverter.GetBytes(input);
        }

        public int FromByte(byte[] inputbytes)
        {
            return BitConverter.ToInt32(inputbytes, 0);
        }
    }
}
