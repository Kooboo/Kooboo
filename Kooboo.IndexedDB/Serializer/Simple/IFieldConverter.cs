//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB.Serializer.Simple
{
    /// <summary>
    /// Use Generic Field Converter to skip the boxing and unboxing, for better performance in the first class level.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFieldConverter<T>
    {
        int FieldNameHash
        {
            get; set;
        }
        /// <summary>
        /// The length for this field,use 0 for uncertain field. 
        /// </summary>
        int ByteLength { get; }

        void SetByteValues(T value, byte[] bytes);

        byte[] ToBytes(T Value);
    }

    public interface IFieldConverter
    {
        int FieldNameHash
        {
            get; set;
        }
        /// <summary>
        /// The length for this field,use 0 for uncertain field. 
        /// </summary>
        int ByteLength { get; }

        void SetByteValues(object value, byte[] bytes);

        byte[] ToBytes(object Value);
    }

}
