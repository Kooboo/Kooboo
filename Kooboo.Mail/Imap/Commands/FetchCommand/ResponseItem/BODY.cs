//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;
using LumiSoft.Net.MIME;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
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
            // Todo: what happen when BODY.PEEK
            if (dataItem.Section == null)
                return RenderBodyStructure(maildb, message, dataItem);

            BeforeRender(maildb, message, dataItem);

            var bytes = message.Bytes;
            if (dataItem.Section.PartSpecifier != null)
            {
                var parsed = LumiSoft.Net.Mail.Mail_Message.ParseFromByte(bytes);

                var specifier = SpecifierParse.Parse(dataItem.Section.PartSpecifier);

                var entity = GetEntity(parsed, specifier.Numbers);
                if (entity == null)
                {
                    bytes = new byte[0];
                }
                else
                {
                    switch (specifier.Sole)
                    {
                        case "MIME":
                            bytes = entity.Header.ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
                            break;
                        case "HEADER":
                            bytes = entity.Header.ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
                            bytes = bytes.Concat(new byte[] { 0x0D, 0x0A }).ToArray();
                            break;
                        case "HEADER.FIELDS":
                            bytes = PickHeaderToBytes(entity, new HashSet<string>(dataItem.Section.Paras, StringComparer.OrdinalIgnoreCase), true);
                            bytes = bytes.Concat(new byte[] { 0x0D, 0x0A }).ToArray();
                            break;
                        case "HEADER.FIELDS.NOT":
                            bytes = PickHeaderToBytes(entity, new HashSet<string>(dataItem.Section.Paras, StringComparer.OrdinalIgnoreCase), false);
                            bytes = bytes.Concat(new byte[] { 0x0D, 0x0A }).ToArray();
                            break;
                        //case "TEXT":
                        default:
                            bytes = entity.Body.ToByte();
                            break;
                    }
                }
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

        public List<ImapResponse> RenderBodyStructure(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            // non-extensions BODYSTRUCTURE
            var builder = new StructureBuilder()
                .Append("BODYSTRUCTURE ");

            BODYSTRUCTURE.ConstructParts(builder, message.Parsed, false);

            return new List<ImapResponse>
            {
                new ImapResponse(builder.ToString())
            };
        }

        protected virtual void BeforeRender(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            if (!message.Message.Read)
            {
                message.Message.Read = true;
                maildb.Messages.Update(message.Message);
            }
        }

        public static byte[] PickHeaderToBytes(MIME_Entity entity, HashSet<string> fieldNames, bool includeOrExclude)
        {
            var builder = new StringBuilder();
            foreach (MIME_h header in entity.Header)
            {
                if ((includeOrExclude && fieldNames.Contains(header.Name)) || 
                    (!includeOrExclude && !fieldNames.Contains(header.Name)))
                {
                    builder.Append(header.ToString(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8));
                }
            }

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public static MIME_Entity GetEntity(LumiSoft.Net.Mail.Mail_Message parsed, int[] numbers)
        {
            var entity = parsed as LumiSoft.Net.MIME.MIME_Entity;

            if (numbers == null)
                return entity;

            // Not multipart
            if (!(entity.Body is LumiSoft.Net.MIME.MIME_b_Multipart) && numbers[0] == 1)
                return entity;

            for (int i = 0; i < numbers.Length; i++)
            {
                var parts = entity.Body as LumiSoft.Net.MIME.MIME_b_Multipart;
                if (parts == null)
                    return null;

                var num = numbers[i] - 1;
                if (num < 0 || num >= parts.BodyParts.Count)
                    return null;

                entity = parts.BodyParts[num];
            }

            return entity;
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

    static class MIME_b_Extensions
    {
        public static byte[] ToByte(this MIME_b body)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                body.ToStream(stream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8, false);
                return stream.ToArray();
            }
        }
    }
}
