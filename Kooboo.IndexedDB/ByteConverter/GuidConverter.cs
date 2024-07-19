//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.IndexedDB.ByteConverter
{
    public class GuidConverter : IByteConverter<Guid>
    {

        public byte[] ToByte(Guid input)
        {
            return input.ToByteArray();
        }

        public Guid FromByte(byte[] inputBytes)
        {
            Guid newGuid = new Guid(inputBytes);
            return newGuid;
        }


        public static byte[] ConvertToByte(Guid input)
        {
            return input.ToByteArray();
        }

        public static Guid ConvertFromByte(byte[] inputBytes)
        {
            Guid newGuid = new Guid(inputBytes);
            return newGuid;
        }
    }
}
