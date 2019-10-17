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
            return _metaData ?? (_metaData = new List<string>
            {
                "head",
                "title",
                "base",
                "isindex",
                "link",
                "meta",
                "style",
                "script",
                "noscript"
            });
        }

        private static List<string> _Sections;

        public static List<string> sections()
        {
            return _Sections ?? (_Sections = new List<string>
            {
                "body",
                "section",
                "nav",
                "article",
                "aside",
                "h1",
                "h2",
                "h3",
                "h4",
                "h5",
                "h6",
                "hgroup",
                "header",
                "footer",
                "address"
            });
        }

        private static List<string> _GroupingContent;

        public static List<string> groupingContent()
        {
            return _GroupingContent ?? (_GroupingContent = new List<string>
            {
                "p",
                "hr",
                "pre",
                "blockquote",
                "ol",
                "ul",
                "li",
                "dl",
                "dt",
                "dd",
                "figure",
                "figcaption",
                "div",
                "center",
                "main"
            });
        }

        private static List<string> _textLevel;

        public static List<string> textLevel()
        {
            return _textLevel ?? (_textLevel = new List<string>
            {
                "a",
                "abbr",
                "acronym",
                "b",
                "basefont",
                "bdo",
                "big",
                "blink",
                "br",
                "cite",
                "code",
                "dfn",
                "em",
                "font",
                "i",
                "kbd",
                "listing",
                "mark",
                "marquee",
                "nextid",
                "nobr",
                "q",
                "rp",
                "rt",
                "ruby",
                "s",
                "samp",
                "small",
                "spacer",
                "span",
                "strike",
                "strong",
                "sub",
                "sup",
                "time",
                "tt",
                "u",
                "var",
                "wbr",
                "xmp"
            });
        }

        private static List<string> _edit;

        public static List<string> edit()
        {
            return _edit ?? (_edit = new List<string> {"ins", "del"});
        }

        private static List<string> _embedded;

        public static List<string> embedded()
        {
            return _embedded ?? (_embedded = new List<string>
            {
                "img",
                "iframe",
                "embed",
                "object",
                "param",
                "video",
                "audio",
                "source",
                "track",
                "canvas",
                "map",
                "area",
                "math",
                "svg",
                "applet",
                "frame",
                "frameset",
                "noframes",
                "bgsound",
                "noembed",
                "plaintext"
            });
        }

        private static List<string> _table;

        public static List<string> table()
        {
            return _table ?? (_table = new List<string>
            {
                "table",
                "caption",
                "colgroup",
                "col",
                "tbody",
                "thead",
                "tfoot",
                "tr",
                "td",
                "th"
            });
        }

        private static List<string> _forms;

        public static List<string> forms()
        {
            return _forms ?? (_forms = new List<string>
            {
                "form",
                "fieldset",
                "legend",
                "label",
                "input",
                "button",
                "select",
                "datalist",
                "optgroup",
                "option",
                "textarea",
                "keygen",
                "output",
                "progress",
                "meter"
            });
        }

        private static List<string> _interactive;

        public static List<string> interactive()
        {
            return _interactive ?? (_interactive = new List<string> {"details", "summary", "command", "menu"});
        }
    }
}