using System;
using System.IO;
using System.Xml.Serialization;

namespace Kooboo.Data
{
    public class KscriptConfigReader
    {
        public static KscriptConfig GetConfig()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = System.IO.Path.Combine(baseDirectory, "kscript.config");
            if (File.Exists(filePath))
            {
                using (var fileStream=new FileStream(filePath, FileMode.Open))
                {
                    try
                    {
                        //var xmlSerializer = new XmlSerializer(typeof(KscriptConfig));
                        XmlSerializer xmlSerializer = XmlSerializer.FromTypes(new[] { typeof(KscriptConfig) })[0];
                        return xmlSerializer.Deserialize(fileStream) as KscriptConfig;
                    }
                    catch(Exception ex) { }
                    
                }
            }

            return null;
        }
    }
}
