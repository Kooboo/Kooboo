//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.ByteConverter
{
  public   class Int16Converter : IByteConverter<Int16>
    {
        public byte[] ToByte(short input)
        {
            return BitConverter.GetBytes(input);
        }

        public short FromByte(byte[] inputbytes)
        {
            return BitConverter.ToInt16(inputbytes, 0);
        }
    }
}
