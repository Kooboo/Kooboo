//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;

namespace Kooboo.Mail.Utility
{
    public static class SmtpUtility
    {
        public static string GetString(byte[] data)
        {
            var encodingresult = Lib.Helper.EncodingDetector.GetEmailEncoding(data);

            System.Text.Encoding encoding = null;
            if (encodingresult != null && !string.IsNullOrEmpty(encodingresult.Charset))
            {
                encoding = System.Text.Encoding.GetEncoding(encodingresult.Charset);
            }

            if (encoding == null)
            {
                encoding = System.Text.Encoding.UTF8;
            }

            string text = encoding.GetString(data);

            if (text != null && encodingresult != null && !string.IsNullOrWhiteSpace(encodingresult.CharSetText))
            {
                text = text.Replace(encodingresult.CharSetText, "charset=utf8");
            }

            return text;
        }

        public static async Task<List<string>> GetMxRecords(string RcptTo)
        {
            string To = Utility.AddressUtility.GetAddress(RcptTo);
            int index = To.IndexOf("@");
            if (index > -1)
            {
                string domain = To.Substring(index + 1);
                return Kooboo.Lib.DnsRequest.RequestManager.GetMx(domain);
            }
            return null;
        }

        public static bool IsValidMailFrom(string fromAddress)
        {
            if (fromAddress == null)
            {
                return false;
            }

            var add = GetEmailFromMailFromLine(fromAddress);
            return add != null;

        }

        public static string GetEmailFromMailFromLine(string MailFromLine)
        {
            MailFromLine = MailFromLine.ToLower();

            //SIZE=  number_of_bytes
            // BODY = 7BIT
            //BODY = 8BITMIME
            //<from@mycode.dev> SIZE=597  
            string address = Utility.AddressUtility.GetAddress(MailFromLine);

            if (address != null && AddressUtility.IsValidEmailAddress(address))
            {
                return address;
            }

            var indexLeft = MailFromLine.IndexOf("<");
            var indexRight = MailFromLine.IndexOf(">");
            if (indexLeft > -1 && indexRight > -1)
            {
                address = MailFromLine.Substring(indexLeft + 1, indexRight - indexLeft - 1);
                if (address != null && AddressUtility.IsValidEmailAddress(address))
                {
                    return address;
                }
            }

            if (address == null)
            {
                var indexS = MailFromLine.IndexOf(" size");
                var indexb = MailFromLine.IndexOf(" body");

                if (indexS > -1 || indexb > -1)
                {
                    int startIndex = 0;

                    if (indexS > -1)
                    {
                        startIndex = indexS;
                    }

                    if (indexb > -1)
                    {
                        if (startIndex <= 0)
                        {
                            startIndex = indexb;
                        }
                        else if (indexb < startIndex)
                        {
                            startIndex = indexb;
                        }
                    }

                    var sub = MailFromLine.Substring(0, startIndex);

                    address = Utility.AddressUtility.GetAddress(sub);

                    if (address != null && AddressUtility.IsValidEmailAddress(address))
                    {
                        return address;
                    }

                    if (sub != null && AddressUtility.IsValidEmailAddress(sub))
                    {
                        return sub;
                    }
                }

            }
            return null;
        }

        public static string GenerateMessageId(string mailFrom)
        {
            var mailbox = AddressUtility.GetAddress(mailFrom);

            return _generateFromStrictAddress(mailbox);
        }

        private static string _generateFromStrictAddress(string emailaddress)
        {
            if (emailaddress != null)
            {
                var seg = AddressUtility.ParseSegment(emailaddress);
                if (!string.IsNullOrEmpty(seg.Host))
                {
                    return "<" + System.Guid.NewGuid().ToString().Replace("-", "") + "@" + seg.Host + ">";
                }
            }

            return "<" + Guid.NewGuid().ToString().Replace("-", "") + "@mailprotected.com>";
        }

        public static string GenerateMessageId(MailboxAddress add)
        {
            if (add != null)
            {
                return _generateFromStrictAddress(add.GetAddress(false));
            }

            return _generateFromStrictAddress(null);
        }

        public static string GenerateMessageId(InternetAddressList add)
        {
            if (add != null)
            {
                foreach (var item in add)
                {
                    var mailbox = item as MailboxAddress;
                    if (mailbox != null)
                    {
                        return GenerateMessageId(mailbox);
                    }
                }
            }

            return _generateFromStrictAddress(null);
        }
    }

}


/*
 * The MAIL FROM: command is used (once), after a HELO or EHLO command, to identify the sender of a piece of mail.
Read syntax diagramSkip visual syntax diagram
MAIL FROM:
<
 sender_path_address
>
SIZE=
 number_of_bytes
BODY=7BIT
BODY=8BITMIME
Operand
Description
sender_path_address
Specifies the full path address of the sender of the mail. Definitions for valid sender_path_address specifications can be obtained from the RFCs that define the naming conventions used throughout the Internet. For detailed information, consult the RFCs listed in the section SMTP Commands.
SIZE=number_of_bytes
Specifies the size of the mail, in bytes, including carriage return/line feed (CRLF, X'0D0A') pairs. The SIZE parameter has a range from 0 to 2,147,483,647.
BODY=7BIT
Specifies that the message is encoded using seven significant bits per 8-bit octet (byte). In practice, however, the body is typically encoded using all eight bits.
BODY=8BITMIME
Specifies that the message is encoded using all eight bits of each octet (byte) and may contain MIME headers.
Note: The SIZE, BODY=7BIT, and BODY=8BITMIME options of the MAIL FROM: command should be used only if an EHLO command was used to initiate a mail transaction. If an EHLO command was not used for this purpose, SMTP ignores these parameters if they are present.
If the SMTP server is known to support the SMTP service extension for Message Size Declaration, the client sending the mail can specify the optional SIZE= parameter with its MAIL FROM: commands. The client then can use the responses to these commands to determine whether the receiving SMTP server has sufficient resources available to process its mail before any data is transmitted to that server.

When a MAIL FROM: command is received that includes the optional SIZE= parameter, the SMTP server compares the supplied number_of_bytes value to its allowed maximum message size (defined by the MAXMAILBYTES statement in the SMTP CONFIG file) to determine if the mail should be accepted. If number_of_bytes exceeds the MAXMAILBYTES value, a reply code 552 is returned to the client.
 * 
 * 
 */