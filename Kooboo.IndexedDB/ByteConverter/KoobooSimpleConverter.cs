//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Columns;
using Kooboo.IndexedDB.Serializer.Simple;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.ByteConverter
{
    public class KoobooSimpleConverter<T> : IByteConverter<T>
    {
        private SimpleConverter<T> converter;

        public KoobooSimpleConverter()
        {
            converter = new SimpleConverter<T>();
        }

        public KoobooSimpleConverter(Dictionary<string, int> columns)
        {
            converter = new SimpleConverter<T>(columns);
        }

        public byte[] ToByte(T input)
        {
            return converter.ToBytes(input);
        }

        public T FromByte(byte[] inputbytes)
        {
            return converter.FromBytes(inputbytes);
        }

        public IColumn<T> GetColumn(string columnName)
        {
            return this.converter.GetColumn(columnName);
        }
    }
}