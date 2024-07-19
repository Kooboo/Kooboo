//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Mail.Multipart;
using Kooboo.Mail.Utility;
using MimeKit;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{

    /*ENVELOPE
    The envelope structure of the message.This is computed by the
    server by parsing the[RFC - 2822] header into the component
    parts, defaulting various fields as necessary
        */
    public class ENVELOPE : ICommandResponse
    {
        public virtual string Name
        {
            get
            {
                return "ENVELOPE";
            }
        }

        public List<ImapResponse> Render(MailDb mailDb, FetchMessage message, DataItem dataItem)
        {
            var builder = new StructureBuilder();

            ConstructEnvelope(builder, message.MailHeader);

            return new List<ImapResponse>
            {
                new ImapResponse(builder.ToString())
            };
        }

        public static void ConstructEnvelope(StructureBuilder builder, HeaderList headerList)
        {
            var HeaderReader = new HeaderListReader(headerList);

            // date, subject, from, sender, reply-to, to, cc, bcc, in-reply-to, and message-id 
            builder.Append("ENVELOPE").SpaceNBracket();

            // date
            try
            {
                if (HeaderReader.Date != default(DateTimeOffset))
                {
                    builder.Append(TextUtils.QuoteString(MimeKit.Utils.DateUtils.FormatDate(HeaderReader.Date)));
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
            if (HeaderReader.Subject != null)
            {
                builder.Append(" " + TextUtils.QuoteString(Utility.HeaderUtility.EncodeFieldB(HeaderReader.Subject)));
            }
            else
            {
                builder.AppendNil();
            }

            // from
            if (HeaderReader.From != null && HeaderReader.From.Count > 0)
            {
                ConstructAddresses(builder, HeaderReader.From.Mailboxes);
            }
            else
            {
                builder.AppendNil();
            }

            // sender	
            //	NOTE: There is confusing part, according rfc 2822 Sender: is MailboxAddress and not AddressList.
            if (HeaderReader.Sender != null)
            {
                builder.SpaceNBracket();

                ConstructAddress(builder, HeaderReader.Sender);

                builder.EndBracket();
            }
            else
            {
                builder.AppendNil();
            }

            // reply-to
            if (HeaderReader.ReplyTo != null)
            {
                ConstructAddresses(builder, HeaderReader.ReplyTo.Mailboxes);
            }
            else
            {
                builder.Append(" NIL");
            }

            // to
            if (HeaderReader.To != null && HeaderReader.To.Count > 0)
            {
                ConstructAddresses(builder, HeaderReader.To.Mailboxes);
            }
            else
            {
                builder.AppendNil();
            }

            // cc
            if (HeaderReader.Cc != null && HeaderReader.Cc.Count > 0)
            {
                ConstructAddresses(builder, HeaderReader.Cc.Mailboxes);
            }
            else
            {
                builder.AppendNil();
            }

            // bcc
            if (HeaderReader.Bcc != null && HeaderReader.Bcc.Count > 0)
            {
                ConstructAddresses(builder, HeaderReader.Bcc.Mailboxes);
            }
            else
            {
                builder.AppendNil();
            }

            // in-reply-to			
            if (HeaderReader.InReplyTo != null)
            {
                builder.Append(" ").Append(TextUtils.QuoteString(Utility.HeaderUtility.EncodeFieldB(HeaderReader.InReplyTo)));
            }
            else
            {
                builder.AppendNil();
            }

            // message-id
            if (HeaderReader.MessageId != null)
            {
                builder.Append(" ").Append(TextUtils.QuoteString(Utility.HeaderUtility.EncodeFieldB(HeaderReader.MessageId)));
            }
            else
            {
                builder.AppendNil();
            }

            builder.EndBracket();
        }


        public static void ConstructAddresses(StructureBuilder builder, IEnumerable<MailboxAddress> mailboxes)
        {

            if (mailboxes != null && mailboxes.Any())
            {
                builder.Append(" ");

                builder.StartBracket();

                foreach (var address in mailboxes)
                {
                    ConstructAddress(builder, address);
                }

                builder.EndBracket();
            }

            else
            {
                builder.AppendNil();
            }

        }

        public static void ConstructAddress(StructureBuilder builder, MailboxAddress address)
        {
            /* An address structure is a parenthesized list that describes an
			   electronic mail address.  The fields of an address structure
			   are in the following order: personal name, [SMTP]
			   at-domain-list (source route), mailbox name, and host name.
			*/

            // NOTE: all header fields and parameters must in ENCODED form !!!

            builder.StartBracket();

            // personal name
            if (address.Name != null)
            {
                builder.Append(TextUtils.QuoteString(Utility.HeaderUtility.EncodeFieldB(RemoveCrlf(address.Name))));
            }
            else
            {
                builder.Append("NIL");
            }

            // source route, always NIL (not used nowdays)
            builder.AppendNil();

            var emailseg = Utility.AddressUtility.ParseSegment(address.Address);

            // mailbox name
            builder.Append(" ").Append(TextUtils.QuoteString(Utility.HeaderUtility.EncodeFieldB(RemoveCrlf(emailseg.Address))));

            // host name
            if (emailseg.Host != null)
            {
                builder.Append(" ").Append(TextUtils.QuoteString(Utility.HeaderUtility.EncodeFieldB(RemoveCrlf(emailseg.Host))));
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
