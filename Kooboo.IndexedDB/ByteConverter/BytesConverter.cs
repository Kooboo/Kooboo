//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB.ByteConverter
{
    public class BytesConverter : IByteConverter<byte[]>
    {

        public byte[] ToByte(byte[] input)
        {
            return input;
        }

        public byte[] FromByte(byte[] inputBytes)
        {
            return inputBytes;
        }
    }
}
