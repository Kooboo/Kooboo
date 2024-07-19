//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB.ByteConverter
{
    public class ByteConverter : IByteConverter<byte>
    {
        public byte[] ToByte(byte input)
        {
            byte[] newBytes = new byte[1];
            newBytes[0] = input;
            return newBytes;
        }

        public byte FromByte(byte[] inputBytes)
        {
            return inputBytes[0];
        }
    }
}
