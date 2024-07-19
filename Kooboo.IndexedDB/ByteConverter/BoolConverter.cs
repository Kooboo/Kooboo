//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

namespace Kooboo.IndexedDB.ByteConverter
{
    public class BoolConverter : IByteConverter<bool>
    {
        public byte[] ToByte(bool input)
        {
            byte[] bytes = new byte[1];
            if (input)
            {
                bytes[0] = 1;
            }

            return bytes;
        }

        public bool FromByte(byte[] inputbytes)
        {
            if (inputbytes[0] > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
