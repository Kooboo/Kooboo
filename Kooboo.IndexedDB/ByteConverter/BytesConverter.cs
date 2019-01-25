//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.ByteConverter
{
  public  class BytesConverter : IByteConverter<byte[]>
    {

        public byte[] ToByte(byte[] input)
        {
            return input;
        }

        public byte[] FromByte(byte[] inputbytes)
        {
            return inputbytes; 
        }
    }
}
