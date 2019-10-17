//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Dom.Dom
{
    public class HtmlElementContentModel
    {
        //http://www.w3.org/TR/html5/dom.html#content-models
        //3.2.4.1 Kinds of content
        //Each element in HTML falls into zero or more categories that group elements with similar characteristics together. The following broad categories are used in this specification:
        //Metadata content
        //Flow content
        //Sectioning content
        //Heading content
        //Phrasing content
        //Embedded content
        //Interactive content

        private static List<string> _metadata;

        /// <summary>
        /// Metadata content is content that sets up the presentation or behavior of the rest of the content, or that sets up the relationship of the document with other documents, or that conveys other "out of band" information.
        /// </summary>
        /// <returns></returns>
        public static List<string> MetaData()
        {
            // base link meta noscript script style template title
            // base and title can only have one, the rest can have multiple.
            return _metadata ?? (_metadata = new List<string>
            {
                "base",
                "link",
                "meta",
                "noscript",
                "script",
                "style",
                "template",
                "title"
            });
        }

        private static List<string> _flow;

        /// <summary>
        /// Most elements that are used in the body of documents and applications are categorized as flow content.
        /// </summary>
        /// <returns></returns>
        public static List<string> Flow()
        {
            //a abbr address area (if it is a descendant of a map element) article aside audio b bdi bdo blockquote
            //br button canvas cite code data datalist del dfn div dl em embed fieldset figure footer form
            //h1 h2 h3 h4 h5 h6 header hr i iframe img input ins kbd keygen label main map mark math meter nav
            // noscript object ol output p pre progress q ruby s samp script section select small span strong
            //sub sup svg table template textarea time u ul var video wbr text
            if (_flow == null)
            {
                _flow = new List<string>();
                string flowcontentelement = "a abbr address area article aside audio b bdi bdo blockquote br button canvas cite code data datalist del dfn div dl em embed fieldset figure footer form h1 h2 h3 h4 h5 h6 header hr i iframe img input ins kbd keygen label main map mark math meter nav noscript object ol output p pre progress q ruby s samp script section select small span strong sub sup svg table template textarea time u ul var video wbr text";
                flowcontentelement = flowcontentelement.Replace("  ", " ");
                string[] stringarray = flowcontentelement.Split(' ');

                foreach (var item in stringarray)
                {
                    _flow.Add(item);
                }
            }

            return _flow;
        }

        private static List<string> _sectioning;

        /// <summary>
        /// Sectioning content is content that defines the scope of headings and footers.
        /// </summary>
        /// <returns></returns>
        public static List<string> Sectioning()
        {
            // 3.2.4.1.3 Sectioning content
            //Sectioning content is content that defines the scope of headings and footers.
            //article aside nav section
            //Each sectioning content element potentially has a heading and an outline. See the section on headings and sections for further details.
            //There are also certain elements that are sectioning roots. These are distinct from sectioning content, but they can also have an outline.

            return _sectioning ?? (_sectioning = new List<string> {"article", "aside", "nav", "section"});
        }

        private static List<string> _heading;

        /// <summary>
        ///  Heading content defines the header of a section (whether explicitly marked up using sectioning content elements, or implied by the heading content itself).
        /// </summary>
        /// <returns></returns>
        public static List<string> Heading()
        {
            return _heading ?? (_heading = new List<string>
            {
                "h1",
                "h2",
                "h3",
                "h4",
                "h5",
                "h6"
            });
        }

        private static List<string> _phrase;

        /// <summary>
        /// Phrasing content is the text of the document, as well as elements that mark up that text at the intra-paragraph level. Runs of phrasing content form paragraphs.
        /// Most elements that are categorized as phrasing content can only contain elements that are themselves categorized as phrasing content, not any flow content.
        /// </summary>
        /// <returns></returns>
        public static List<string> Phrase()
        {
            //a abbr area (if it is a descendant of a map element) audio b bdi bdo br button canvas cite code data datalist del dfn em embed i iframe img input ins kbd keygen label map mark math meter noscript object output progress q ruby s samp script select small span strong sub sup svg template textarea time u var video wbr text

            if (_phrase == null)
            {
                _phrase = new List<string>();

                string phrase = "a abbr area audio b bdi bdo br button canvas cite code data datalist del dfn em embed i iframe img input ins kbd keygen label map mark math meter noscript object output progress q ruby s samp script select small span strong sub sup svg template textarea time u var video wbr text";
                phrase = phrase.Replace("  ", " ");
                string[] stringarray = phrase.Split(' ');

                foreach (var item in stringarray)
                {
                    _phrase.Add(item);
                }
            }

            return _phrase;
        }

        private static List<string> _embedded;

        /// <summary>
        /// Embedded content is content that imports another resource into the document, or content from another vocabulary that is inserted into the document.
        /// </summary>
        /// <returns></returns>
        public static List<string> Embedded()
        {
            //audio canvas embed iframe img math object svg video
            return _embedded ?? (_embedded = new List<string>
            {
                "audio",
                "canvas",
                "embed",
                "iframe",
                "img",
                "math",
                "object",
                "svg",
                "video"
            });
        }

        private static List<string> _interactive;

        /// <summary>
        /// Interactive content is content that is specifically intended for user interaction.
        /// </summary>
        /// <returns></returns>
        public static List<string> Interactive()
        {
            ///a audio (if the controls attribute is present) button embed iframe img (if the usemap attribute is present) input (if the type attribute is not in the hidden state) keygen label object (if the usemap attribute is present) select textarea video (if the controls attribute is present)
            return _interactive ?? (_interactive = new List<string>
            {
                "a",
                "audio",
                "button",
                "embed",
                "iframe",
                "img",
                "input",
                "keygen",
                "label",
                "object",
                "select",
                "textarea",
                "video"
            });
        }
    }
}