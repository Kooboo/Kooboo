//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Kooboo.Data.Models;
using Kooboo.Mail.Factory;
using MimeKit;

namespace Kooboo.Mail.Utility
{
    public static class ComposeUtility
    {
        public static ViewModel.ComposeViewModel ToComposeViewModel(Message message, User user)
        {
            ViewModel.ComposeViewModel model = new ViewModel.ComposeViewModel();

            var fromaddress = AddressUtility.GetAddress(message.From);
            var orgdb = Factory.DBFactory.OrgDb(user.CurrentOrgId);
            var dbaddress = orgdb.Email.Get(fromaddress);
            if (dbaddress != null)
            {
                model.From = dbaddress.Id;
            }
            model.MessageId = (int)message.MsgId;
            model.Cc = GetAddressList(message.Cc);
            model.Bcc = GetAddressList(message.Bcc);
            model.To = GetAddressList(message.To);
            model.Subject = message.Subject;
            model.Attachments = message.Attachments;
            return model;
        }

        public static Message FromComposeViewModel(ViewModel.ComposeViewModel model, User user)
        {
            Message msg = new Message();

            if (model.MessageId != null)
            {
                msg.Id = model.MessageId.Value;
                msg.MsgId = model.MessageId.Value;
            }
            msg.From = GetFrom(model, user);
            msg.Cc = ToAddress(model.Cc);
            msg.Bcc = ToAddress(model.Bcc);
            msg.To = ToAddress(model.To);
            msg.Subject = model.Subject;
            msg.AddressId = model.From;
            msg.OutGoing = true;

            if (model.Attachments != null && model.Attachments.Count > 0)
            {
                // fill in the attachment info.  
                foreach (var item in model.Attachments)
                {
                    if (item.Size <= 0)
                    {
                        item.Size = Kooboo.Mail.MultiPart.FileService.GetSize(user, item.FileName);
                    }

                    if (string.IsNullOrEmpty(item.Type) || string.IsNullOrEmpty(item.SubType))
                    {
                        var mimetype = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetMimeMapping(item.FileName);
                        int index = mimetype.IndexOf("/");
                        if (index == -1)
                        {
                            index = mimetype.IndexOf("\\");
                        }

                        if (index > 0)
                        {
                            item.Type = mimetype.Substring(0, index);
                            item.SubType = mimetype.Substring(index + 1);
                        }
                        else
                        {
                            item.Type = mimetype;
                        }
                    }

                    msg.Attachments.Add(item);
                }
            }

            return msg;
        }

        public static string GetFrom(ViewModel.ComposeViewModel model, User user)
        {
            var orgdb = Factory.DBFactory.OrgDb(user.CurrentOrgId);
            var dbaddress = orgdb.Email.Get(model.From);
            if (dbaddress != null)
            {
                var add = dbaddress.Address;
                if (dbaddress.AddressType == EmailAddressType.Wildcard)
                {
                    add = model.FromAddress;
                }

                string name = null;

                if (!string.IsNullOrEmpty(dbaddress.Name))
                {
                    // return dbaddress.Name + " <" + add + ">";
                    name = dbaddress.Name;
                }

                else if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
                {
                    // return user.FirstName + " " + user.LastName + " <" + add + ">";
                    name = user.FirstName + " " + user.LastName;
                }
                else
                {
                    name = AddressUtility.GetDisplayName(dbaddress.Address);
                }

                string FullAddress = name + "<" + dbaddress.Address + ">";

                return FullAddress;

                ///string FullAddress = "\"" + name + "\"<" + dbaddress.Address + ">";
                if (MailboxAddress.TryParse(FullAddress, out var internetadd))
                {
                    return internetadd.ToString();
                }

                MailboxAddress mailbox = new MailboxAddress(name, dbaddress.Address);
                return mailbox.ToString();
            }
            return null;
        }

        public static string ComposeMessageBody(ViewModel.ComposeViewModel model, User user, bool IncludeBcc = false, string msgId = null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            var maildb = DBFactory.OrgDb(user.CurrentOrgId);

            string fromAdd = GetFrom(model, user);
            headers.Add("From", fromAdd);
            var to = model.To;
            var cc = model.Cc;

            headers.Add("To", ToAddress(to));
            if (cc != null && cc.Any())
            {
                headers.Add("Cc", ToAddress(cc));
            }

            if (IncludeBcc)
            {
                var bcc = model.Bcc;

                if (bcc != null && bcc.Any())
                {
                    headers.Add("Bcc", ToAddress(bcc));
                }
            }

            headers.Add("Subject", model.Subject);

            if (msgId == null)
            {
                msgId = Kooboo.Mail.Utility.SmtpUtility.GenerateMessageId(fromAdd);
            }

            headers.Add("Message-ID", msgId);

            string strHeader = Kooboo.Mail.Multipart.HeaderComposer.Compose(headers);

            if (model.Calendar is null)
            {
                var bodycomposer = new Kooboo.Mail.Multipart.BodyComposer(model.Html, model.Attachments, user);
                return strHeader + bodycomposer.Body();
            }
            else
            {
                string calendarContent = ICalendarUtility.GenerateICalendarContent(model.Calendar);
                var bodycomposer = new Kooboo.Mail.Multipart.BodyComposer(model.Html, null, calendarContent, model.Attachments, user);
                return strHeader + bodycomposer.Body();
            }
        }

        public static string ComposeUpdateOrCancelEventBody(ViewModel.ComposeViewModel model, User user, bool IncludeBcc = false, string msgId = null, bool isCancel = false)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            var maildb = DBFactory.OrgDb(user.CurrentOrgId);

            string fromAdd = GetFrom(model, user);

            headers.Add("From", fromAdd);
            var to = model.To;

            List<string> toReal = new List<string>();
            foreach (var item in to)
            {
                var x = MessageUtility.GetAddressModel(item);
                EmailAddress emailAddress = maildb.Email.Get(x.Address);
                if (emailAddress != null && !string.IsNullOrEmpty(emailAddress.ForwardAddress))
                {
                    toReal.Add(emailAddress.ForwardAddress);
                }
                else
                {
                    toReal.Add(item);
                }
            }
            to = toReal;
            headers.Add("To", ToAddress(to));

            var cc = model.Cc;
            if (cc != null && cc.Any())
            {
                headers.Add("Cc", ToAddress(cc));
            }

            if (IncludeBcc)
            {
                var bcc = model.Bcc;

                if (bcc != null && bcc.Any())
                {
                    headers.Add("Bcc", ToAddress(bcc));
                }
            }

            headers.Add("Subject", model.Subject);

            if (msgId == null)
            {
                msgId = Kooboo.Mail.Utility.SmtpUtility.GenerateMessageId(fromAdd);
            }

            headers.Add("Message-ID", msgId);

            string strHeader = Kooboo.Mail.Multipart.HeaderComposer.Compose(headers);

            if (model.Calendar is null)
            {
                var bodycomposer = new Kooboo.Mail.Multipart.BodyComposer(model.Html, model.Attachments, user);
                return strHeader + bodycomposer.Body();
            }
            else
            {
                string calendarContent = ICalendarUtility.UpdateAndCancelICalendar(model.Calendar, isCancel);
                var bodycomposer = new Kooboo.Mail.Multipart.BodyComposer(model.Html, null, calendarContent, model.Attachments, user);
                return strHeader + bodycomposer.Body();
            }

        }

        public static string ComposeMessageBodySaveSent(ViewModel.ComposeViewModel model, User user)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("From", GetFrom(model, user));

            var to = model.To;
            var cc = model.Cc;
            var bcc = model.Bcc;

            headers.Add("To", ToAddress(to));

            if (cc != null && cc.Any())
            {
                headers.Add("Cc", ToAddress(cc));
            }

            if (bcc != null && bcc.Any())
            {
                headers.Add("Bcc", ToAddress(bcc));
            }

            headers.Add("Subject", model.Subject);

            string strHeader = Kooboo.Mail.Multipart.HeaderComposer.Compose(headers);

            if (model.Calendar is null)
            {
                var bodycomposer = new Kooboo.Mail.Multipart.BodyComposer(model.Html, model.Attachments, user);
                return strHeader + bodycomposer.Body();
            }
            else
            {
                string calendarContent = ICalendarUtility.GenerateICalendarContent(model.Calendar);
                var bodycomposer = new Kooboo.Mail.Multipart.BodyComposer(model.Html, null, calendarContent, model.Attachments, user);
                return strHeader + bodycomposer.Body();
            }
        }

        public static string ComposeTextEmailBody(string from, string to, string subject, string textbody)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("From", from);
            headers.Add("To", to);
            headers.Add("Subject", subject);
            string strHeader = Multipart.HeaderComposer.Compose(headers);

            string contenttype = "Content-Type:text/plain";

            var charset = Lib.Helper.EncodingDetector.GetTextCharset(textbody);
            if (charset != null && charset.ToLower() != "ascii")
            {
                contenttype += "; CharSet=" + charset + ";\r\n Content-Transfer-Encoding:8bit";
            }

            strHeader += contenttype + "\r\n";

            string body = strHeader + "\r\n" + textbody;
            return body;
        }

        public static string ComposeHtmlTextEmailBody(string from, string to, string subject, string htmlBody, string textBody)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("From", from);
            headers.Add("To", to);
            headers.Add("Subject", subject);
            string strHeader = Multipart.HeaderComposer.Compose(headers);

            var bodyComposer = new Multipart.BodyComposer(htmlBody, textBody);
            return strHeader + bodyComposer.Body();
        }


        public static string RestoreHtmlOrText(User user, int MsgId)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);
            var mine = maildb.Message2.GetMimeMessageContent(MsgId);
            var body = mine.ToString();

            if (mine == null)
            {
                return null;
            }

            string html = MessageUtility.GetHtmlBody(mine);

            if (!string.IsNullOrWhiteSpace(html))
            {
                return Kooboo.Mail.Multipart.BodyComposer.RestoreInlineImages(html, user, MsgId);
            }

            html = MessageUtility.GetTextBody(mine);
            if (!string.IsNullOrEmpty(html))
            {
                return "<pre>" + html.Replace("<", "&lt;").Replace(">", "&gt;") + "</pre>";
            }

            if (MessageUtility.GetAllAttachmentZip(body) != null && string.IsNullOrEmpty(html))
            {
                return "";
            }

            if (string.IsNullOrEmpty(mine.TextBody) && string.IsNullOrEmpty(mine.HtmlBody))
            {
                return "";
            }

            int index = body.IndexOf("\r\n\r\n");

            if (index > 0)
            {
                return "<pre>" + body.Substring(index + 2) + "</pre>";
            }
            else
            {
                return "<pre>" + body + "</pre>";
            }

        }

        //   internal static string ToAddress(List<string> address)
        //  {
        //var result = new List<string>();
        //foreach(var item in address)
        //{
        //   // string mailAddress = AddQuotationInMailAddressName(item);
        //    if (MailboxAddress.TryParse(item, out var mailboxAddress))
        //        result.Add(mailboxAddress.ToString());
        //    else
        //        result.Add(item);
        //}
        //return string.Join(",", result);

        //  }

        internal static string ToAddress(List<string> address)
        {
            return string.Join(",", address);
        }

        private static string AddQuotationInMailAddressName(string item)
        {
            var index = item.LastIndexOf("<");
            var mailAddress = item;
            if (index > 0)
            {
                mailAddress = $"\"{item}";
                mailAddress = mailAddress.Insert(index, "\"");
            }
            return mailAddress;
        }

        internal static List<string> GetAddressList(string address)
        {
            var models = MessageUtility.GetAddressModels(address);

            return GetAddressList(models);
        }

        internal static List<string> GetAddressList(List<ViewModel.AddressModel> models)
        {
            List<string> result = new List<string>();
            if (models == null)
            {
                return result;
            }

            foreach (var item in models)
            {
                var add = item.Name + " <" + item.Address + ">";
                result.Add(add);
            }

            return result;
        }

        // For forward type email address. 
        public static string ComposeForwardAddressMessage(string MessageSource)
        {
            var msg = MessageUtility.ParseMessage(MessageSource);

            var find = msg.Headers.FirstOrDefault(o => o.Field == "x-kforward");

            if (find != null && find.Field == "x-kforward" && find.Value != null)
            {
                return null;
            }

            msg.Headers.Add("x-kforward", "via Kooboo");

            var newSource = msg.ToString().Trim();

            return newSource;

        }

        public static string ComposeGroupMail(string MessageSource, string NewReplyTo)
        {
            // send the group mail...
            // Change reply-to header to the group email address...ReGenerate the emai....

            var msg = MessageUtility.ParseMessage(MessageSource);

            if (msg.Headers.Contains(MimeKit.HeaderId.ReplyTo))
            {
                msg.Headers.Replace(MimeKit.HeaderId.ReplyTo, NewReplyTo);
            }
            else
            {
                msg.Headers.Add(MimeKit.HeaderId.ReplyTo, NewReplyTo);
            }

            // also change from. otherwise, message can not be delivered,may due to SPF or others. 


            msg.Headers.Add(MimeKit.HeaderId.ResentFrom, msg.From.Mailboxes.First().Address);

            msg.Sender = msg.From.Mailboxes.FirstOrDefault();

            msg.From.Clear();
            msg.From.Add(new MailboxAddress("Group Mail", NewReplyTo));

            var newSource = msg.ToString().Trim();

            return newSource;
        }

        public static string ComposeDeliveryFailed(string MailFrom, string RcptTo, string OrginalMessageBody, string errorMessage)
        {
            // string mailfrom, string rcptto, string messagebody, string errorreason
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Subject", "Delivery Status Notification (Failure)");
            headers.Add("From", "postmaster@noreply.kooboo.net");
            headers.Add("To", MailFrom);

            var newheader = Multipart.HeaderComposer.Compose(headers);

            string html = "<p>This is an automatically generated Delivery Status Notification.</p>";
            html += "<p>Delivery to recipient " + RcptTo + " failed for: </p>";
            html += "<p>" + errorMessage + "</p>";

            var bodycomposer = new Multipart.BodyComposer(html);

            return newheader + bodycomposer.Body();
        }

        private static string GetMessageBodyPart(ref string MessageSource)
        {
            int index = MessageSource.IndexOf("\r\n\r\n");
            if (index > 0)
            {
                return MessageSource.Substring(index + 4);
            }
            return null;
        }

        public static TransferEncoding GetTransferEncoding(ref string textbody, ref string htmlbody)
        {
            int totallen = 0;
            int biggercount = 0;

            if (textbody != null)
            {
                totallen += textbody.Length;

                for (int i = 0; i < textbody.Length; i++)
                {
                    if (textbody[i] > 127)
                    {
                        biggercount += 1;
                    }
                }
            }
            if (htmlbody != null)
            {
                totallen += htmlbody.Length;

                for (int i = 0; i < htmlbody.Length; i++)
                {
                    if (htmlbody[i] > 127)
                    {
                        biggercount += 1;
                    }
                }
            }

            if (biggercount == 0)
            {
                return TransferEncoding.Unknown;
            }
            else
            {
                // 5% of bigger char, make it into base64.

                return TransferEncoding.Base64;

                // int percent5 = (int)totallen / 10;

                //if (biggercount > percent5)
                //{
                //    return TransferEncoding.Base64;
                //}
                //else
                //{
                //    return TransferEncoding.QuotedPrintable;
                //}
            }

        }

        public static TransferEncoding GetTransferEncoding(ref string MsgBody)
        {
            if (MsgBody == null)
            {
                return TransferEncoding.Unknown;
            }

            int totalLen = 0;
            int biggerCount = 0;

            if (MsgBody != null)
            {
                totalLen += MsgBody.Length;

                for (int i = 0; i < MsgBody.Length; i++)
                {
                    if (MsgBody[i] > 127)
                    {
                        biggerCount += 1;
                    }
                }
            }

            if (biggerCount == 0)
            {
                return TransferEncoding.Unknown;
            }
            else
            {
                // 5% of bigger char, make it into base64. 
                if (biggerCount * 20 > totalLen)
                {
                    return TransferEncoding.Base64;
                }
                else
                {
                    return TransferEncoding.QuotedPrintable;
                }
            }

        }





        public static string GetTransferEncodingHeader(TransferEncoding transferEncoding)
        {
            //Content - Transfer - Encoding := "BASE64" / "QUOTED-PRINTABLE" /
            //  "8BIT" / "7BIT" /
            // "BINARY" / x - token 
            if (transferEncoding == TransferEncoding.QuotedPrintable)
            {
                return "Content-Transfer-Encoding:QUOTED-PRINTABLE";
            }
            else if (transferEncoding == TransferEncoding.Base64)
            {
                return "Content-Transfer-Encoding:BASE64";
            }
            return null;
        }

        public static string GetEncoding(ref string textbody, ref string htmlbody)
        {
            if (textbody != null)
            {
                for (int i = 0; i < textbody.Length; i++)
                {
                    if (textbody[i] > 127)
                    {
                        return "UTF-8";
                    }
                }
            }
            if (htmlbody != null)
            {
                for (int i = 0; i < htmlbody.Length; i++)
                {
                    if (htmlbody[i] > 127)
                    {
                        return "UTF-8";
                    }
                }
            }

            return null;
        }


        public static string GetEncoding(ref string MsgBody)
        {
            if (MsgBody != null)
            {
                for (int i = 0; i < MsgBody.Length; i++)
                {
                    if (MsgBody[i] > 127)
                    {
                        return "UTF-8";
                    }
                }
            }
            return null;
        }

        public static string Encode(string input, TransferEncoding transferEncoding)
        {
            if (transferEncoding == TransferEncoding.QuotedPrintable)
            {
                return Kooboo.Mail.Smtp.QuotedPrintable.Encode(input);
            }
            else if (transferEncoding == TransferEncoding.Base64)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                return ToBase64(bytes);
            }

            return input;
        }


        public static string ToBase64(byte[] binary)
        {
            var value = Convert.ToBase64String(binary);

            var len = value.Length;

            StringBuilder sb = new StringBuilder();

            int index = 0;
            int nextindex = 0;

            while (true)
            {
                nextindex = index + 75;
                if (nextindex > len)
                {
                    sb.Append(value.Substring(index)).Append("\r\n");
                    break;
                }
                else
                {
                    sb.Append(value.Substring(index, 75)).Append("\r\n");
                    index = index + 75;
                }
            }
            return sb.ToString();
        }


    }
}
