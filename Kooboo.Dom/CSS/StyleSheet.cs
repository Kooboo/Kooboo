//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;


namespace Kooboo.Dom.CSS
{

    //http://dev.w3.org/csswg/cssom
    //interface StyleSheet {
    //readonly attribute DOMString type;
    //readonly attribute DOMString? href;
    //readonly attribute (Element or ProcessingInstruction)? ownerNode;
    //readonly attribute StyleSheet? parentStyleSheet;
    //readonly attribute DOMString? title;
    //[SameObject, PutForwards=mediaText] readonly attribute MediaList media;
    //attribute boolean disabled;


    [Serializable]
    public class StyleSheet
    {
        /// <summary>
        /// Return text/css
        /// </summary>
        public string type = "text/css";

        /// <summary>
        /// Specified when created. The absolute URL of the first request of the CSS style sheet
        /// or null if the CSS style sheet was embedded. Does not change during the lifetime of the CSS style sheet.
        /// </summary>
        public string href;

        /// <summary>
        /// Specified when created. The DOM node associated with the CSS style sheet or null if there is no associated DOM node.
        /// </summary>
        public Element ownerNode;

        /// <summary>
        /// Returns a StyleSheet including this one, if any; returns null if there aren't any.
        /// </summary>
        public StyleSheet parentStyleSheet;

        public string title;

        public MediaList Medialist = new MediaList();

        public bool disabled;


        /// <summary>
        /// origin-clean flag. Specified when created. Either set or unset. If it is set, the API allows reading and modifying of the CSS rules.
        /// </summary>
        public bool originClean = true;

        /// <summary>
        /// alternate
        /// Specified when created. Either set or unset. Unset by default.
        //The following CSS style sheets have their alternate flag set:
        //<?xml-stylesheet alternate="yes" title="x" href="data:text/css,…"?>
        //<link rel="alternate stylesheet" title="x" href="data:text/css,…">
        /// </summary>
        public bool alternative;

    }
}
