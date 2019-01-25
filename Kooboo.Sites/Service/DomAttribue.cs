//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom.Dom;
using Kooboo.Dom;
using System.Text;
using System.Linq; 



namespace Kooboo.Sites.Service
{

    /// <summary>
    /// This class is used to add new html element attribute the dom and generate a new html string back.
    /// 
    /// </summary>
    public class DomAttribue
    {
        public DomAttribue()
        {
            this.ToBeAdded = new Dictionary<Element, Dictionary<string, string>>();
        }

        public DomAttribue(Dom.Document sourceDocument)
        {
            this.ToBeAdded = new Dictionary<Element, Dictionary<string, string>>();
            this.document = sourceDocument; 
        }

        public Dom.Document document { get; set; }

        private Dictionary<Element, Dictionary<string, string>> ToBeAdded;

        public void AddAttribute(Element element, string attributeName, string attributeValue)
        {
            if (ToBeAdded.ContainsKey(element))
            {
                Dictionary<string, string> dictionary = ToBeAdded[element];

                if (dictionary.ContainsKey(attributeName))
                {
                    dictionary[attributeName] = attributeValue;
                }
                else
                {
                    dictionary.Add(attributeName, attributeValue);
                }
            }
            else
            {
                Dictionary<string, string> newvalue = new Dictionary<string, string>();
                newvalue.Add(attributeName, attributeValue);
                ToBeAdded.Add(element, newvalue);
            }
        }

        /// <summary>
        /// Reserialize the dom with new attribute values and return the new html source string. 
        /// </summary>
        /// <returns></returns>
        public string ReSerialize()
        {
            if (this.document == null)
            {
                return string.Empty; 
            }

            // first make sure that the dictionary is sorted by the element token start index. 
            int currentindex = 0;
            int totallen = document.HtmlSource.Length;

            StringBuilder sb = new StringBuilder();

            foreach (var item in ToBeAdded.OrderBy(o => o.Key.location.openTokenStartIndex))
            {
                if (!string.IsNullOrEmpty(item.Key.tagName) && item.Key.location.openTokenStartIndex > 0 && item.Key.location.openTokenEndIndex > 0)
                {
                    int taglen = item.Key.tagName.Length + 1;

                    string previous = document.HtmlSource.Substring(currentindex, item.Key.location.openTokenStartIndex + taglen - currentindex);

                    currentindex = item.Key.location.openTokenStartIndex + taglen;

                    string koobooattribute; 
                    
                    koobooattribute= "";

                    foreach (var aitem in item.Value)
                    {

                        koobooattribute += " " + aitem.Key;

                        if (!string.IsNullOrEmpty(aitem.Value))
                        {

                            koobooattribute += "=\"" + aitem.Value + "\"";
                        }
                    }

                    sb.Append(previous);
                    sb.Append(koobooattribute);

                }

            }

            if (currentindex < totallen)
            {
                sb.Append(document.HtmlSource.Substring(currentindex, totallen - currentindex));
            }

            return sb.ToString();

        }

    }
}
