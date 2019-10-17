//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Dom
{
    public class documentMode
    {
        public static bool checkQuirkMode(Document doc, DocumentType doctype)
        {
            if (doc.iframeSrcDoc)
            {
                return false;
            }

            //Then, if the document is not an iframe srcdoc document, and the DOCTYPE token matches one of the conditions in the following list, then set the Document to quirks mode:

            //The force-quirks flag is set to on.

            if (doc.isQuirksMode)
            {
                return true;
            }

            //The name is set to anything other than "html" (compared case-sensitively).

            if (string.Equals(doctype.name, "html", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            //The public identifier starts with: "+//Silmaril//dtd html Pro v0r11 19970101//"
            //The public identifier starts with: "-//AdvaSoft Ltd//DTD HTML 3.0 asWedit + extensions//"
            //The public identifier starts with: "-//AS//DTD HTML 3.0 asWedit + extensions//"
            //The public identifier starts with: "-//IETF//DTD HTML 2.0 Level 1//"
            //The public identifier starts with: "-//IETF//DTD HTML 2.0 Level 2//"
            //The public identifier starts with: "-//IETF//DTD HTML 2.0 Strict Level 1//"
            //The public identifier starts with: "-//IETF//DTD HTML 2.0 Strict Level 2//"
            //The public identifier starts with: "-//IETF//DTD HTML 2.0 Strict//"
            //The public identifier starts with: "-//IETF//DTD HTML 2.0//"
            //The public identifier starts with: "-//IETF//DTD HTML 2.1E//"
            //The public identifier starts with: "-//IETF//DTD HTML 3.0//"
            //The public identifier starts with: "-//IETF//DTD HTML 3.2 Final//"
            //The public identifier starts with: "-//IETF//DTD HTML 3.2//"
            //The public identifier starts with: "-//IETF//DTD HTML 3//"
            //The public identifier starts with: "-//IETF//DTD HTML Level 0//"
            //The public identifier starts with: "-//IETF//DTD HTML Level 1//"
            //The public identifier starts with: "-//IETF//DTD HTML Level 2//"
            //The public identifier starts with: "-//IETF//DTD HTML Level 3//"
            //The public identifier starts with: "-//IETF//DTD HTML Strict Level 0//"
            //The public identifier starts with: "-//IETF//DTD HTML Strict Level 1//"
            //The public identifier starts with: "-//IETF//DTD HTML Strict Level 2//"
            //The public identifier starts with: "-//IETF//DTD HTML Strict Level 3//"
            //The public identifier starts with: "-//IETF//DTD HTML Strict//"
            //The public identifier starts with: "-//IETF//DTD HTML//"
            //The public identifier starts with: "-//Metrius//DTD Metrius Presentational//"
            //The public identifier starts with: "-//Microsoft//DTD Internet Explorer 2.0 HTML Strict//"
            //The public identifier starts with: "-//Microsoft//DTD Internet Explorer 2.0 HTML//"
            //The public identifier starts with: "-//Microsoft//DTD Internet Explorer 2.0 Tables//"
            //The public identifier starts with: "-//Microsoft//DTD Internet Explorer 3.0 HTML Strict//"
            //The public identifier starts with: "-//Microsoft//DTD Internet Explorer 3.0 HTML//"
            //The public identifier starts with: "-//Microsoft//DTD Internet Explorer 3.0 Tables//"
            //The public identifier starts with: "-//Netscape Comm. Corp.//DTD HTML//"
            //The public identifier starts with: "-//Netscape Comm. Corp.//DTD Strict HTML//"
            //The public identifier starts with: "-//O'Reilly and Associates//DTD HTML 2.0//"
            //The public identifier starts with: "-//O'Reilly and Associates//DTD HTML Extended 1.0//"
            //The public identifier starts with: "-//O'Reilly and Associates//DTD HTML Extended Relaxed 1.0//"
            //The public identifier starts with: "-//SoftQuad Software//DTD HoTMetaL PRO 6.0::19990601::extensions to HTML 4.0//"
            //The public identifier starts with: "-//SoftQuad//DTD HoTMetaL PRO 4.0::19971010::extensions to HTML 4.0//"
            //The public identifier starts with: "-//Spyglass//DTD HTML 2.0 Extended//"
            //The public identifier starts with: "-//SQ//DTD HTML 2.0 HoTMetaL + extensions//"
            //The public identifier starts with: "-//Sun Microsystems Corp.//DTD HotJava HTML//"
            //The public identifier starts with: "-//Sun Microsystems Corp.//DTD HotJava Strict HTML//"
            //The public identifier starts with: "-//W3C//DTD HTML 3 1995-03-24//"
            //The public identifier starts with: "-//W3C//DTD HTML 3.2 Draft//"
            //The public identifier starts with: "-//W3C//DTD HTML 3.2 Final//"
            //The public identifier starts with: "-//W3C//DTD HTML 3.2//"
            //The public identifier starts with: "-//W3C//DTD HTML 3.2S Draft//"
            //The public identifier starts with: "-//W3C//DTD HTML 4.0 Frameset//"
            //The public identifier starts with: "-//W3C//DTD HTML 4.0 Transitional//"
            //The public identifier starts with: "-//W3C//DTD HTML Experimental 19960712//"
            //The public identifier starts with: "-//W3C//DTD HTML Experimental 970421//"
            //The public identifier starts with: "-//W3C//DTD W3 HTML//"
            //The public identifier starts with: "-//W3O//DTD W3 HTML 3.0//"
            //The public identifier starts with: "-//WebTechs//DTD Mozilla HTML 2.0//"
            //The public identifier starts with: "-//WebTechs//DTD Mozilla HTML//"

            List<string> startwithList = new List<string>
            {
                "+//Silmaril//dtd html Pro v0r11 19970101//",
                "-//AdvaSoft Ltd//DTD HTML 3.0 asWedit + extensions//",
                "-//AS//DTD HTML 3.0 asWedit + extensions//",
                "-//IETF//DTD HTML 2.0 Level 1//",
                "-//IETF//DTD HTML 2.0 Level 2//",
                "-//IETF//DTD HTML 2.0 Strict Level 1//",
                "-//IETF//DTD HTML 2.0 Strict Level 2//",
                "-//IETF//DTD HTML 2.0 Strict//",
                "-//IETF//DTD HTML 2.0//",
                "-//IETF//DTD HTML 2.1E//",
                "-//IETF//DTD HTML 3.0//",
                "-//IETF//DTD HTML 3.2 Final//",
                "-//IETF//DTD HTML 3.2//",
                "-//IETF//DTD HTML 3//",
                "-//IETF//DTD HTML Level 0//",
                "-//IETF//DTD HTML Level 1//",
                "-//IETF//DTD HTML Level 2//",
                "-//IETF//DTD HTML Level 3//",
                "-//IETF//DTD HTML Strict Level 0//",
                "-//IETF//DTD HTML Strict Level 1//",
                "-//IETF//DTD HTML Strict Level 2//",
                "-//IETF//DTD HTML Strict Level 3//",
                "-//IETF//DTD HTML Strict//",
                "-//IETF//DTD HTML//",
                "-//Metrius//DTD Metrius Presentational//",
                "-//Microsoft//DTD Internet Explorer 2.0 HTML Strict//",
                "-//Microsoft//DTD Internet Explorer 2.0 HTML//",
                "-//Microsoft//DTD Internet Explorer 2.0 Tables//",
                "-//Microsoft//DTD Internet Explorer 3.0 HTML Strict//",
                "-//Microsoft//DTD Internet Explorer 3.0 HTML//",
                "-//Microsoft//DTD Internet Explorer 3.0 Tables//",
                "-//Netscape Comm. Corp.//DTD HTML//",
                "-//Netscape Comm. Corp.//DTD Strict HTML//",
                "-//O'Reilly and Associates//DTD HTML 2.0//",
                "-//O'Reilly and Associates//DTD HTML Extended 1.0//",
                "-//O'Reilly and Associates//DTD HTML Extended Relaxed 1.0//",
                "-//SoftQuad Software//DTD HoTMetaL PRO 6.0::19990601::extensions to HTML 4.0//",
                "-//SoftQuad//DTD HoTMetaL PRO 4.0::19971010::extensions to HTML 4.0//",
                "-//Spyglass//DTD HTML 2.0 Extended//",
                "-//SQ//DTD HTML 2.0 HoTMetaL + extensions//",
                "-//Sun Microsystems Corp.//DTD HotJava HTML//",
                "-//Sun Microsystems Corp.//DTD HotJava Strict HTML//",
                "-//W3C//DTD HTML 3 1995-03-24//",
                "-//W3C//DTD HTML 3.2 Draft//",
                "-//W3C//DTD HTML 3.2 Final//",
                "-//W3C//DTD HTML 3.2//",
                "-//W3C//DTD HTML 3.2S Draft//",
                "-//W3C//DTD HTML 4.0 Frameset//",
                "-//W3C//DTD HTML 4.0 Transitional//",
                "-//W3C//DTD HTML Experimental 19960712//",
                "-//W3C//DTD HTML Experimental 970421//",
                "-//W3C//DTD W3 HTML//",
                "-//W3O//DTD W3 HTML 3.0//",
                "-//WebTechs//DTD Mozilla HTML 2.0//",
                "-//WebTechs//DTD Mozilla HTML//"
            };


            foreach (var item in startwithList)
            {
                if (doctype.publicId.StartsWith(item, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            //The public identifier is set to: "-//W3O//DTD W3 HTML Strict 3.0//EN//"
            //The public identifier is set to: "-/W3C/DTD HTML 4.0 Transitional/EN"
            //The public identifier is set to: "HTML"
            //The system identifier is set to: "http://www.ibm.com/data/dtd/v11/ibmxhtml1-transitional.dtd"

            if (string.Equals(doctype.publicId, "-//W3O//DTD W3 HTML Strict 3.0//EN//", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(doctype.publicId, "-/W3C/DTD HTML 4.0 Transitional/EN", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (string.Equals(doctype.publicId, "HTML", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (string.Equals(doctype.publicId, "http://www.ibm.com/data/dtd/v11/ibmxhtml1-transitional.dtd", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            //The system identifier is missing and the public identifier starts with: "-//W3C//DTD HTML 4.01 Frameset//"
            //The system identifier is missing and the public identifier starts with: "-//W3C//DTD HTML 4.01 Transitional//"

            //Anything else
            //If the document is not an iframe srcdoc document, then this is a parse error; set the Document to quirks mode.

            return true;
        }
    }
}