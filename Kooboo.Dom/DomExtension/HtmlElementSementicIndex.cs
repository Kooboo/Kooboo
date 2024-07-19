//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Dom.Dom
{
    public class HtmlElementSementicIndex
    {

        //from w3c wiki page. 

        private static List<string> _metaData;
        public static List<string> metaData()
        {
            ///script and style are also called raw text element. 
            if (_metaData == null)
            {
                _metaData = new List<string>();

                _metaData.Add("head");
                _metaData.Add("title");
                _metaData.Add("base");
                _metaData.Add("isindex");
                _metaData.Add("link");
                _metaData.Add("meta");
                _metaData.Add("style");
                _metaData.Add("script");
                _metaData.Add("noscript");
            }

            return _metaData;
        }

        private static List<string> _Sections;
        public static List<string> sections()
        {
            if (_Sections == null)
            {
                _Sections = new List<string>();
                _Sections.Add("body");
                _Sections.Add("section");
                _Sections.Add("nav");
                _Sections.Add("article");
                _Sections.Add("aside");
                _Sections.Add("h1");
                _Sections.Add("h2");
                _Sections.Add("h3");
                _Sections.Add("h4");
                _Sections.Add("h5");
                _Sections.Add("h6");
                _Sections.Add("hgroup");
                _Sections.Add("header");
                _Sections.Add("footer");
                _Sections.Add("address");
            }

            return _Sections;

        }

        private static List<string> _GroupingContent;
        public static List<string> groupingContent()
        {
            if (_GroupingContent == null)
            {
                _GroupingContent = new List<string>();
                _GroupingContent.Add("p");
                _GroupingContent.Add("hr");
                _GroupingContent.Add("pre");
                _GroupingContent.Add("blockquote");
                _GroupingContent.Add("ol");
                _GroupingContent.Add("ul");
                _GroupingContent.Add("li");
                _GroupingContent.Add("dl");
                _GroupingContent.Add("dt");
                _GroupingContent.Add("dd");
                _GroupingContent.Add("figure");
                _GroupingContent.Add("figcaption");
                _GroupingContent.Add("div");
                _GroupingContent.Add("center");
                _GroupingContent.Add("main");
            }

            return _GroupingContent;

        }

        private static List<string> _textLevel;
        public static List<string> textLevel()
        {
            if (_textLevel == null)
            {
                _textLevel = new List<string>();
                _textLevel.Add("a");
                _textLevel.Add("abbr");
                _textLevel.Add("acronym");
                _textLevel.Add("b");
                _textLevel.Add("basefont");
                _textLevel.Add("bdo");
                _textLevel.Add("big");
                _textLevel.Add("blink");
                _textLevel.Add("br");
                _textLevel.Add("cite");
                _textLevel.Add("code");
                _textLevel.Add("dfn");
                _textLevel.Add("em");
                _textLevel.Add("font");
                _textLevel.Add("i");
                _textLevel.Add("kbd");
                _textLevel.Add("listing");
                _textLevel.Add("mark");
                _textLevel.Add("marquee");
                _textLevel.Add("nextid");
                _textLevel.Add("nobr");
                _textLevel.Add("q");
                _textLevel.Add("rp");
                _textLevel.Add("rt");
                _textLevel.Add("ruby");
                _textLevel.Add("s");
                _textLevel.Add("samp");
                _textLevel.Add("small");
                _textLevel.Add("spacer");
                _textLevel.Add("span");
                _textLevel.Add("strike");
                _textLevel.Add("strong");
                _textLevel.Add("sub");
                _textLevel.Add("sup");
                _textLevel.Add("time");
                _textLevel.Add("tt");
                _textLevel.Add("u");
                _textLevel.Add("var");
                _textLevel.Add("wbr");
                _textLevel.Add("xmp");

            }
            return _textLevel;

        }

        private static List<string> _edit;
        public static List<string> edit()
        {
            if (_edit == null)
            {
                _edit = new List<string>();
                _edit.Add("ins");
                _edit.Add("del");
            }
            return _edit;
        }

        private static List<string> _embedded;
        public static List<string> embedded()
        {
            if (_embedded == null)
            {
                _embedded = new List<string>();
                _embedded.Add("img");
                _embedded.Add("iframe");
                _embedded.Add("embed");
                _embedded.Add("object");
                _embedded.Add("param");
                _embedded.Add("video");
                _embedded.Add("audio");
                _embedded.Add("source");
                _embedded.Add("track");
                _embedded.Add("canvas");
                _embedded.Add("map");
                _embedded.Add("area");
                _embedded.Add("math");
                _embedded.Add("svg");
                _embedded.Add("applet");
                _embedded.Add("frame");
                _embedded.Add("frameset");
                _embedded.Add("noframes");
                _embedded.Add("bgsound");
                _embedded.Add("noembed");
                _embedded.Add("plaintext");

            }
            return _embedded;
        }

        private static List<string> _table;
        public static List<string> table()
        {
            if (_table == null)
            {
                _table = new List<string>();
                _embedded.Add("table");
                _embedded.Add("caption");
                _embedded.Add("colgroup");
                _embedded.Add("col");
                _embedded.Add("tbody");
                _embedded.Add("thead");
                _embedded.Add("tfoot");
                _embedded.Add("tr");
                _embedded.Add("td");
                _embedded.Add("th");
            }

            return _table;

        }

        private static List<string> _forms;
        public static List<string> forms()
        {
            if (_forms == null)
            {
                _forms = new List<string>();
                _forms.Add("form");
                _forms.Add("fieldset");
                _forms.Add("legend");
                _forms.Add("label");
                _forms.Add("input");
                _forms.Add("button");
                _forms.Add("select");
                _forms.Add("datalist");
                _forms.Add("optgroup");
                _forms.Add("option");
                _forms.Add("textarea");
                _forms.Add("keygen");
                _forms.Add("output");
                _forms.Add("progress");
                _forms.Add("meter");

            }

            return _forms;
        }

        private static List<string> _interactive;
        public static List<string> interactive()
        {
            if (_interactive == null)
            {
                _interactive = new List<string>();
                _interactive.Add("details");
                _interactive.Add("summary");
                _interactive.Add("command");
                _interactive.Add("menu");
            }
            return _interactive;
        }

    }
}
