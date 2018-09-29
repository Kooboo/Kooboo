//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Language
{
    public static class LanguageTaskHelper
    {

        public static List<LanguageTask> ParseDom(string input)
        {
            List<LanguageTask> tasklist = new List<LanguageTask>();
            if (string.IsNullOrEmpty(input))
            {
                return tasklist;
            }
            var doc = DomParser.CreateDom(input);
            int currentindex = 0;
            int totallen = input.Length;

            // var iterator = doc.createNodeIterator(doc.documentElement, enumWhatToShow.TEXT | enumWhatToShow.ELEMENT, null);
            var iterator = doc.createNodeIterator(doc.documentElement, enumWhatToShow.TEXT | enumWhatToShow.ELEMENT, null);

            var nextnode = iterator.nextNode(); 
            while (nextnode != null)
            {  
                if (nextnode.nodeType == enumNodeType.TEXT)
                {
                    var textndoe = nextnode as Kooboo.Dom.Text;

                    if (textndoe != null && MultiLingualHelper.IsMultilingualKey(textndoe.data))
                    {
                        string key = textndoe.data;

                        int len = nextnode.location.openTokenStartIndex - currentindex;
                        if (len > 0)
                        {
                            tasklist.Add(new LanguageTask(input.Substring(currentindex, len), false));
                        }
                        tasklist.Add(new LanguageTask(key, true));
                        currentindex = nextnode.location.endTokenEndIndex + 1;
                    }  
                }
                else if (nextnode.nodeType == enumNodeType.ELEMENT)
                {
                    var el = nextnode as Element;

                    if (el.tagName == "script" || el.tagName == "link" || el.tagName == "style" || el.tagName == "meta")
                    {
                        nextnode = iterator.NextSibling(nextnode);
                        continue; 
                    }
           
                    string placeholder = el.getAttribute("placeholder");
                    string title = el.getAttribute("title");

                    if (MultiLingualHelper.IsMultilingualKey(title) || MultiLingualHelper.IsMultilingualKey(placeholder))
                    {
                        int len = nextnode.location.openTokenStartIndex - currentindex;
                        string substring = string.Empty; 
                        if (len > 0)
                        {
                            substring = input.Substring(currentindex, len);  
                        }
                        el.removeAttribute("placeholder");
                        el.removeAttribute("title");

                        substring += Helper.DomHelper.GetHalfOpenTag(el);
                        tasklist.Add(new LanguageTask(substring, false));

                        if (!string.IsNullOrEmpty(title))
                        {
                            string titleatt =  " title=\"";
                            tasklist.Add(new LanguageTask(titleatt, false));
                            tasklist.Add(new LanguageTask(title, true));
                            tasklist.Add(new LanguageTask("\"", false));
                        }

                        if (!string.IsNullOrWhiteSpace(placeholder))
                        {
                            string placeatt = " placeholder=\"";
                            tasklist.Add(new LanguageTask(placeatt, false));
                            tasklist.Add(new LanguageTask(placeholder, true));
                            tasklist.Add(new LanguageTask("\"", false));
                        }

                        if (Helper.DomHelper.IsSelfCloseTag(el.tagName))
                        {
                            tasklist.Add(new LanguageTask(" />", false)); 
                        }
                        else
                        {
                            tasklist.Add(new LanguageTask(">", false)); 
                        }

                        currentindex = nextnode.location.openTokenEndIndex + 1;
                    } 
                }
                nextnode = iterator.nextNode();
            }

            if (currentindex < totallen - 1)
            {
                tasklist.Add(new LanguageTask(doc.HtmlSource.Substring(currentindex, totallen - currentindex), false));
            }

            OptimizeTask(tasklist); 
            return tasklist;
        }

        public static string Render(List<LanguageTask> tasklist, string langcode = null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in tasklist)
            {
                sb.Append(item.Render(langcode));
            }
            return sb.ToString();
        }


        public static List<LanguageTask> ParseJs(string jstext)
        {

            List<LanguageTask> tasklist = new List<LanguageTask>();
            if (string.IsNullOrEmpty(jstext))
            {
                return tasklist;
            }

            int currentindex = 0;
            int len = jstext.Length;

            string value = string.Empty;
            bool inValue = false;

            for (int i = 0; i < len; i++)
            {
                var onechar = jstext[i];

                if (inValue)
                {
                    if (onechar == '"')
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {

                            tasklist.Add(new LanguageTask(value, true));
                            value = string.Empty;
                            inValue = false;
                            currentindex = i;
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

                        int sublen = i - currentindex + 1;
                        if (sublen > 0)
                        {
                            tasklist.Add(new LanguageTask(jstext.Substring(currentindex, sublen), false));
                        }
                    }
                }
            }


            if (currentindex <= len - 1)
            {
                tasklist.Add(new LanguageTask(jstext.Substring(currentindex, len - currentindex), false));
            }
            return tasklist;

        }


        private static void OptimizeTask(List<LanguageTask> task)
        {
            int count = task.Count();
            bool IsPreviousContent = false;
            int lastContentI = -1;
            List<int> ToRemoved = new List<int>();
            for (int i = 0; i < count; i++)
            {
                var item = task[i];

                if (!string.IsNullOrEmpty(item.Content))
                {
                    if (IsPreviousContent && lastContentI != -1)
                    { 
                        var lastcontenttask = task[lastContentI];
                        lastcontenttask.Content += item.Content;
                        ToRemoved.Add(i);
                    }
                    IsPreviousContent = true;
                    if (lastContentI == -1)
                    {
                        lastContentI = i;
                    }
                }

                else
                {
                    IsPreviousContent = false;
                    lastContentI = -1;
                }
            }

            ToRemoved.Reverse();
            foreach (var item in ToRemoved)
            {
                task.RemoveAt(item);
            }
        }

    }
}
