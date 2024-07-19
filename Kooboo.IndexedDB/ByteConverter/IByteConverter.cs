//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB.ByteConverter
{

    /// <summary>
    /// Convert save object to and from bytes. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IByteConverter<T>
    {

        byte[] ToByte(T input);

        T FromByte(byte[] inputbytes);

    }
}
