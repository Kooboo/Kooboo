//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Mail.Multipart;
using MimeKit;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class BODYSTRUCTURE : ICommandResponse
    {
        public virtual string Name
        {
            get
            {
                return "BODYSTRUCTURE";
            }
        }

        public List<ImapResponse> Render(MailDb mailDb, FetchMessage message, DataItem dataItem)
        {
            var builder = new StructureBuilder()
                 .Append(dataItem.FullItemName)
                 .Append(" ");

            ConstructParts(builder, message.PartInfo.Main, message, true);

            return new List<ImapResponse>
            {
                new ImapResponse(builder.ToString())
            };
        }

        public static void ConstructParts(StructureBuilder builder, PartInfo partInfo, FetchMessage message, bool includeExtensions)
        {
            //if (entity is MimeKit.Multipart)
            if (partInfo.IsMultiPart)
            {
                // Recursive build for multipart
                builder.StartBracket();

                // var multipart = entity as MimeKit.Multipart;

                //foreach (var child in multipart)
                //{
                //    ConstructParts(builder, child, message, includeExtensions);
                //} 
                foreach (var child in partInfo.Parts)
                {
                    ConstructParts(builder, child, message, includeExtensions);
                }

                var headerReader = new HeaderListReader(partInfo.HeaderList);

                if (headerReader.ContentType != null && headerReader.ContentType.MediaSubtype != null)
                {
                    builder.SpaceNQuoted(headerReader.ContentType.MediaSubtype.ToUpperInvariant());
                }
                else
                {
                    builder.AppendQuoted("PLAIN");
                }

                //Extension data follows the multipart subtype. Extension data is never returned with the BODY fetch, but can be returned with a BODYSTRUCTURE fetch. Extension data, if present, MUST be in the defined order. The extension data of a multipart body part are in the following order:
                //body parameter parenthesized list
                //A parenthesized list of attribute/value pairs [e.g., ("foo" "bar" "baz" "rag") where "bar" is the value of "foo", and "rag" is the value of "baz"] as defined in [MIME-IMB]. Servers SHOULD decode parameter value continuations and parameter value character sets as described in [RFC2231], for example, if the message contains parameters "baz*0", "baz*1" and "baz*2", the server should RFC2231-decode them, concatenate and return the resulting value as a parameter "baz". Similarly, if the message contains parameters "foo*0*" and "foo*1*", the server should RFC2231-decode them, convert to UTF-8, concatenate and return the resulting value as a parameter "foo*".
                //body disposition
                //A parenthesized list, consisting of a disposition type string, followed by a parenthesized list of disposition attribute/value pairs as defined in [DISPOSITION]. Servers SHOULD decode parameter value continuations as described in [RFC2231].
                //body language
                //A string or parenthesized list giving the body language value as defined in [LANGUAGE-TAGS].
                //body location
                //A string giving the body content URI as defined in [LOCATION].

                if (includeExtensions)
                {
                    // conentTypeParameters - Syntax: {("name" SP "value" *(SP "name" SP "value"))}
                    ConstructTypeParameters(builder, partInfo);

                    // body disposition  Syntax: {(disposition-type [ SP ("name" SP "value" *(SP "name" SP "value"))])}
                    ConstructBodyDisposition(builder, partInfo);

                    // body language
                    builder.AppendNil();

                    // body location
                    builder.AppendNil();
                }

                builder.Append(")");
            }
            else
            {
                ConstructSinglePartMsg(builder, partInfo, message, includeExtensions);
            }
        }

        public static void ConstructTypeParameters(StructureBuilder builder, PartInfo partInfo)
        {
            var headerReader = new HeaderListReader(partInfo.HeaderList);

            if (headerReader.ContentType != null)
            {
                if (headerReader.ContentType.Parameters.Count > 0)
                {
                    builder.SpaceNBracket();
                    bool first = true;
                    foreach (var parameter in headerReader.ContentType.Parameters)
                    {
                        if (String.IsNullOrEmpty(parameter.Name))
                            continue;
                        // For the first item, don't add SP.
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            builder.Append(" ");
                        }

                        builder.AppendQuoted(parameter.Name.ToUpperInvariant()).SpaceNQuoted(Utility.HeaderUtility.EncodeFieldB(parameter.Value));
                    }
                    builder.EndBracket();
                }
                else
                {
                    builder.AppendNil();
                }
            }
            else
            {
                builder.AppendNil();
            }
        }

        public static void ConstructBodyDisposition(StructureBuilder builder, PartInfo partInfo)
        {

            var headerReader = new HeaderListReader(partInfo.HeaderList);

            if (headerReader.ContentDisposition != null && headerReader.ContentDisposition.Parameters.Count > 0)
            {
                builder.SpaceNBracket().AppendQuoted(headerReader.ContentDisposition.Disposition.ToUpperInvariant());

                if (headerReader.ContentDisposition.Parameters.Count > 0)
                {
                    builder.SpaceNBracket();

                    bool first = true;
                    foreach (var parameter in headerReader.ContentDisposition.Parameters)
                    {
                        if (String.IsNullOrEmpty(parameter.Name))
                            continue;

                        // For the first item, don't add SP.
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            builder.Append(" ");
                        }

                        builder.AppendQuoted(parameter.Name.ToUpperInvariant()).SpaceNQuoted(Utility.HeaderUtility.EncodeFieldB(parameter.Value));
                    }
                    builder.EndBracket();
                }
                else
                {
                    builder.AppendNil();
                }

                builder.EndBracket();
            }
            else
            {
                builder.AppendNil();
            }
        }

        public static void ConstructSinglePartMsg(StructureBuilder builder, PartInfo partInfo, FetchMessage message, bool includeExtensions)
        {

            var HeaderReader = new HeaderListReader(partInfo.HeaderList);

            builder.Append("(");

            // NOTE: all header fields and parameters must in ENCODED form !!!

            // Add contentTypeMainMediaType 
            if (HeaderReader.ContentType != null && HeaderReader.ContentType.MediaType != null)
            {
                builder.AppendQuoted(HeaderReader.ContentType.MediaType.ToUpperInvariant());
            }
            else
            {
                builder.AppendQuoted("TEXT");
            }

            // Add contentTypeSubMediaType
            if (HeaderReader.ContentType != null && HeaderReader.ContentType.MediaSubtype != null)
            {
                builder.SpaceNQuoted(HeaderReader.ContentType.MediaSubtype.ToUpperInvariant());
            }
            else
            {
                builder.SpaceNQuoted("PLAIN");
            }

            // conentTypeParameters - Syntax: {("name" SP "value" *(SP "name" SP "value"))}
            ConstructTypeParameters(builder, partInfo);

            // contentID 
            if (HeaderReader.ContentId != null)
            {
                builder.SpaceNQuoted(HeaderReader.ContentId);
            }
            else
            {
                builder.AppendNil();
            }

            // contentDescription
            //string contentDescription = entity;
            //if (contentDescription != null)
            //{
            //    builder.SpaceNQuoted(wordEncoder.Encode(contentDescription));
            //}
            //else
            //{
            builder.AppendNil();
            //}

            // contentEncoding
            //if (mimepart != null)
            //{
            //    var encodingText = "7BIT"; // default. 
            //    if (mimepart.ContentTransferEncoding == ContentEncoding.Base64)
            //    {
            //        encodingText = "BASE64";
            //    }
            //    else if (mimepart.ContentTransferEncoding == ContentEncoding.QuotedPrintable)
            //    {
            //        encodingText = "QUOTED-PRINTABLE";
            //    }
            //    else if (mimepart.ContentTransferEncoding == ContentEncoding.EightBit)
            //    {
            //        encodingText = "8BIT";
            //    }

            //    builder.SpaceNQuoted(encodingText);
            //}
            //else
            //{
            //    // If not specified, then must be 7bit.
            //    builder.SpaceNQuoted("7BIT");
            //}

            var encodingText = "7BIT"; // default. 
            if (HeaderReader.ContentTransferEncoding == ContentEncoding.Base64)
            {
                encodingText = "BASE64";
            }
            else if (HeaderReader.ContentTransferEncoding == ContentEncoding.QuotedPrintable)
            {
                encodingText = "QUOTED-PRINTABLE";
            }
            else if (HeaderReader.ContentTransferEncoding == ContentEncoding.EightBit)
            {
                encodingText = "8BIT";
            }

            builder.SpaceNQuoted(encodingText);


            // contentSize
            //if (entity.Body is MIME_b_SinglepartBase)
            //{
            // builder.Append(" ").Append(((MIME_b_SinglepartBase)entity.Body).EncodedData.Length.ToString());
            //}
            //else
            //{
            //    builder.Append(" 0");
            //}
            // TODO: to verify , this must be a singlepart message to reach here...

            //byte[] contentBytes = null;

            //if (mimepart != null)
            //{
            //    contentBytes = FetchHelper.GetPartBytes(mimepart);
            //    //long length = 0; 
            //    //if (mimepart.Content !=null && mimepart.Content.Stream !=null)
            //    //{
            //    //    length = mimepart.Content.Stream.Length;
            //    //}

            //    builder.Append(" ").Append(contentBytes.Length.ToString());
            //}
            //else
            //{
            //    builder.Append(" 0");
            //}

            //          A body type of type MESSAGE and subtype RFC822 contains,
            //           immediately after the basic fields, the envelope structure,
            //body structure, and size in text lines of the encapsulated
            //message.
            //A body type of type TEXT contains, immediately after the basic
            //fields, the size of the body in text lines. Note that this
            //size is the size in its content transfer encoding and not the
            //resulting size after any decoding.

            // envelope --->FOR ContentType: message / rfc822 ONLY ###
            //if (entity.Body is MIME_b_MessageRfc822)
            //{
            //    builder.Append(" "); 
            //     ENVELOPE.ConstructEnvelope(builder, ((MIME_b_MessageRfc822)entity.Body).Message);

            //    // BODYSTRUCTURE
            //    builder.AppendNil();

            //    // LINES
            //    builder.AppendNil();
            //}

            //// contentLines ---> FOR ContentType: text/xxx ONLY ###
            //if (entity.Body is MIME_b_Text)
            //{ 
            //    builder.Append(" ").Append(GetLines(entity).ToString());
            //} 

            //if (textpart != null)
            //{
            //    if (textpart.Content != null && textpart.Content.Stream != null)
            //    {
            //        var lineNumber = FetchHelper.GetLineNumber(contentBytes);
            //        builder.Append(" ").Append(lineNumber.ToString());
            //    }
            //    else
            //    {
            //        builder.Append(" ").Append("0");
            //    }

            //}


            var bodyText = partInfo.GetBodyPart();

            var contentBytes = System.Text.Encoding.UTF8.GetBytes(bodyText);

            int len = contentBytes != null ? contentBytes.Length : 0;

            builder.Append(" ").Append(len.ToString());
            //}
            //else
            //{
            //    builder.Append(" 0");

            var lineNumber = FetchHelper.GetLineNumber(bodyText);
            builder.Append(" ").Append(lineNumber.ToString());

            //  The extension data of a non-multipart body part are in the following order:
            //   body MD5
            //A string giving the body MD5 value as defined in [MD5].
            //body disposition
            //A parenthesized list with the same content and function as the body disposition for a multipart body part.
            //body language
            //A string or parenthesized list giving the body language value as defined in [LANGUAGE-TAGS].
            //body location
            //A string giving the body content URI as defined in [LOCATION].

            if (includeExtensions)
            {
                // body MD5
                builder.AppendNil();

                // body disposition  Syntax: {(disposition-type [ SP ("name" SP "value" *(SP "name" SP "value"))])}
                ConstructBodyDisposition(builder, partInfo);

                // body language
                builder.AppendNil();

                // body location
                builder.AppendNil();
            }


            builder.EndBracket();
        }

    }
}


/*
 * RFC 3501
 * 
 *  BODYSTRUCTURE
 A parenthesized list that describes the [MIME-IMB] body
 structure of a message. This is computed by the server by
 parsing the [MIME-IMB] header fields, defaulting various fields
 as necessary.
 For example, a simple text message of 48 lines and 2279 octets
 can have a body structure of: ("TEXT" "PLAIN" ("CHARSET"
 "US-ASCII") NIL NIL "7BIT" 2279 48)
 Multiple parts are indicated by parenthesis nesting. Instead
 of a body type as the first element of the parenthesized list,
 there is a sequence of one or more nested body structures. The
 second element of the parenthesized list is the multipart
 subtype (mixed, digest, parallel, alternative, etc.).
Crispin Standards Track [Page 74]
RFC 3501 IMAPv4 March 2003
 For example, a two part message consisting of a text and a
 BASE64-encoded text attachment can have a body structure of:
 (("TEXT" "PLAIN" ("CHARSET" "US-ASCII") NIL NIL "7BIT" 1152
 23)("TEXT" "PLAIN" ("CHARSET" "US-ASCII" "NAME" "cc.diff")
 "<960723163407.20117h@cac.washington.edu>" "Compiler diff"
 "BASE64" 4554 73) "MIXED")
 Extension data follows the multipart subtype. Extension data
 is never returned with the BODY fetch, but can be returned with
 a BODYSTRUCTURE fetch. Extension data, if present, MUST be in
 the defined order. The extension data of a multipart body part
 are in the following order:
 body parameter parenthesized list
 A parenthesized list of attribute/value pairs [e.g., ("foo"
 "bar" "baz" "rag") where "bar" is the value of "foo", and
 "rag" is the value of "baz"] as defined in [MIME-IMB].
 body disposition
 A parenthesized list, consisting of a disposition type
 string, followed by a parenthesized list of disposition
 attribute/value pairs as defined in [DISPOSITION].
 body language
 A string or parenthesized list giving the body language
 value as defined in [LANGUAGE-TAGS].
 body location
 A string list giving the body content URI as defined in
 [LOCATION].
 Any following extension data are not yet defined in this
 version of the protocol. Such extension data can consist of
 zero or more NILs, strings, numbers, or potentially nested
 parenthesized lists of such data. Client implementations that
 do a BODYSTRUCTURE fetch MUST be prepared to accept such
 extension data. Server implementations MUST NOT send such
 extension data until it has been defined by a revision of this
 protocol.
 The basic fields of a non-multipart body part are in the
 following order:
 body type
 A string giving the content media type name as defined in
 [MIME-IMB].
Crispin Standards Track [Page 75]
RFC 3501 IMAPv4 March 2003
 body subtype
 A string giving the content subtype name as defined in
 [MIME-IMB].
 body parameter parenthesized list
 A parenthesized list of attribute/value pairs [e.g., ("foo"
 "bar" "baz" "rag") where "bar" is the value of "foo" and
 "rag" is the value of "baz"] as defined in [MIME-IMB].
 body id
 A string giving the content id as defined in [MIME-IMB].
 body description
 A string giving the content description as defined in
 [MIME-IMB].
 body encoding
 A string giving the content transfer encoding as defined in
 [MIME-IMB].
 body size
 A number giving the size of the body in octets. Note that
 this size is the size in its transfer encoding and not the
 resulting size after any decoding.
 A body type of type MESSAGE and subtype RFC822 contains,
 immediately after the basic fields, the envelope structure,
 body structure, and size in text lines of the encapsulated
 message.
 A body type of type TEXT contains, immediately after the basic
 fields, the size of the body in text lines. Note that this
 size is the size in its content transfer encoding and not the
 resulting size after any decoding.
 Extension data follows the basic fields and the type-specific
 fields listed above. Extension data is never returned with the
 BODY fetch, but can be returned with a BODYSTRUCTURE fetch.
 Extension data, if present, MUST be in the defined order.
 The extension data of a non-multipart body part are in the
 following order:
 body MD5
 A string giving the body MD5 value as defined in [MD5].
Crispin Standards Track [Page 76]
RFC 3501 IMAPv4 March 2003
 body disposition
 A parenthesized list with the same content and function as
 the body disposition for a multipart body part.
 body language
 A string or parenthesized list giving the body language
 value as defined in [LANGUAGE-TAGS].
 body location
 A string list giving the body content URI as defined in
 [LOCATION].
 Any following extension data are not yet defined in this
 version of the protocol, and would be as described above under
 multipart extension data.
*/