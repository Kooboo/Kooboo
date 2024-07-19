//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Kooboo.IndexedDB
{
    public class SettingSerializer
    {
        public static ObjectStoreSetting DeserializeObjectStoreSetting(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                ObjectStoreSetting settings = (ObjectStoreSetting)formatter.Deserialize(stream);
                formatter = null;
                stream.Close();
                return settings;
            }
            return null;
        }
    }
}
