//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Kooboo.Lib.Serializer;

namespace Kooboo.Lib.Helper
{
    public static class XmlHelper
    {
        public static XDocument DeSerialize(string xmlstring)
        {
            if (xmlstring.Contains("<") && xmlstring.Contains(">"))
            {
                try
                {
                    var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlstring));
                    return XDocument.Load(stream);
                }
                catch (Exception)
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

        public static string SerializeElement(XElement root)
        {
            var builder = new StringBuilder();
            using (TextWriter writer = new Utf8StringWriter(builder))
            {
                root.Save(writer);
            }
            return builder.ToString();
        }

        public static string GetElementValue(XElement item, string key)
        {
            if (item == default)
            {
                return string.Empty;
            }
            return Descendants(item, key)?.FirstOrDefault()?.Value?.Trim();
        }

        public static IEnumerable<XElement> Descendants(XElement root, string key)
        {
            if (root == null) return Enumerable.Empty<XElement>();
            return root.Descendants(root.GetDefaultNamespace() + key);
        }

        public static IEnumerable<XElement> Descendants(XDocument doc, string key)
        {
            return Descendants(doc.Root, key);
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
