using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.ByteConverter
{
  public  class GuidConverter : IByteConverter<Guid>
    {

        public byte[] ToByte(Guid input)
        {
           return input.ToByteArray();
        }

        public Guid FromByte(byte[] inputbytes)
        {
            Guid newguid = new Guid(inputbytes);
            return newguid;
        }


        public static byte[] ConvertToByte(Guid input)
        {
            return input.ToByteArray();
        }

        public static Guid ConvertFromByte(byte[] inputbytes)
        {
            Guid newguid = new Guid(inputbytes);
            return newguid;
        }
    }
}
