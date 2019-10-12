//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.IndexedDB.ByteConverter
{
    public class ByteConverter : IByteConverter<byte>
    {
        public byte[] ToByte(byte input)
        {
            byte[] newbytes = new byte[1];
            newbytes[0] = input;
            return newbytes;
        }

        public byte FromByte(byte[] inputbytes)
        {
            return inputbytes[0];
        }
    }
}