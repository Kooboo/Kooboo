//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Tag
{
    public class TagGroup
    {


        private static List<string> _textTag;

        public static List<string> TextTag
        {
            get
            {
                if (_textTag == null)
                {
                    _textTag = new List<string>();
                    string flowcontent = "a abbr address area article aside audio b bdi bdo blockquote br button canvas cite code data datalist del dfn dd dl em embed fieldset figure h1 h2 h3 h4 h5 h6 hr i iframe img input ins kbd keygen label main map mark math meter noscript object ol output p pre progress q ruby s samp script select small span strong sub sup svg textarea time var video wbr text comment";

                    string[] eachitem = flowcontent.Split(' ');

                    foreach (var item in eachitem)
                    {
                        _textTag.Add(item);
                    }
                }
                return _textTag;

            }
        }

        public static bool isText(string TagName)
        {
            /// check whether this is a text node or not. 
            return TextTag.Contains(TagName);

        }

    }


    //    3.2.4.1.1 Metadata content

    //Metadata content is content that sets up the presentation or behavior of the rest of the content, or that sets up the relationship of the document with other documents, or that conveys other "out of band" information.

    //base link meta noscript script style template title
    //Elements from other namespaces whose semantics are primarily metadata-related (e.g. RDF) are also metadata content.

    //Thus, in the XML serialization, one can use RDF, like this:

    //<html xmlns="http://www.w3.org/1999/xhtml"
    //      xmlns:r="http://www.w3.org/1999/02/22-rdf-syntax-ns#">
    // <head>
    //  <title>Hedral's Home Page</title>
    //  <r:RDF>
    //   <Person xmlns="http://www.w3.org/2000/10/swap/pim/contact#"
    //           r:about="http://hedral.example.com/#">
    //    <fullName>Cat Hedral</fullName>
    //    <mailbox r:resource="mailto:hedral@damowmow.com"/>
    //    <personalTitle>Sir</personalTitle>
    //   </Person>
    //  </r:RDF>
    // </head>
    // <body>
    //  <h1>My home page</h1>
    //  <p>I like playing with string, I guess. Sister says squirrels are fun
    //  too so sometimes I follow her to play with them.</p>
    // </body>
    //</html>
    //This isn't possible in the HTML serialization, however.

    //3.2.4.1.2 Flow content

    //Most elements that are used in the body of documents and applications are categorized as flow content.

    //a abbr address area (if it is a descendant of a map element) article aside audio b bdi bdo blockquote br button canvas cite code data datalist del dfn div dl em embed fieldset figure footer form h1 h2 h3 h4 h5 h6 header hr i iframe img input ins kbd keygen label main map mark math meter nav noscript object ol output p pre progress q ruby s samp script section select small span strong sub sup svg table template textarea time u ul var video wbr text
    //3.2.4.1.3 Sectioning content

    //Sectioning content is content that defines the scope of headings and footers.

    //article aside nav section
    //Each sectioning content element potentially has a heading and an outline. See the section on headings and sections for further details.

    //There are also certain elements that are sectioning roots. These are distinct from sectioning content, but they can also have an outline.

    //3.2.4.1.4 Heading content

    //Heading content defines the header of a section (whether explicitly marked up using sectioning content elements, or implied by the heading content itself).

    //h1 h2 h3 h4 h5 h6
    //3.2.4.1.5 Phrasing content

    //Phrasing content is the text of the document, as well as elements that mark up that text at the intra-paragraph level. Runs of phrasing content form paragraphs.

    //a abbr area (if it is a descendant of a map element) audio b bdi bdo br button canvas cite code data datalist del dfn em embed i iframe img input ins kbd keygen label map mark math meter noscript object output progress q ruby s samp script select small span strong sub sup svg template textarea time u var video wbr text
    //Most elements that are categorized as phrasing content can only contain elements that are themselves categorized as phrasing content, not any flow content.

    //Text, in the context of content models, means either nothing, or Text nodes. Text is sometimes used as a content model on its own, but is also phrasing content, and can be inter-element whitespace (if the Text nodes are empty or contain just space characters).

    //Text nodes and attribute values must consist of Unicode characters, must not contain U+0000 characters, must not contain permanently undefined Unicode characters (noncharacters), and must not contain control characters other than space characters. This specification includes extra constraints on the exact value of Text nodes and attribute values depending on their precise context.

    //3.2.4.1.6 Embedded content

    //Embedded content is content that imports another resource into the document, or content from another vocabulary that is inserted into the document.

    //audio canvas embed iframe img math object svg video
    //Elements that are from namespaces other than the HTML namespace and that convey content but not metadata, are embedded content for the purposes of the content models defined in this specification. (For example, MathML, or SVG.)

    //Some embedded content elements can have fallback content: content that is to be used when the external resource cannot be used (e.g. because it is of an unsupported format). The element definitions state what the fallback is, if any.

    //3.2.4.1.7 Interactive content

    //Interactive content is content that is specifically intended for user interaction.

    //a audio (if the controls attribute is present) button embed iframe img (if the usemap attribute is present) input (if the type attribute is not in the hidden state) keygen label object (if the usemap attribute is present) select textarea video (if the controls attribute is present)











}
