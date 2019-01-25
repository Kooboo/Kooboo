//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

using LumiSoft.Net;
using LumiSoft.Net.MIME;
using LumiSoft.Net.Mail;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class ENVELOPE : ICommandResponse
    {
        public virtual string Name
        {
            get
            {
                return "ENVELOPE"; 
            } 
        }

        public List<ImapResponse> Render(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            var builder = new StructureBuilder();

            ConstructEnvelope(builder, message.Parsed);

            return new List<ImapResponse>
            {
                new ImapResponse(builder.ToString())
            };
        }

        public static void ConstructEnvelope(StructureBuilder builder, Mail_Message entity)
        {
            // date, subject, from, sender, reply-to, to, cc, bcc, in-reply-to, and message-id
            var wordEncoder = new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8);
            wordEncoder.Split = false;

            builder.Append("ENVELOPE").SpaceNBracket();

            // date
            try
            {
                if (entity.Date != DateTime.MinValue)
                {
                    builder.Append(TextUtils.QuoteString(MIME_Utils.DateTimeToRfc2822(entity.Date)));
                }
                else
                {
                    builder.Append("NIL");
                }
            }
            catch
            {
                builder.Append("NIL");
            }

            // subject
            if (entity.Subject != null)
            {
                builder.Append(" " + TextUtils.QuoteString(wordEncoder.Encode(entity.Subject)));
                //string val = wordEncoder.Encode(entity.Subject);
                //builder.Append(" {").Append(val.Length).Append("}\r\n").Append(val);
            }
            else
            {
                builder.AppendNil();
            }

            // from
            if (entity.From != null && entity.From.Count > 0)
            {
                builder.Append(" ");
                ConstructAddresses(builder, entity.From.ToArray(), wordEncoder);
            }
            else
            {
                builder.AppendNil();
            }

            // sender	
            //	NOTE: There is confusing part, according rfc 2822 Sender: is MailboxAddress and not AddressList.
            if (entity.Sender != null)
            {
                builder.SpaceNBracket();

                ConstructAddress(builder, entity.Sender, wordEncoder);

                builder.EndBracket();
            }
            else
            {
                builder.AppendNil();
            }

            // reply-to
            if (entity.ReplyTo != null)
            {
                builder.Append(" ");
                ConstructAddresses(builder, entity.ReplyTo.Mailboxes, wordEncoder);
            }
            else
            {
                builder.Append(" NIL");
            }

            // to
            if (entity.To != null && entity.To.Count > 0)
            {
                builder.Append(" ");
                ConstructAddresses(builder, entity.To.Mailboxes, wordEncoder);
            }
            else
            {
                builder.Append(" NIL");
            }

            // cc
            if (entity.Cc != null && entity.Cc.Count > 0)
            {
                builder.Append(" ");
                ConstructAddresses(builder, entity.Cc.Mailboxes, wordEncoder);
            }
            else
            {
                builder.AppendNil();
            }

            // bcc
            if (entity.Bcc != null && entity.Bcc.Count > 0)
            {
                builder.Append(" ");
                ConstructAddresses(builder, entity.Bcc.Mailboxes, wordEncoder);
            }
            else
            {
                builder.AppendNil();
            }

            // in-reply-to			
            if (entity.InReplyTo != null)
            {
                builder.Append(" ").Append(TextUtils.QuoteString(wordEncoder.Encode(entity.InReplyTo)));
            }
            else
            {
                builder.AppendNil();
            }

            // message-id
            if (entity.MessageID != null)
            {
                builder.Append(" ").Append(TextUtils.QuoteString(wordEncoder.Encode(entity.MessageID)));
            }
            else
            {
                builder.AppendNil();
            }

            builder.EndBracket();
        }

        public static void ConstructAddresses(StructureBuilder builder, Mail_t_Mailbox[] mailboxes, MIME_Encoding_EncodedWord wordEncoder)
        {
            builder.StartBracket();

            foreach (Mail_t_Mailbox address in mailboxes)
            {
                ConstructAddress(builder, address, wordEncoder);
            }

            builder.EndBracket();
        }

        public static void ConstructAddress(StructureBuilder builder, Mail_t_Mailbox address, MIME_Encoding_EncodedWord wordEncoder)
        {
            /* An address structure is a parenthesized list that describes an
			   electronic mail address.  The fields of an address structure
			   are in the following order: personal name, [SMTP]
			   at-domain-list (source route), mailbox name, and host name.
			*/

            // NOTE: all header fields and parameters must in ENCODED form !!!

            builder.StartBracket();

            // personal name
            if (address.DisplayName != null)
            {
                builder.Append(TextUtils.QuoteString(wordEncoder.Encode(RemoveCrlf(address.DisplayName))));
            }
            else
            {
                builder.Append("NIL");
            }

            // source route, always NIL (not used nowdays)
            builder.AppendNil();

            // mailbox name
            builder.Append(" ").Append(TextUtils.QuoteString(wordEncoder.Encode(RemoveCrlf(address.LocalPart))));

            // host name
            if (address.Domain != null)
            {
                builder.Append(" ").Append(TextUtils.QuoteString(wordEncoder.Encode(RemoveCrlf(address.Domain))));
            }
            else
            {
                builder.AppendNil();
            }

            builder.EndBracket();
        }

        private static string RemoveCrlf(string value)
        {
            return value.Replace("\r", "").Replace("\n", "");
        }
    }
}
