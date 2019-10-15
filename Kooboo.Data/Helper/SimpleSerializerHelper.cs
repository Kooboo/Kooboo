//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Data.Helper
{
    public static class SimpleSerializerHelper
    {
        public static byte[] Serialize(object model)
        {
            var type = model.GetType();
            var ser = new IndexedDB.Serializer.Simple.SimpleConverter(type);
            return ser.ToBytes(model);
        }

        public static byte[] Serialize<T>(T model)
        {
            var type = typeof(T);
            var ser = new IndexedDB.Serializer.Simple.SimpleConverter(type);
            return ser.ToBytes(model);
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            var type = typeof(T);
            var ser = new IndexedDB.Serializer.Simple.SimpleConverter(type);
            var value = ser.FromBytes(bytes);
            if (value != null)
            {
                return (T)value;
            }
            return default(T);
        }

        public static object Deserialize(byte[] bytes, Type objectType)
        {
            var ser = new IndexedDB.Serializer.Simple.SimpleConverter(objectType);
            return ser.FromBytes(bytes);
        }
    }
}