//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.ByteConverter
{
    public class CharConverter : IByteConverter<char>
    {
        public char FromByte(byte[] inputbytes)
        {
            int codevalue = BitConverter.ToInt32(inputbytes, 0); 
            return (char)codevalue;
        }

        public byte[] ToByte(char input)
        {
            int codevalue =  input;
            return BitConverter.GetBytes(codevalue); 
        }
    }
     
}
