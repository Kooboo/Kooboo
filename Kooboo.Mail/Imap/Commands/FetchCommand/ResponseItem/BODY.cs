//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kooboo.Mail.Multipart;
using MimeKit;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    //http://www.watersprings.org/pub/id/draft-ietf-extra-imap4rev2-17.html#rfc.section.7.4.2
    //https://datatracker.ietf.org/doc/rfc9051/

    public class BODY : ICommandResponse
    {
        public virtual string Name
        {
            get
            {
                return "BODY";
            }
        }

        public List<ImapResponse> Render(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            //  BODY
            //Non - extensible form of BODYSTRUCTURE.
            if (dataItem.Section == null)
                return RenderBodyStructure(maildb, message, dataItem);
            SetSeen(maildb, message, dataItem);

            byte[] bytes = null;
            //The HEADER, HEADER.FIELDS, HEADER.FIELDS.NOT, and TEXT part
            //specifiers can be the sole part specifier or can be prefixed by
            //one or more numeric part specifiers, provided that the numeric
            //part specifier refers to a part of type MESSAGE / RFC822.The
            //MIME part specifier MUST be prefixed by one or more numeric
            //part specifiers.

            if (dataItem.Section.PartSpecifier != null)
            {
                var specifier = SpecifierParse.Parse(dataItem.Section.PartSpecifier);

                if (string.IsNullOrEmpty(specifier.Sole))
                {
                    specifier.Sole = "";
                }
                if (specifier.Numbers == null || !specifier.Numbers.Any())
                {
                    // An empty section specification refers to the entire message, including the header.

                    var headers = message.MailHeader;

                    foreach (var item in message.MailHeader)
                    {
                        if (!headers.Contains(item))
                        {
                            headers.Add(item);
                        }
                    }

                    switch (specifier.Sole.ToUpper())
                    {
                        case "MIME":
                            bytes = HeaderToBytes(headers);
                            break;
                        case "HEADER":
                            bytes = HeaderToBytes(headers);
                            break;
                        case "HEADER.FIELDS":
                            bytes = PickHeaderToBytes(headers, new HashSet<string>(dataItem.Section.Paras, StringComparer.OrdinalIgnoreCase), true);
                            break;
                        case "HEADER.FIELDS.NOT":
                            bytes = PickHeaderToBytes(headers, new HashSet<string>(dataItem.Section.Paras, StringComparer.OrdinalIgnoreCase), false);
                            break;

                        //case "TEXT":
                        default:
                            {
                                // The TEXT part specifier refers to the text body of the message,   omitting the[RFC - 2822] header 
                                var text = message.GetTextSource();
                                var body = FetchHelper.GetBodyPart(text);
                                // bytes = System.Text.Encoding.ASCII.GetBytes(body);
                                bytes = System.Text.Encoding.UTF8.GetBytes(body);
                                break;
                            }
                    }
                }
                else
                {
                    // var entity = GetEntity(message.MailMessage, specifier.Numbers);

                    var PartInfo = GetPart(message, specifier.Numbers);

                    if (PartInfo == null)
                    {
                        bytes = new byte[0];
                    }
                    else
                    {
                        switch (specifier.Sole.ToUpper())
                        {
                            case "MIME":
                                bytes = HeaderToBytes(PartInfo.HeaderList);
                                break;
                            case "HEADER":
                                bytes = HeaderToBytes(PartInfo.HeaderList);
                                break;
                            case "HEADER.FIELDS":
                                bytes = PickHeaderToBytes(PartInfo.HeaderList, new HashSet<string>(dataItem.Section.Paras, StringComparer.OrdinalIgnoreCase), true);
                                break;
                            case "HEADER.FIELDS.NOT":
                                bytes = PickHeaderToBytes(PartInfo.HeaderList, new HashSet<string>(dataItem.Section.Paras, StringComparer.OrdinalIgnoreCase), false);
                                break;

                            //case "TEXT":
                            default:
                                {
                                    //var part = entity as MimePart;
                                    //if (part != null)
                                    //{
                                    //if (part.Content != null && part.Content.Stream != null)
                                    //{
                                    // if (IsMultiplePart(message.MailMessage))
                                    if (message.PartInfo.Main.IsMultiPart)
                                    {

                                        // bytes = FetchHelper.GetPartBytes(part);
                                        var partTxt = PartInfo.GetBodyPart();
                                        // bytes = System.Text.Encoding.ASCII.GetBytes(partTxt);
                                        bytes = System.Text.Encoding.UTF8.GetBytes(partTxt);

                                    }
                                    else
                                    {
                                        // single part message. must include message header. Not sure which RFC I saw this. 
                                        // will not come here any more, since we convert every email to multipart. 

                                        //var PartText = part.GetText();

                                        //var BodyOnly = GetBodyPart(PartText); // PartText.Substring(index + 4);

                                        var BodyOnly = PartInfo.GetBodyPart();

                                        var bodyMessage = new StringBuilder();

                                        if (dataItem.SinglePartBodyHeader)
                                        {
                                            bool EndWithLine = false;
                                            foreach (var header in message.MailHeader)
                                            {
                                                var value = System.Text.Encoding.UTF8.GetString(header.RawValue);
                                                bodyMessage.Append(header.Field + ":" + value);
                                                EndWithLine = value.EndsWith("\n");
                                            }

                                            if (PartInfo.HeaderList != null && PartInfo.HeaderList.Any())
                                            {
                                                foreach (var header in PartInfo.HeaderList)
                                                {
                                                    var value = System.Text.Encoding.UTF8.GetString(header.RawValue);
                                                    bodyMessage.Append(header.Field + ":" + value);
                                                    EndWithLine = value.EndsWith("\n");
                                                }
                                            }

                                            if (EndWithLine)
                                            {
                                                bodyMessage.Append("\r\n");
                                            }
                                            else
                                            {
                                                bodyMessage.Append("\r\n\r\n");
                                            }
                                        }

                                        bodyMessage.Append(BodyOnly);

                                        bytes = System.Text.Encoding.UTF8.GetBytes(bodyMessage.ToString());
                                        // bytes = System.Text.Encoding.ASCII.GetBytes(bodyMessage.ToString());

                                    }

                                    //}
                                    //else
                                    //{
                                    //    bytes = new byte[0];
                                    //}

                                    //}
                                    break;
                                }
                        }
                    }

                }
            }
            else
            {
                var text = message.GetTextSource();
                // bytes = System.Text.Encoding.UTF8.GetBytes(message.MailMessage.ToMessageText());
                bytes = System.Text.Encoding.UTF8.GetBytes(text);
                // bytes = System.Text.Encoding.ASCII.GetBytes(text);
            }

            if (bytes == null)
            {
                var text = message.GetTextSource();
                bytes = System.Text.Encoding.UTF8.GetBytes(text);
                //bytes = System.Text.Encoding.ASCII.GetBytes(text);
                // bytes = System.Text.Encoding.UTF8.GetBytes(message.MailMessage.ToMessageText());
            }

            var result = new List<ImapResponse>();
            var builder = new StringBuilder()
                .Append("BODY").Append("[");

            if (dataItem.Section != null)
            {
                builder.Append(dataItem.Section.PartSpecifier);
                if (dataItem.Section.Paras != null && dataItem.Section.Paras.Count > 0)
                {
                    builder.Append(" (").Append(String.Join(" ", dataItem.Section.Paras.Select(o => o.ToUpperInvariant()))).Append(")");
                }
                builder.Append("]");
            }

            if (dataItem.Partial != null)
            {
                if (dataItem.Partial.OffSet >= bytes.Length)
                {
                    builder.Append("<").Append(dataItem.Partial.OffSet).Append(">").Append(" \"\"");
                    result.Add(new ImapResponse(builder.ToString()));
                }
                else
                {
                    var count = Math.Min(dataItem.Partial.Count, bytes.Length - dataItem.Partial.OffSet);

                    builder.Append("<").Append(dataItem.Partial.OffSet).Append(">")
                        .Append(" {").Append(count).Append("}");

                    result.Add(new ImapResponse(builder.ToString()));
                    result.Add(new ImapResponse(bytes.Skip(dataItem.Partial.OffSet).Take(count).ToArray()));
                }
            }
            else
            {
                builder.Append(" {").Append(bytes.Length).Append("}");
                result.Add(new ImapResponse(builder.ToString()));
                result.Add(new ImapResponse(bytes));
            }

            return result;
        }


        public string GetBodyPart(string FullMsg)
        {
            var index = FullMsg.IndexOf("\r\n\r\n");

            var linuxIndex = FullMsg.IndexOf("\n\n");

            if (index == -1 || (index > linuxIndex && linuxIndex > -1))
            {
                return FullMsg.Substring(linuxIndex + 2);
            }
            else
            {
                if (index > -1)
                {
                    return FullMsg.Substring(index + 4);
                }
            }
            return FullMsg;

        }

        public bool IsMultiplePart(MimeMessage msg)
        {
            if (msg == null)
            {
                return false;
            }

            if (msg.BodyParts.Count() > 1)
            {
                return true;
            }

            return msg.Body is MimeKit.Multipart;

        }

        public string GetPartText(MimePart part, MimeMessage message)
        {
            string body = null;
            if (part is TextPart)
            {
                var textPart = part as TextPart;
                body = textPart.Text;

            }

            if (body == null)
            {
                if (part.Content != null && part.Content.Stream != null)
                {
                    MemoryStream ms = new MemoryStream();
                    part.Content.Stream.CopyTo(ms);
                    body = System.Text.Encoding.ASCII.GetString(ms.ToArray());
                }
            }

            if (body == null)
            {
                return null;
            }

            if (message.BodyParts.Count() > 1)
            {
                // multiple, return directly. 
                return body;
            }
            else
            {
                var builder = new StringBuilder();
                bool EndWithLine = false;

                // builder.Append("Content-Type: text/text; charset=utf8\r\n");

                foreach (var header in message.Headers)
                {
                    var value = System.Text.Encoding.ASCII.GetString(header.RawValue);
                    builder.Append(header.Field + ":" + value);
                    EndWithLine = value.EndsWith("\r\n");
                }

                if (EndWithLine)
                {
                    builder.Append("\r\n");
                }
                else
                {
                    builder.Append("\r\n\r\n");
                }

                builder.Append(body);
                return builder.ToString();
            }

        }

        public List<ImapResponse> RenderBodyStructure(MailDb mailDb, FetchMessage message, DataItem dataItem)
        {
            var builder = new StructureBuilder()
                .Append("BODYSTRUCTURE ");

            // BODYSTRUCTURE.ConstructParts(builder, message.MailMessage.Body, message, false);
            BODYSTRUCTURE.ConstructParts(builder, message.PartInfo.Main, message, false);

            return new List<ImapResponse>
            {
                new ImapResponse(builder.ToString())
            };
        }

        protected virtual void SetSeen(MailDb mailDb, FetchMessage message, DataItem dataItem)
        {
            if (!message.Message.Read)
            {
                message.Message.Read = true;
                mailDb.Message2.UpdateMeta(message.Message);
            }
        }

        public byte[] PickHeaderToBytes(HeaderList Headers, HashSet<string> fieldNames, bool Include)
        {
            // all header fieldname will be transfer to upper case during parser.

            if (Include)
            {
                var builder = new StringBuilder();
                foreach (var header in Headers)
                {
                    if (fieldNames.Contains(header.Field.ToUpper()))
                    {
                        var value = System.Text.Encoding.UTF8.GetString(header.RawValue);
                        builder.Append(header.Field + ":" + value);
                        if (!value.EndsWith("\r\n"))
                        {
                            builder.Append("\r\n");
                        }
                    }
                }
                return Encoding.UTF8.GetBytes(builder.ToString());
            }
            else
            {
                var builder = new StringBuilder();
                foreach (var header in Headers)
                {
                    if (!fieldNames.Contains(header.Field.ToUpper()))
                    {
                        var value = System.Text.Encoding.UTF8.GetString(header.RawValue);
                        builder.Append(header.Field + ":" + value);
                        if (!value.EndsWith("\r\n"))
                        {
                            builder.Append("\r\n");
                        }
                    }
                }
                return Encoding.UTF8.GetBytes(builder.ToString());
            }
        }

        public byte[] HeaderToBytes(HeaderList Headers)
        {
            var builder = new StringBuilder();
            foreach (var header in Headers)
            {
                var value = System.Text.Encoding.UTF8.GetString(header.RawValue);
                builder.Append(header.Field + ":" + value);
                if (!value.EndsWith("\r\n"))
                {
                    builder.Append("\r\n");
                }
            }

            builder.Append("\r\n");

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public static MimeEntity GetEntity(MimeMessage msg, int[] numbers)
        {
            if (numbers == null || !numbers.Any())
            {
                return msg.Body;
            }

            if (msg.Body is MimeKit.Multipart)
            {
                var part = msg.Body;

                for (int i = 0; i < numbers.Length; i++)
                {
                    var entity = part as MimeKit.Multipart;

                    if (entity == null)
                    {
                        return null;
                    }

                    var num = numbers[i] - 1;
                    if (num < 0 || num >= entity.ToList().Count())
                        return null;

                    part = entity.ToList()[num];
                }

                return part;
            }
            else
            {
                if (numbers[0] == 1)
                {
                    return msg.Body;
                }
            }

            return null;



        }

        public static PartInfo GetPart(FetchMessage msg, int[] numbers)
        {
            if (numbers == null || !numbers.Any())
            {
                return msg.PartInfo.Main;
            }

            if (msg.PartInfo.Main.IsMultiPart)
            {
                var part = msg.PartInfo.Main;

                for (int i = 0; i < numbers.Length; i++)
                {
                    if (!part.IsMultiPart)
                    {
                        return null;
                    }
                    var num = numbers[i] - 1;
                    if (num < 0 || num >= part.Parts.Count())
                        return null;

                    part = part.Parts[num];
                }

                return part;
            }
            else
            {
                if (numbers[0] == 1)
                {
                    return msg.PartInfo.Main;
                }
            }

            return null;

        }




        public class SpecifierParse
        {
            public int[] Numbers { get; set; }

            public string Sole { get; set; }

            public static SpecifierParse Parse(string str)
            {
                var specifiers = str.Split('.');

                var numbers = new List<int>();
                int i = 0;
                for (; i < specifiers.Length; i++)
                {
                    int num;
                    if (!Int32.TryParse(specifiers[i], out num))
                        break;

                    numbers.Add(num);
                }

                return new SpecifierParse
                {
                    Numbers = i == 0 ? null : numbers.ToArray(),
                    Sole = i >= specifiers.Length ? null : String.Join(".", specifiers, i, specifiers.Length - i)
                };
            }
        }
    }
}


/*
 * BODY[<section>]<<partial>>
 The text of a particular body section. The section
 specification is a set of zero or more part specifiers
 delimited by periods. A part specifier is either a part number
 or one of the following: HEADER, HEADER.FIELDS,
 HEADER.FIELDS.NOT, MIME, and TEXT. An empty section
 specification refers to the entire message, including the
 header.
 Every message has at least one part number. Non-[MIME-IMB]
 messages, and non-multipart [MIME-IMB] messages with no
 encapsulated message, only have a part 1.
 Multipart messages are assigned consecutive part numbers, as
 they occur in the message. If a particular part is of type
 message or multipart, its parts MUST be indicated by a period
 followed by the part number within that nested multipart part.
 A part of type MESSAGE/RFC822 also has nested part numbers,
 referring to parts of the MESSAGE part��s body.
 The HEADER, HEADER.FIELDS, HEADER.FIELDS.NOT, and TEXT part
 specifiers can be the sole part specifier or can be prefixed by
 one or more numeric part specifiers, provided that the numeric
 part specifier refers to a part of type MESSAGE/RFC822. The
 MIME part specifier MUST be prefixed by one or more numeric
 part specifiers.
 The HEADER, HEADER.FIELDS, and HEADER.FIELDS.NOT part
 specifiers refer to the [RFC-2822] header of the message or of
 an encapsulated [MIME-IMT] MESSAGE/RFC822 message.
 HEADER.FIELDS and HEADER.FIELDS.NOT are followed by a list of
 field-name (as defined in [RFC-2822]) names, and return a
Crispin Standards Track [Page 55]
RFC 3501 IMAPv4 March 2003
 subset of the header. The subset returned by HEADER.FIELDS
 contains only those header fields with a field-name that
 matches one of the names in the list; similarly, the subset
 returned by HEADER.FIELDS.NOT contains only the header fields
 with a non-matching field-name. The field-matching is
 case-insensitive but otherwise exact. Subsetting does not
 exclude the [RFC-2822] delimiting blank line between the header
 and the body; the blank line is included in all header fetches,
 except in the case of a message which has no body and no blank
 line.
 The MIME part specifier refers to the [MIME-IMB] header for
 this part.
 The TEXT part specifier refers to the text body of the message,
 omitting the [RFC-2822] header.
 Here is an example of a complex message with some of its
 part specifiers:
 HEADER ([RFC-2822] header of the message)
 TEXT ([RFC-2822] text body of the message) MULTIPART/MIXED
 1 TEXT/PLAIN
 2 APPLICATION/OCTET-STREAM
 3 MESSAGE/RFC822
 3.HEADER ([RFC-2822] header of the message)
 3.TEXT ([RFC-2822] text body of the message) MULTIPART/MIXED
 3.1 TEXT/PLAIN
 3.2 APPLICATION/OCTET-STREAM
 4 MULTIPART/MIXED
 4.1 IMAGE/GIF
 4.1.MIME ([MIME-IMB] header for the IMAGE/GIF)
 4.2 MESSAGE/RFC822
 4.2.HEADER ([RFC-2822] header of the message)
 4.2.TEXT ([RFC-2822] text body of the message) MULTIPART/MIXED
 4.2.1 TEXT/PLAIN
 4.2.2 MULTIPART/ALTERNATIVE
 4.2.2.1 TEXT/PLAIN
 4.2.2.2 TEXT/RICHTEXT
 It is possible to fetch a substring of the designated text.
 This is done by appending an open angle bracket ("<"), the
 octet position of the first desired octet, a period, the
 maximum number of octets desired, and a close angle bracket
 (">") to the part specifier. If the starting octet is beyond
 the end of the text, an empty string is returned.
Crispin Standards Track [Page 56]
RFC 3501 IMAPv4 March 2003
 Any partial fetch that attempts to read beyond the end of the
 text is truncated as appropriate. A partial fetch that starts
 at octet 0 is returned as a partial fetch, even if this
 truncation happened.
 Note: This means that BODY[]<0.2048> of a 1500-octet message
 will return BODY[]<0> with a literal of size 1500, not
 BODY[].
 Note: A substring fetch of a HEADER.FIELDS or
 HEADER.FIELDS.NOT part specifier is calculated after
 subsetting the header.
 The \Seen flag is implicitly set; if this causes the flags to
 change, they SHOULD be included as part of the FETCH responses
 * 
 * 
 * 
 * 
 * 
 */