//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.IO; 

namespace Kooboo.Data.Language
{
    public static class MultiLingualHelper
    {
   
        public static HashSet<string> GetDomKeys(string html)
        {
            HashSet<string> result = new HashSet<string>();

            var keys = GetDomText(html);
            foreach (var item in keys)
            {
                if (IsMultilingualKey(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }
        private static HashSet<string> GetDomText(string html)
        {
            var dom = Kooboo.Dom.DomParser.CreateDom(html);
            HashSet<string> keys = new HashSet<string>();
            _getDomText(dom.documentElement, ref keys);
            return keys;
        }


        private static void _getDomText(Node node, ref HashSet<string> keys)
        {
            if (node.nodeType == enumNodeType.TEXT)
            {
                var text = node as Kooboo.Dom.Text;
                if (!string.IsNullOrWhiteSpace(text.data))
                {
                    string value = Kooboo.Lib.Helper.StringHelper.TrimSpace(text.data);
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        keys.Add(value);
                    }
                }
            }
            else if (node.nodeType == enumNodeType.ELEMENT)
            {
                var el = node as Element;
                string placeholder = el.getAttribute("placeholder");
                if (!string.IsNullOrWhiteSpace(placeholder))
                {
                    keys.Add(placeholder);
                }

                string title = el.getAttribute("title");
                if (!string.IsNullOrWhiteSpace(title))
                {
                    keys.Add(title);
                }

                if (el.tagName != "script" && el.tagName != "link" && el.tagName != "style" && el.tagName != "meta")
                {
                    foreach (var item in node.childNodes.item)
                    {
                        _getDomText(item, ref keys);
                    }
                }
            }
        }

        public static bool IsMultilingualKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            key = key.ToLower(); 

            if (key.StartsWith("<%") && key.EndsWith("%>"))
            {
                return false;
            }
            if (key.StartsWith("<!--") && key.EndsWith("-->"))
            {
                return false;
            }
            if (key == "&lt;" || key == "&lt;/" || key == "&gt;" || key == "/&gt;")
            {
                return false;
            }

            if (key == "&nbsp;" || key == "@media" || key == "@import")
            {
                return false; 
            }

            if (key.StartsWith("https://") || key.StartsWith("http://") || key.StartsWith("mailto:"))
            {
                return false; 
            }
           
              
            bool hasascii = false;
            int len = key.Length;
            for (int i = 0; i < len; i++)
            {
                if (Kooboo.Dom.CommonIdoms.isAscii(key[i]))
                {
                    hasascii = true;
                    break;
                }
            }
            if (!hasascii)
            {
                return false;
            }

            return true;
        }

        public static HashSet<string> GetJsKeys(string input)
        {
            int len = input.Length;

            HashSet<string> Result = new HashSet<string>();

            string value = string.Empty;
            bool inValue = false;

            for (int i = 0; i < len; i++)
            {
                var onechar = input[i];

                if (inValue)
                {
                    if (onechar == '"')
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            // check if next is a : 
                            bool isJsKey = false;
                            int lookaheadmax = i + 7;
                            for (int j = i +1; j < lookaheadmax; j++)
                            {
                                if (j > len -1)
                                {
                                    break; 
                                }
                                var jitem = input[j]; 
                                if (jitem == ':')
                                {
                                    isJsKey = true;
                                    break; 
                                }
                                
                                if (!Lib.Helper.CharHelper.isSpaceCharacters(jitem))
                                {
                                    break; 
                                }

                            }
                            
                            if (!isJsKey)
                            {
                                Result.Add(value);
                            } 
                            value = string.Empty;
                            inValue = false;
                        }
                    }
                    else
                    {
                        value += onechar;
                    }
                }
                else
                {
                    if (onechar == '"')
                    {
                        inValue = true;
                    }
                }
            }

            return Result;
        }
         
        public static HashSet<string> GetHardCodeValue(string text)
        {
            string method = "Hardcoded.GetValue(";
            int methodlen = method.Length;

            HashSet<string> result = new HashSet<string>();

            int totallen = text.Length;
            int currentindex = 0;

            while (true)
            {
                int next = text.IndexOf(method, currentindex);

                if (next == -1)
                {
                    break;
                }

                int EndBracket = text.IndexOf(")", next);

                if (EndBracket == -1)
                {
                    break;
                }

                currentindex = EndBracket;
                int start = next + methodlen;

                string middlestring = text.Substring(start, EndBracket - start);

                string key = _getStringKey(middlestring);

                 if (!string.IsNullOrEmpty(key))
                {
                    result.Add(key);
                } 
            }

            return result; 
        }
          
        private static string _getStringKey(string input)
        {
            int len = input.Length;

            bool InValue = false;
            string value = string.Empty; 

            for (int i = 0; i < len; i++)
            {
                var onechar = input[i]; 
                  
                if (onechar == '"')
                {
                    if (!InValue)
                    {
                        InValue = true;
                        continue; 
                    }
                    else
                    {
                        if (i == len-1)
                        {
                            return value; 
                        }

                        var nextchar = input[i + 1]; 
                        if (nextchar == '"')
                        {
                            value += '"';
                            i = i + 1; 
                        }
                        else
                        {
                            return value; 
                        }  
                    }
                } 
                else
                {
                    value = value + onechar; 
                    if (!InValue)
                    {
                        InValue = true; 
                    }
                }
            }
            return value; 
        }


        public static Dictionary<string, string> Deserialize(string alltext)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
          
            try
            {
                XElement rootElement = XElement.Parse(alltext); 
                foreach (var el in rootElement.Elements())
                {
                    var id = el.Attribute("id").Value;
                    if (!string.IsNullOrWhiteSpace(id))
                    { 
                        result[id] = el.Value;
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static string Serialize(Dictionary<string, string> langtext)
        {
            
            XElement root = new XElement("Lang");
              
            foreach (var pair in langtext)
            {
                XElement cElement = new XElement("key");
                cElement.Value = pair.Value; 

                cElement.SetAttributeValue("id", pair.Key);
                root.Add(cElement);
            } 
            return root.ToString();
        }
         
    }
}
