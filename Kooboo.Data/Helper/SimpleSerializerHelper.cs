//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
  
namespace Kooboo.Data.Helper
{
   public static class SimpleSerializerHelper
    { 
        public static byte[] Serialize(object Model)
        {
            var type = Model.GetType();
            var ser = new IndexedDB.Serializer.Simple.SimpleConverter(type);
            return ser.ToBytes(Model);  
        }

        public static byte[] Serialize<T>(T Model)
        {
            var type = typeof(T); 
            var ser = new IndexedDB.Serializer.Simple.SimpleConverter(type); 
            return ser.ToBytes(Model);
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            var type = typeof(T);
            var ser = new IndexedDB.Serializer.Simple.SimpleConverter(type);
            var value = ser.FromBytes(bytes); 
            if (value !=null)
            {
                return (T)value; 
            }
            return default(T);
        }

        public static object Deserialize(byte[] bytes, Type ObjectType)
        {
            var ser = new IndexedDB.Serializer.Simple.SimpleConverter(ObjectType);
            return ser.FromBytes(bytes);
        }
         
    }
}
