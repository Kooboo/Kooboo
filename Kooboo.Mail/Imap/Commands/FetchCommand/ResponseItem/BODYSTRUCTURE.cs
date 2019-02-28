//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

using LumiSoft.Net.MIME;

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

        public List<ImapResponse> Render(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            var builder = new StructureBuilder()
                .Append(dataItem.FullItemName)
                .Append(" ");

            ConstructParts(builder, message.Parsed, true);

            return new List<ImapResponse>
            {
                new ImapResponse(builder.ToString())
            };
        }

        public static void ConstructParts(StructureBuilder builder, MIME_Entity entity, bool includeExtensions)
        {
            var wordEncoder = new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8);
            wordEncoder.Split = false;

            if (entity.Body is MIME_b_Multipart)
            {
                // Recursive build for multipart
                builder.StartBracket();

                foreach (MIME_Entity child in ((MIME_b_Multipart)entity.Body).BodyParts)
                {
                    ConstructParts(builder, child, includeExtensions);
                }

                if (entity.ContentType != null && entity.ContentType.SubType != null)
                {
                    builder.SpaceNQuoted(entity.ContentType.SubType.ToUpperInvariant());
                }
                else
                {
                    builder.AppendQuoted("PLAIN");
                }

                if (includeExtensions)
                {
                    // conentTypeParameters - Syntax: {("name" SP "value" *(SP "name" SP "value"))}
                    ConstructTypeParameters(builder, entity, wordEncoder);

                    // body disposition  Syntax: {(disposition-type [ SP ("name" SP "value" *(SP "name" SP "value"))])}
                    ConstructBodyDisposition(builder, entity, wordEncoder);

                    // body language
                    builder.AppendNil();

                    // body location
                    builder.AppendNil();
                }

                builder.Append(")");
            }
            else
            {
                ConstructSinglePart(builder, entity, wordEncoder, includeExtensions);
            }
        }

        public static void ConstructSinglePart(StructureBuilder builder, MIME_Entity entity, MIME_Encoding_EncodedWord wordEncoder, bool includeExtensions)
        {
            builder.Append("(");

            // NOTE: all header fields and parameters must in ENCODED form !!!

            // Add contentTypeMainMediaType
            if (entity.ContentType != null && entity.ContentType.Type != null)
            {
                builder.AppendQuoted(entity.ContentType.Type.ToUpperInvariant());
            }
            else
            {
                builder.AppendQuoted("TEXT");
            }

            // Add contentTypeSubMediaType
            if (entity.ContentType != null && entity.ContentType.SubType != null)
            {
                builder.SpaceNQuoted(entity.ContentType.SubType.ToUpperInvariant());
            }
            else
            {
                builder.SpaceNQuoted("PLAIN");
            }

            // conentTypeParameters - Syntax: {("name" SP "value" *(SP "name" SP "value"))}
            ConstructTypeParameters(builder, entity, wordEncoder);

            // contentID
            string contentID = entity.ContentID;
            if (contentID != null)
            {
                builder.SpaceNQuoted(wordEncoder.Encode(contentID));
            }
            else
            {
                builder.AppendNil();
            }

            // contentDescription
            string contentDescription = entity.ContentDescription;
            if (contentDescription != null)
            {
                builder.SpaceNQuoted(wordEncoder.Encode(contentDescription));
            }
            else
            {
                builder.AppendNil();
            }

            // contentEncoding
            if (entity.ContentTransferEncoding != null)
            {
                builder.SpaceNQuoted(entity.ContentTransferEncoding.ToUpperInvariant());
            }
            else
            {
                // If not specified, then must be 7bit.
                builder.SpaceNQuoted("7bit");
            }

            // contentSize
            if (entity.Body is MIME_b_SinglepartBase)
            {
                builder.Append(" ").Append(((MIME_b_SinglepartBase)entity.Body).EncodedData.Length.ToString());
            }
            else
            {
                builder.Append(" 0");
            }

            // envelope --->FOR ContentType: message / rfc822 ONLY ###
            if (entity.Body is MIME_b_MessageRfc822)
            {
                builder.Append(" ");
                ENVELOPE.ConstructEnvelope(builder, ((MIME_b_MessageRfc822)entity.Body).Message);

                // BODYSTRUCTURE
                builder.AppendNil();

                // LINES
                builder.AppendNil();
            }

            // contentLines ---> FOR ContentType: text/xxx ONLY ###
            if (entity.Body is MIME_b_Text)
            {


                builder.Append(" ").Append(GetLines(entity).ToString());
            }


            #region BODYSTRUCTURE extention fields

            if (includeExtensions)
            {
                // body MD5
                builder.AppendNil();

                // body disposition  Syntax: {(disposition-type [ SP ("name" SP "value" *(SP "name" SP "value"))])}
                ConstructBodyDisposition(builder, entity, wordEncoder);

                // body language
                builder.AppendNil();

                // body location
                builder.AppendNil();
            }

            #endregion

            builder.EndBracket();
        }

        public static int GetLines(MIME_Entity entity)
        {
            var count = 0;
            using (var memory = new MemoryStream(((MIME_b_SinglepartBase)entity.Body).EncodedData))
            {
                using (var reader = new StreamReader(memory))
                {
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        count++;
                        line = reader.ReadLine();
                    }
                }
            }
            return count;
        }

        public static void ConstructTypeParameters(StructureBuilder builder, MIME_Entity entity, MIME_Encoding_EncodedWord wordEncoder)
        {
            if (entity.ContentType != null)
            {
                if (entity.ContentType.Parameters.Count > 0)
                {
                    builder.SpaceNBracket();
                    bool first = true;
                    foreach (MIME_h_Parameter parameter in entity.ContentType.Parameters)
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

                        builder.AppendQuoted(parameter.Name.ToUpperInvariant()).SpaceNQuoted(wordEncoder.Encode(parameter.Value));
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

        public static void ConstructBodyDisposition(StructureBuilder builder, MIME_Entity entity, MIME_Encoding_EncodedWord wordEncoder)
        {
            if (entity.ContentDisposition != null && entity.ContentDisposition.Parameters.Count > 0)
            {
                builder.SpaceNBracket().AppendQuoted(entity.ContentDisposition.DispositionType.ToUpperInvariant());

                if (entity.ContentDisposition.Parameters.Count > 0)
                {
                    builder.SpaceNBracket();

                    bool first = true;
                    foreach (MIME_h_Parameter parameter in entity.ContentDisposition.Parameters)
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

                        builder.AppendQuoted(parameter.Name.ToUpperInvariant()).SpaceNQuoted(wordEncoder.Encode(parameter.Value));
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
    }
}
