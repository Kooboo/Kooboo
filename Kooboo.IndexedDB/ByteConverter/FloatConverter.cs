//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.ByteConverter
{
  public  class FloatConverter : IByteConverter<float>
    {
        public byte[] ToByte(float input)
        {
            return BitConverter.GetBytes(input);
        }

        public float FromByte(byte[] inputbytes)
        {
            // float == single
            return BitConverter.ToSingle(inputbytes, 0);
        }
    }
}
