using Kooboo.Data.Interface;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Sync.DiskSyncLog
{
    public static class DiskObjectConverter
    {
        internal static string _endmark { get; set; } = "_end//\r\n";

        public static byte[] ToBytes(SiteDb sitedb, ISiteObject siteobject)
        {
            if (siteobject == null)
            {
                return null;
            }
            var disktype = Kooboo.Attributes.AttributeHelper.GetDiskType(siteobject);

            if (disktype == Attributes.DiskType.Text && siteobject is ITextObject)
            {
                var textobject = siteobject as ITextObject;
                if (textobject is Kooboo.Sites.Models.Code)
                {
                    var code = textobject as Kooboo.Sites.Models.Code;
                    return CodeToBytes(sitedb, code);
                }
                else
                {
                    if (!string.IsNullOrEmpty(textobject.Body))
                    {
                        return System.Text.Encoding.UTF8.GetBytes(textobject.Body);
                    }
                }
            }
            else if (disktype == Attributes.DiskType.Binary && siteobject is IBinaryFile)
            {
                var binaryfile = siteobject as IBinaryFile;
                return binaryfile.ContentBytes;
            }
            else
            {
                string textbody = JsonHelper.Serialize(siteobject);
                if (!string.IsNullOrEmpty(textbody))
                {
                    return System.Text.Encoding.UTF8.GetBytes(textbody);
                }
            }
            return null;
        }

        public static bool FromBytes(ref ISiteObject SiteObject, byte[] DiskBytes)
        {
            var modeltype = SiteObject.GetType();
            var SerializerType = Attributes.AttributeHelper.GetDiskType(modeltype);

            if (SerializerType == Kooboo.Attributes.DiskType.Binary)
            {
                var binaryfile = SiteObject as IBinaryFile;
                if (DiskBytes == null || IOHelper.IsEqualBytes(binaryfile.ContentBytes, DiskBytes))
                {
                    return false;
                }
                binaryfile.ContentBytes = DiskBytes;
            }
            else if (SerializerType == Kooboo.Attributes.DiskType.Text)
            {
                var textfile = SiteObject as ITextObject;

                if (SiteObject is Kooboo.Sites.Models.Code)
                {
                    var currentcode = SiteObject as Models.Code;
                    var currentbody = currentcode.Body;

                    var ok = FromCodeBytes(currentcode, DiskBytes);
                    if (!ok || currentbody == currentcode.Body)
                    {
                        return false;
                    }
                }
                else
                {
                    string textbody = System.Text.Encoding.UTF8.GetString(DiskBytes);

                    if (StringHelper.IsSameValue(textbody, textfile.Body))
                    {
                        return false;
                    }
                    textfile.Body = textbody;
                }
            }
            else
            {
                string fulltext = System.Text.Encoding.UTF8.GetString(DiskBytes);
                var generatedbody = Lib.Helper.JsonHelper.Serialize(SiteObject);

                if (StringHelper.IsSameValue(generatedbody, fulltext))
                {
                    return false;
                }
                SiteObject = JsonHelper.Deserialize(fulltext, modeltype) as ISiteObject;
            }
            return true;
        }

        public static byte[] CodeToBytes(SiteDb sitedb, Kooboo.Sites.Models.Code code)
        {
            string result = "//Generated, do not modify:  <type>" + code.CodeType.ToString() + "</type>";

            if (code.CodeType == Models.CodeType.Api && sitedb != null)
            {
                // API has routing. 
                var route = sitedb.Routes.GetByObjectId(code.Id);
                if (route != null)
                {
                    result += "<route>" + route.Name + "</route>";
                }
            }

            result += _endmark;

            var textobject = code as ITextObject;
            if (!string.IsNullOrEmpty(textobject.Body))
            {
                result += textobject.Body;
            }

            return System.Text.Encoding.UTF8.GetBytes(result);

        }

        public static bool FromCodeBytes(Models.Code code, byte[] DiskBytes)
        {
            string text = System.Text.Encoding.UTF8.GetString(DiskBytes);

            var linebreak = text.IndexOf(_endmark);

            var type = GetFirstLineString(text, "type");

            if (type != null && linebreak > -1)
            {
                var codebody = text.Substring(linebreak + _endmark.Length);
                code.CodeType = Lib.Helper.EnumHelper.GetEnum<Kooboo.Sites.Models.CodeType>(type);
                code.Body = codebody;
                return true;
            }

            return false;
        }


        public static string GetRouteFromCodeBytes(byte[] codeDiskBytes)
        {
            string text = System.Text.Encoding.UTF8.GetString(codeDiskBytes);
            return GetFirstLineString(text, "route");
        }

        public static string GetFirstLineString(string input, string tag)
        {

            var linebreak = input.IndexOf(_endmark);
            if (linebreak > 0)
            {
                var line = input.Substring(0, linebreak);

                string starttag = "<" + tag + ">";
                string endtag = "</" + tag + ">";

                var start = line.IndexOf(starttag);
                var end = line.IndexOf(endtag);

                if (start > -1 && end > -1)
                {
                    return line.Substring(start + starttag.Length, end - start - starttag.Length);
                }
            }
            return null;
        }

        //TODO: move somewhere else.
        public static string GetNewRoute(SiteDb sitedb, string routename)
        {
           if (routename == null)
            {
                return null; 
            }

           if (!routename.StartsWith("/"))
            {
                routename = "/" + routename; 
            }

            var route = sitedb.Routes.Get(routename);
            if (route == null)
            {
                return routename; 
            }
            else
            {
                for (int i = 1; i < 999; i++)
                {
                    var name = routename + i.ToString();

                    route = sitedb.Routes.Get(name); 
                    if (route == null)
                    {
                        return name; 
                    }
                    
                } 
            }

            return null; 
        }
    }
}
