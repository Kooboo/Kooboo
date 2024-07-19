//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom
{
    public static class DomParser
    {

        public static Document CreateDom(string htmlText)
        {
            TreeConstruction treeParser = new TreeConstruction();
            Document doc = treeParser.Parse(htmlText);
            doc.HtmlSource = htmlText;
            return doc;
        }

        public static Document CreateDom(string htmlText, List<string> ParserErrors)
        {
            TreeConstruction treeParser = new TreeConstruction();
            treeParser.EnableErrorLogging = true;
            Document doc = treeParser.Parse(htmlText);
            doc.HtmlSource = htmlText;

            ParserErrors.AddRange(_getErrors(treeParser.Errors, htmlText));
            return doc;
        }

        private static List<string> _getErrors(Dictionary<int, string> errors, string htmlsource)
        {
            List<string> ErrList = new List<string>();
            int currentposition = 0;

            int totallines = 1;

            foreach (var item in errors)
            {
                int takecount = item.Key - currentposition;

                int lines = htmlsource.Skip(currentposition).Take(takecount).Count(o => o == '\n');

                totallines = totallines + lines;

                currentposition = item.Key;

                string newerr = "line " + totallines.ToString() + ". " + item.Value;
                ErrList.Add(newerr);
            }
            return ErrList;
        }

        /// <summary>
        /// create dom and apply css to dom elements
        /// </summary>
        /// <param name="FullUrlOrPath"></param>
        /// <param name="applyCss">parse and apply css to dom elements</param>
        /// <returns></returns>
        public static Document CreateDomFromUri(string FullUrlOrPath)
        {
            string htmlstring = Loader.LoadHtml(FullUrlOrPath);

            if (string.IsNullOrEmpty(htmlstring))
            {
                return null;
            }

            Document doc = CreateDom(htmlstring);
            doc.URL = FullUrlOrPath;

            return doc;
        }

        public static NodeList ParseFragment(string input, Element context)
        {
            TreeConstruction treeParser = new TreeConstruction();
            return treeParser.ParseFragment(input, context);
        }

        public static NodeList ParseFragment(string input)
        {

            TreeConstruction treeParser = new TreeConstruction();
            treeParser.doc.HtmlSource = input;
            return treeParser.ParseFragment(input);

        }

    }
}
