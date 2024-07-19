//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.IndexedDB.ByteConverter
{
    public class DoubleConverter : IByteConverter<double>
    {
        public byte[] ToByte(double input)
        {
            return BitConverter.GetBytes(input);
        }

        public double FromByte(byte[] inputbytes)
        {
            return BitConverter.ToDouble(inputbytes, 0);
        }
    }
}
