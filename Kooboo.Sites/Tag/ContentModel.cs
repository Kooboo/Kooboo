//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom;

namespace Kooboo.Sites.Tag
{

    /// <summary>
    /// //See: http://www.w3.org/wiki/HTML/Elements
     //modify to fit real needs. modify h1-6 and div with class + id. 
    /// </summary>
  public static  class ContentModel
    {

      public static EnumContentModel GetContentModel(Element element)
      {

            if (element.tagName == "html")
            {
                return EnumContentModel.root; 
            }
            else if (element.tagName == "body")
            {
                return EnumContentModel.body; 
               }
            else if (ContentModel.MetaList.Contains(element.tagName))
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


        private static List<string> _metalist;
        public static List<string> MetaList
        {
            get
            {
                if (_metalist == null)
                {
                    _metalist = new List<string>();
                    _metalist.Add("head");
                    _metalist.Add("title");
                    _metalist.Add("base");
                    _metalist.Add("isindex");
                    _metalist.Add("link");
                    _metalist.Add("meta");
                    _metalist.Add("style");
                    _metalist.Add("script");
                    _metalist.Add("noscript");

                }
                return _metalist;
            }
        }

        private static  List<string> _sectionlist;
        public static List<string> SectionList
        {
            get
            {
                if (_sectionlist == null)
                {
                    _sectionlist = new List<String>();
                    // _sectionlist.Add("body");
                    _sectionlist.Add("section");
                    _sectionlist.Add("nav");
                    _sectionlist.Add("article");
                    _sectionlist.Add("aside");
                    ///_sectionlist.Add("h1, h2, h3, h4, h5, and h6");
                    _sectionlist.Add("hgroup");
                    _sectionlist.Add("header");
                    _sectionlist.Add("footer");
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
                    _groupinglist = new List<string>();

                    _groupinglist.Add("p");
                    _groupinglist.Add("hr");
                    _groupinglist.Add("pre");
                    _groupinglist.Add("blockquote");
                    _groupinglist.Add("ol");
                    _groupinglist.Add("ul");
                    _groupinglist.Add("li");
                    _groupinglist.Add("dl");
                    _groupinglist.Add("dt");
                    _groupinglist.Add("dd");
                    _groupinglist.Add("figure");
                    _groupinglist.Add("figcaption");
                    _groupinglist.Add("div");
                    _groupinglist.Add("center");


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
                    _textlist = new List<string>();

                    _textlist.Add("a");
                    _textlist.Add("abbr");
                    _textlist.Add("acronym");
                    _textlist.Add("b");
                    _textlist.Add("basefont");
                    _textlist.Add("bdo");
                    _textlist.Add("big");
                    _textlist.Add("blink");
                    _textlist.Add("br");
                    _textlist.Add("cite");
                    _textlist.Add("code");
                    _textlist.Add("dfn");
                    _textlist.Add("em");
                    _textlist.Add("font");
                    _textlist.Add("i");
                    _textlist.Add("kbd");
                    _textlist.Add("listing");
                    _textlist.Add("mark");
                    _textlist.Add("marquee");
                    _textlist.Add("nextid");
                    _textlist.Add("nobr");
                    _textlist.Add("q");
                    _textlist.Add("rp");
                    _textlist.Add("rt");
                    _textlist.Add("ruby");
                    _textlist.Add("s");
                    _textlist.Add("samp");
                    _textlist.Add("small");
                    _textlist.Add("spacer");
                    _textlist.Add("span");
                    _textlist.Add("strike");
                    _textlist.Add("strong");
                    _textlist.Add("sub, sup");
                    _textlist.Add("time");
                    _textlist.Add("tt");
                    _textlist.Add("u");
                    _textlist.Add("var");
                    _textlist.Add("wbr");
                    _textlist.Add("xmp");

                    _textlist.Add("h1");
                    _textlist.Add("h2");
                    _textlist.Add("h3");
                    _textlist.Add("h4");
                    _textlist.Add("h5");
                    _textlist.Add("h6");

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
                    _embeddedlist = new List<string>();

                    _embeddedlist.Add("img");
                    _embeddedlist.Add("iframe");
                    _embeddedlist.Add("embed");
                    _embeddedlist.Add("object");
                    _embeddedlist.Add("param");
                    _embeddedlist.Add("video");
                    _embeddedlist.Add("audio");
                    _embeddedlist.Add("source");
                    _embeddedlist.Add("track");
                    _embeddedlist.Add("canvas");
                    _embeddedlist.Add("map");
                    _embeddedlist.Add("area");
                    _embeddedlist.Add("math");
                    _embeddedlist.Add("svg");
                    _embeddedlist.Add("applet");
                    _embeddedlist.Add("frame");
                    _embeddedlist.Add("frameset");
                    _embeddedlist.Add("noframes");
                    _embeddedlist.Add("bgsound");
                    _embeddedlist.Add("noembed");
                    _embeddedlist.Add("plaintext");


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
                    _tablelist = new List<string>();
                    _tablelist.Add("table");
                    _tablelist.Add("caption");
                    _tablelist.Add("colgroup");
                    _tablelist.Add("col");
                    _tablelist.Add("tbody");
                    _tablelist.Add("thead");
                    _tablelist.Add("tfoot");
                    _tablelist.Add("tr");
                    _tablelist.Add("td");
                    _tablelist.Add("th");
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
                    _formlist = new List<String>();

                    _formlist.Add("form");
                    _formlist.Add("fieldset");
                    _formlist.Add("legend");
                    _formlist.Add("label");
                    _formlist.Add("input");
                    _formlist.Add("button");
                    _formlist.Add("select");
                    _formlist.Add("datalist");
                    _formlist.Add("optgroup");
                    _formlist.Add("option");
                    _formlist.Add("textarea");
                    _formlist.Add("keygen");
                    _formlist.Add("output");
                    _formlist.Add("progress");
                    _formlist.Add("meter");


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
                    _interactive = new List<string>();
                    _interactive.Add("details");
                    _interactive.Add("summary");
                    _interactive.Add("command");
                    _interactive.Add("menu");

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
                    _edit = new List<string>();
                    _edit.Add("ins");
                    _edit.Add("del");
                }
                return _edit;
            }

        }

    }



    public  enum EnumContentModel
    {
         root=0,
        body =1,
        metadata=2,
        section =3,
        grouping = 4,
        text = 5,
        embedded = 6,
        table =7,
        form = 8,
        interactive =9,
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





