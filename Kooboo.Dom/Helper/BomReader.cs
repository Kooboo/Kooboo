//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom
{

    /// <summary>
    /// TO be implemented, we assume that this is done by the .NET string or webclient or httpclient now. 
    /// </summary>
    public class BomReader
    {

        //3.2. The input byte stream

        //When parsing a stylesheet, the stream of Unicode code points that comprises the input to the tokenization stage might be initially seen by the user agent as a stream of bytes (typically coming over the network or from the local file system). If so, the user agent must decode these bytes into code points according to a particular character encoding.

        //To decode the stream of bytes into a stream of code points, UAs must use the decode algorithm defined in [ENCODING], with the fallback encoding determined as follows.

        //Note: The decode algorithm gives precedence to a byte order mark (BOM), and only uses the fallback when none is found.

        //To determine the fallback encoding:

        //If HTTP or equivalent protocol defines an encoding (e.g. via the charset parameter of the Content-Type header), get an encoding [ENCODING] for the specified value. If that does not return failure, use the return value as the fallback encoding.
        //Otherwise, check the byte stream. If the first 1024 bytes of the stream begin with the hex sequence
        //40 63 68 61 72 73 65 74 20 22 XX* 22 3B
        //where each XX byte is a value between 016 and 2116 inclusive or a value between 2316 and 7F16 inclusive, then get an encoding for the sequence of XX bytes, interpreted as ASCII.

        //What does that byte sequence mean?
        //If the return value was utf-16be or utf-16le, use utf-8 as the fallback encoding; if it was anything else except failure, use the return value as the fallback encoding.

        //Why use utf-8 when the declaration says utf-16?
        //Note: Note that the syntax of an encoding declaration looks like the syntax of an @charset rule, but it’s actually much more restrictive. A number of things you can do in CSS that would produce a valid @charset rule, such as using multiple spaces, comments, or single quotes, will cause the encoding declaration to not be recognized. This behavior keeps the encoding declaration as simple as possible, and thus maximizes the likelihood of it being implemented correctly.

        //Otherwise, if an environment encoding is provided by the referring document, use that as the fallback encoding.
        //Otherwise, use utf-8 as the fallback encoding.
        //Though UTF-8 is the default encoding for the web, and many newer web-based file formats assume or require UTF-8 encoding, CSS was created before it was clear which encoding would win, and thus can’t automatically assume the stylesheet is UTF-8.

        //Stylesheet authors should author their stylesheets in UTF-8, and ensure that either an HTTP header (or equivalent method) declares the encoding of the stylesheet to be UTF-8, or that the referring document declares its encoding to be UTF-8. (In HTML, this is done by adding a <meta charset=utf-8> element to the head of the document.)

        //If neither of these options are available, authors should begin the stylesheet with a UTF-8 BOM or the exact characters

        //@charset "utf-8";
        //Document languages that refer to CSS stylesheets that are decoded from bytes may define an environment encoding for each such stylesheet, which is used as a fallback when other encoding hints are not available or can not be used.

        //The concept of environment encoding only exists for compatibility with legacy content. New formats and new linking mechanisms should not provide an environment encoding, so the stylesheet defaults to UTF-8 instead in the absence of more explicit information.

        //Note: [HTML] defines the environment encoding for <link rel=stylesheet>.

        //Note: [CSSOM] defines the environment encoding for <xml-stylesheet?>.

        //Note: [CSS3CASCADE] defines the environment encoding for @import.


    }
}
