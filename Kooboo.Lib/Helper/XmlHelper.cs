//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Kooboo.Lib.Helper
{
  public static  class XmlHelper
    {
        public static XDocument DeSerialize(string xmlstring)
        {
            if (xmlstring.Contains("<") && xmlstring.Contains(">"))
            {
                try
                {
                    return XDocument.Parse(xmlstring);
                }
                catch (Exception ex)
                {

                }
            }
            return null; 
        }

        public static string Serialize(Object data)
        {
            var type = data.GetType(); 
            XmlSerializer s = XmlSerializer.FromTypes(new[] { type })[0]; 
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            s.Serialize(stream, data);
            string xmlstring = Encoding.UTF8.GetString(stream.ToArray());
            s = null;
            stream.Close();
            return xmlstring;
        } 

        public static object GetMember(XNode node, string memberName)
        {
           if (node is XElement)
            {
                XElement el = node as XElement; 
                foreach (var item in el.Nodes())
                {
                    if (item is XElement)
                    { 
                        var xitem = item as XElement; 
                        if (xitem.Name == memberName)
                        {
                            if (xitem.HasElements)
                            {
                                return xitem.Nodes().ToList();
                            }
                            else
                            {
                                return xitem.Value;
                            }
                        }
                    }
                } 
            } 
           else if (node is XDocument)
            {
                var doc = node as XDocument;
                return GetMember(doc.Root, memberName); 
            } 
            return null; 
        }
    }
}
