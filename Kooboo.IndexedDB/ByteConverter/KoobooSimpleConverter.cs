//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB.Columns;
using Kooboo.IndexedDB.Serializer.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.ByteConverter
{
  public  class KoobooSimpleConverter<T> : IByteConverter<T>
    {
       internal SimpleConverter<T> converter; 

        public KoobooSimpleConverter()
        {
            converter = new SimpleConverter<T>(); 
        }

        public KoobooSimpleConverter(Dictionary<string, int> Columns)
        {
            converter = new SimpleConverter<T>(Columns);
        }

        public byte[] ToByte(T input)
        {
           return converter.ToBytes(input); 
        }

        public T FromByte(byte[] inputbytes)
        {
            return converter.FromBytes(inputbytes); 
        }
         
        public IColumn<T> GetColumn(string ColumnName)
        {
            return this.converter.GetColumn(ColumnName);  
        } 
    }
}
