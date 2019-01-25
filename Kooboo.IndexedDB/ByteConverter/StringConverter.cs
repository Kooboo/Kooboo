//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.ByteConverter
{
  public  class StringConverter : IByteConverter<string>
    {

        public byte[] ToByte(string input)
        {
            if (input == null)
            {
                input = string.Empty;
            }
           
            return GlobalSettings.DefaultEncoding.GetBytes(input);
         
        }

        public string FromByte(byte[] inputbytes)
        { 
            return GlobalSettings.DefaultEncoding.GetString(inputbytes).TrimEnd('\0');
        }

    }
}
