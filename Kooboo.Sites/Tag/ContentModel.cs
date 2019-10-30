//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Tag
{
    // <summary>
    // //See: http://www.w3.org/wiki/HTML/Elements
    //modify to fit real needs. modify h1-6 and div with class + id.
    // </summary>
    public static class ContentModel
    {
        public static EnumContentModel GetContentModel(Element element)
        {
            switch (element.tagName)
            {
                case "html":
                    return EnumContentModel.root;
                case "body":
                    return EnumContentModel.body;
                default:
                {
                    if (ContentModel.MetaList.Contains(element.tagName))
                    {
                        return EnumContentModel.metadata;
                    }
                    else if (SectionList.Contains(element.tagName))
                    {
                        return EnumContentModel.section;
                    }
                    else if (element.tagName == "div" && (element.hasAttribute("class") || element.hasAttribute("id")))
                    {
                        // when a div has a class or id, it upgraded to a section element.
                        return EnumContentModel.section;
                    }
                    else if (GroupingList.Contains(element.tagName))
                    {
                        return EnumContentModel.grouping;
                    }
                    else if (TextList.Contains(element.tagName))
                    {
                        return EnumContentModel.text;
                    }
                    else if (EmbeddedList.Contains(element.tagName))
                    {
                        return EnumContentModel.embedded;
                    }
                    else if (TableList.Contains(element.tagName))
                    {
                        return EnumContentModel.table;
                    }
                    else if (FormList.Contains(element.tagName))
                    {
                        return EnumContentModel.form;
                    }
                    else if (Interactive.Contains(element.tagName))
                    {
                        return EnumContentModel.interactive;
                    }
                    else if (Edit.Contains(element.tagName))
                    {
                        return EnumContentModel.edit;
                    }
                    else
                    {
                        return EnumContentModel.unknown;
                    }
                }
            }
        }

        private static List<string> _metalist;

        public static List<string> MetaList
        {
            get
            {
                if (_metalist == null)
                {
                    _metalist = new List<string>
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
                    };
                }
                return _metalist;
            }
        }

        private static List<string> _sectionlist;

        public static List<string> SectionList
        {
            get
            {
                if (_sectionlist == null)
                {
                    _sectionlist = new List<String>
                    {
                        "section",
                        "nav",
                        "article",
                        "aside",
                        "hgroup",
                        "header",
                        "footer"
                    };
                    // _sectionlist.Add("body");
                    //_sectionlist.Add("h1, h2, h3, h4, h5, and h6");
                    //() header
                    //() footer
                    //() address
                }

                return _sectionlist;
            }
        }

        private static List<string> _groupinglist;

        public static List<string> GroupingList
        {
            get
            {
                if (_groupinglist == null)
                {
                    _groupinglist = new List<string>
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
                        "center"
                    };

                }

                return _groupinglist;
            }
        }

        private static List<string> _textlist;

        public static List<string> TextList
        {
            get
            {
                if (_textlist == null)
                {
                    _textlist = new List<string>
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
                        "sub, sup",
                        "time",
                        "tt",
                        "u",
                        "var",
                        "wbr",
                        "xmp",
                        "h1",
                        "h2",
                        "h3",
                        "h4",
                        "h5",
                        "h6"
                    };


                }

                return _textlist;
            }
        }

        private static List<string> _embeddedlist;

        public static List<string> EmbeddedList
        {
            get
            {
                if (_embeddedlist == null)
                {
                    _embeddedlist = new List<string>
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
                    };

                }

                return _embeddedlist;
            }
        }

        private static List<string> _tablelist;

        public static List<string> TableList
        {
            get
            {
                if (_tablelist == null)
                {
                    _tablelist = new List<string>
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
                    };
                }

                return _tablelist;
            }
        }

        private static List<string> _formlist;

        public static List<string> FormList
        {
            get
            {
                if (_formlist == null)
                {
                    _formlist = new List<String>
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
                    };

                }
                return _formlist;
            }
        }

        private static List<string> _interactive;

        public static List<string> Interactive
        {
            get
            {
                if (_interactive == null)
                {
                    _interactive = new List<string> {"details", "summary", "command", "menu"};
                }
                return _interactive;
            }
        }

        private static List<string> _edit;

        public static List<string> Edit
        {
            get
            {
                if (_edit == null)
                {
                    _edit = new List<string> {"ins", "del"};
                }
                return _edit;
            }
        }
    }

    public enum EnumContentModel
    {
        root = 0,
        body = 1,
        metadata = 2,
        section = 3,
        grouping = 4,
        text = 5,
        embedded = 6,
        table = 7,
        form = 8,
        interactive = 9,
        edit = 10,
        unknown = 11
    }
}

//The root element

//() html
//Document metadata

//() head
//() title
//() base
//() isindex
//() link
//() meta
//() style
//Scripting

//() script
//() noscript
//Sections

//() body
//() section
//() nav
//() article
//() aside
//() h1, h2, h3, h4, h5, and h6
//() hgroup
//() header
//() footer
//() address
//Grouping content

//() p
//() hr
//() pre
//() blockquote
//() ol
//() ul
//() li
//() dl
//() dt
//() dd
//() figure
//() figcaption
//() div
//() center
//() main
//Text-level semantics

//() a
//() abbr
//() acronym
//() b
//() basefont
//() bdo
//() big
//() blink
//() br
//() cite
//() code
//() dfn
//() em
//() font
//() i
//() kbd
//() listing
//() mark
//() marquee
//() nextid
//() nobr
//() q
//() rp
//() rt
//() ruby
//() s
//() samp
//() small
//() spacer
//() span
//() strike
//() strong
//() sub, sup
//() time
//() tt
//() u
//() var
//() wbr
//() xmp
//Edits

//() ins
//() del
//Embedded content

//() img
//() iframe
//() embed
//() object
//() param
//() video
//() audio
//() source
//() track
//() canvas
//() map
//() area
//() math
//() svg
//() applet
//() frame
//() frameset
//() noframes
//() bgsound
//() noembed
//() plaintext
//Tables

//() table
//() caption
//() colgroup
//() col
//() tbody
//() thead
//() tfoot
//() tr
//() td
//() th
//Forms

//() form
//() fieldset
//() legend
//() label
//() input
//() button
//() select
//() datalist
//() optgroup
//() option
//() textarea
//() keygen
//() output
//() progress
//() meter
//Interactive

//() details
//() summary
//() command
//() menu