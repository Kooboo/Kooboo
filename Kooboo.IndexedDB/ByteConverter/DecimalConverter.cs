//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.ByteConverter
{
  public  class DecimalConverter : IByteConverter<decimal>
    {
        public byte[] ToByte(decimal input)
        {
            double doublenumber = Convert.ToDouble(input);
            return BitConverter.GetBytes(doublenumber);
        }

        public decimal FromByte(byte[] inputbytes)
        {
            double doublenumber = BitConverter.ToDouble(inputbytes, 0);
            decimal decimalnumber = Convert.ToDecimal(doublenumber);
            return decimalnumber;
        }
    }
}
