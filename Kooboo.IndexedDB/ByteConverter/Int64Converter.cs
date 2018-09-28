using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.ByteConverter
{
  public  class Int64Converter : IByteConverter<Int64>
    {

        public byte[] ToByte(long input)
        {
            return BitConverter.GetBytes(input);
        }

        public long FromByte(byte[] inputbytes)
        {
            return BitConverter.ToInt64(inputbytes,0);
        }
    }
}
