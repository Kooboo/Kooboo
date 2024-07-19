//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.Dom
{
    //8.1.2.4 Optional tags
    //Certain tags can be omitted.
    //Omitting an element's start tag in the situations described below does not mean the element is not present; it is implied, but it is still there. For example, an HTML document always has a root html element, even if the string <html> doesn't appear anywhere in the markup.

    //An html element's start tag may be omitted if the first thing inside the html element is not a comment.

    //An html element's end tag may be omitted if the html element is not immediately followed by a comment.

    //A head element's start tag may be omitted if the element is empty, or if the first thing inside the head element is an element.

    //A head element's end tag may be omitted if the head element is not immediately followed by a space character or a comment.

    //A body element's start tag may be omitted if the element is empty, or if the first thing inside the body element is not a space character or a comment, except if the first thing inside the body element is a meta, link, script, style, or template element.
    //A body element's end tag may be omitted if the body element is not immediately followed by a comment.

    //An li element's end tag may be omitted if the li element is immediately followed by another li element or if there is no more content in the parent element.

    //A dt element's end tag may be omitted if the dt element is immediately followed by another dt element or a dd element.

    //A dd element's end tag may be omitted if the dd element is immediately followed by another dd element or a dt element, or if there is no more content in the parent element.

    //A p element's end tag may be omitted if the p element is immediately followed by an address, article, aside, blockquote, div, dl, fieldset, footer, form, h1, h2, h3, h4, h5, h6, header, hgroup, hr, main, nav, ol, p, pre, section, table, or ul, element, or if there is no more content in the parent element and the parent element is not an a element.

    //An rb element's end tag may be omitted if the rb element is immediately followed by an rb, rt, rtc or rp element, or if there is no more content in the parent element.

    //An rt element's end tag may be omitted if the rt element is immediately followed by an rb, rt, rtc, or rp element, or if there is no more content in the parent element.

    //An rtc element's end tag may be omitted if the rtc element is immediately followed by an rb, rtc or rp element, or if there is no more content in the parent element.

    //An rp element's end tag may be omitted if the rp element is immediately followed by an rb, rt, rtc or rp element, or if there is no more content in the parent element.

    //An optgroup element's end tag may be omitted if the optgroup element is immediately followed by another optgroup element, or if there is no more content in the parent element.

    //An option element's end tag may be omitted if the option element is immediately followed by another option element, or if it is immediately followed by an optgroup element, or if there is no more content in the parent element.

    //A colgroup element's start tag may be omitted if the first thing inside the colgroup element is a col element, and if the element is not immediately preceded by another colgroup element whose end tag has been omitted. (It can't be omitted if the element is empty.)

    //A colgroup element's end tag may be omitted if the colgroup element is not immediately followed by a space character or a comment.

    //A thead element's end tag may be omitted if the thead element is immediately followed by a tbody or tfoot element.

    //A tbody element's start tag may be omitted if the first thing inside the tbody element is a tr element, and if the element is not immediately preceded by a tbody, thead, or tfoot element whose end tag has been omitted. (It can't be omitted if the element is empty.)

    //A tbody element's end tag may be omitted if the tbody element is immediately followed by a tbody or tfoot element, or if there is no more content in the parent element.

    //A tfoot element's end tag may be omitted if the tfoot element is immediately followed by a tbody element, or if there is no more content in the parent element.

    //A tr element's end tag may be omitted if the tr element is immediately followed by another tr element, or if there is no more content in the parent element.

    //A td element's end tag may be omitted if the td element is immediately followed by a td or th element, or if there is no more content in the parent element.

    //A th element's end tag may be omitted if the th element is immediately followed by a td or th element, or if there is no more content in the parent element.

    //However, a start tag must never be omitted if it has any attributes.

    //8.1.2.5 Restrictions on content models

    //For historical reasons, certain elements have extra restrictions beyond even the restrictions given by their content model.

    //A table element must not contain tr elements, even though these elements are technically allowed inside table elements according to the content models described in this specification. (If a tr element is put inside a table in the markup, it will in fact imply a tbody start tag before it.)

    //A single newline may be placed immediately after the start tag of pre and textarea elements. If the element's contents are intended to start with a newline, two consecutive newlines thus need to be included by the author.

    //The following two pre blocks are equivalent:

    //<pre>Hello</pre>
    //<pre>
    //Hello</pre>


    class HtmlOptionalTags
    {
    }
}
