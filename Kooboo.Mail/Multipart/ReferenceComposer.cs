//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using dotless.Core.Utils;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Mail.Factory;
using Kooboo.Mail.Utility;
using Kooboo.Mail.ViewModel;
using Microsoft.Ajax.Utilities;
using MimeKit;

namespace Kooboo.Mail.Multipart
{
    public static class ReferenceComposer
    {
        public static ComposeViewModel ComposeForward(int MsgId, RenderContext context, string timeZoneId)
        {
            ComposeViewModel model = new ComposeViewModel();

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(context.User);

            // var msg = maildb.Msgstore.Get(MsgId);
            var msg = maildb.Message2.Get(MsgId);

            model.Subject = msg.Subject;
            // model.Attachments = msg.Attachments;
            model.From = msg.AddressId;

            if (!model.Subject.ToLower().StartsWith("fw:"))
            {
                model.Subject = "Fw:" + model.Subject;
            }

            model.Html = ComposeRefMsg(MsgId, context, timeZoneId);
            model.Attachments = msg.Attachments;

            if (msg.Attachments != null && msg.Attachments.Any())
            {
                var mimeMsg = MessageUtility.ParseMessage(msg.Body);

                foreach (var item in msg.Attachments)
                {
                    var bytes = MessageUtility.GetFileBinary(mimeMsg, item.FileName);
                    if (bytes != null)
                    {
                        Kooboo.Mail.MultiPart.FileService.Upload(context.User, item.FileName, bytes);
                    }
                    else
                    {
                        model.Attachments.RemoveAll(o => o.FileName == item.FileName);
                    }
                }
            }

            return model;
        }

        public static ComposeViewModel ComposeReply(int MsgId, RenderContext context, bool ReplyAll, string timeZoneId)
        {
            ComposeViewModel model = new ComposeViewModel();

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(context.User);

            // var msg = maildb.Msgstore.Get(MsgId);
            var msg = maildb.Message2.Get(MsgId);

            model.Subject = msg.Subject;

            model.From = msg.AddressId;

            if (model.Subject == null)
            {
                model.Subject = "Re:";
            }

            if (!model.Subject.ToLower().StartsWith("re:"))
            {
                model.Subject = "Re:" + model.Subject;
            }

            var mime = MessageUtility.ParseMessage(msg.Body);

            model.Html = ComposeRefMsg(mime, context, MsgId, timeZoneId);

            var (replyto, to) = GetReplyTo(msg);

            if (!string.IsNullOrEmpty(replyto))
            {
                model.To.Add(to);
            }

            if (ReplyAll)
            {
                HashSet<string> excl = new HashSet<string>();
                if (replyto != null)
                {
                    var reply = Utility.AddressUtility.GetAddress(replyto);
                    if (reply != null)
                    {
                        excl.Add(reply.ToLower());
                    }
                }

                if (msg.RcptTo != null)
                {
                    var rcpt = Utility.AddressUtility.GetAddress(msg.RcptTo);
                    if (rcpt != null)
                    {
                        excl.Add(rcpt.ToLower());
                    }
                }

                var all = GetReplyAll(msg, excl);

                foreach (var item in all)
                {
                    model.Cc.Add(item.Replace("\"", ""));
                }
                //all.Add(replyto);
            }


            return model;
        }

        public static ComposeViewModel ComposeReEdit(User user, int MsgId, RenderContext context)
        {
            var content = MessageUtility.GetContentViewModel(user, MsgId);

            var maildb = Factory.DBFactory.UserMailDb(context.User);
            var msg = maildb.Message2.Get(MsgId);

            var mimeMessage = MailKitUtility.LoadMessage(msg.Body);
            var attachments = mimeMessage.Attachments;
            if (attachments != null)
            {
                foreach (var item in attachments)
                {
                    var temp = System.IO.Path.Combine(Data.AppSettings.TempDataPath, "MailAttachment");
                    var folder = System.IO.Path.Combine(temp, user.Id.ToString());
                    Lib.Helper.IOHelper.EnsureDirectoryExists(folder);
                    var bytes = Kooboo.Mail.Utility.MessageUtility.GetFileBinary(msg.Body, item.ContentDisposition.FileName);
                    if (bytes is not null)
                        File.WriteAllBytes(Path.Combine(folder, item.ContentDisposition.FileName), bytes);
                }
            }

            ComposeViewModel model = new ComposeViewModel
            {
                Subject = content.Subject,
                Html = content.Html,
                Cc = ComposeUtility.GetAddressList(content.Cc),
                Bcc = ComposeUtility.GetAddressList(content.Bcc),
                FromAddress = content.From?.Address,
                To = ComposeUtility.GetAddressList(content.To),
                From = msg.AddressId,
                Attachments = msg.Attachments,
            };

            return model;
        }

        /// <summary>
        /// (ReplyTo, To)
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static (string, string) GetReplyTo(Message msg)
        {
            var mime = MessageUtility.ParseMessage(msg.Body);
            var replyto = GetHeader(mime, "reply-to");

            if (string.IsNullOrEmpty(replyto))
            {
                replyto = GetHeader(mime, "From");
            }

            if (string.IsNullOrEmpty(replyto))
            {
                replyto = msg.From;
            }

            if (string.IsNullOrEmpty(replyto))
            {
                replyto = msg.MailFrom;
            }

            if (string.IsNullOrEmpty(replyto))
            {
                replyto = GetHeader(mime, "sender");
            }

            if (!string.IsNullOrEmpty(replyto))
            {
                string displayName = CheckAndGetDisplayName(replyto.Replace("\"", ""));
                return (replyto, displayName.Replace("\"", ""));
            }

            return (replyto, null);
        }

        private static string CheckAndGetDisplayName(string mailAddress)
        {
            if (InternetAddressList.TryParse(mailAddress, out var internetAddresses))
            {
                var displayNames = new List<string>();
                internetAddresses.Mailboxes.ForEach(v =>
                {
                    displayNames.Add(v.ToString());
                });

                string result;

                if (displayNames.Count > 1)
                {
                    result = displayNames.JoinStrings(",");
                }
                else
                {
                    result = displayNames[0];
                }
                if (result != null)
                {
                    result = result.Trim('\\');
                    result = result.Trim('\"');
                    return result;

                }
            }

            return mailAddress;
        }

        public static List<string> GetReplyAll(Message msg, HashSet<string> exclList)
        {
            var toList = new List<MailboxAddress>();
            if (!string.IsNullOrWhiteSpace(msg.To))
            {
                toList.AddRange(Utility.MailKitUtility.GetMailKitAddressList(msg.To));
            }
            if (msg.Cc != null && msg.Cc.IndexOf("@") > -1)
            {
                var ccList = Utility.MailKitUtility.GetMailKitAddressList(msg.Cc);
                if (ccList != null)
                {
                    toList.AddRange(ccList);
                }
            }

            List<string> all = new List<string>();

            foreach (var item in toList)
            {
                var add = item.GetAddress(false);
                if (add != null)
                {
                    if (!exclList.Contains(add.ToLower()))
                    {
                        all.Add(item.ToString());
                    }
                }

            }

            return all;
        }

        public static string ComposeRefMsg(int MsgId, RenderContext context, string timeZoneId)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(context.User);

            // var msgbody = maildb.Msgstore.GetContent(MsgId);
            var msgbody = maildb.Message2.GetContent(MsgId);
            if (msgbody == null)
            {
                return null;
            }

            return ComposeRefMsg(context, msgbody, MsgId, timeZoneId);
        }

        public static string ComposeRefMsg(RenderContext context, string msgbody, int MsgId, string timeZoneId)
        {
            var mime = MessageUtility.ParseMessage(msgbody);
            return ComposeRefMsg(mime, context, MsgId, timeZoneId);
        }

        public static string ComposeRefMsg(MimeMessage mime, RenderContext context, int MsgId, string timeZoneId)
        {
            var bodywithheader = ComposeHeader(mime, context, timeZoneId);

            string mailbody = MessageUtility.GetHtmlBody(mime);
            if (mailbody == null)
            {
                mailbody = MessageUtility.GetTextBody(mime);
            }

            if (mailbody == null)
            {
                return null;
            }

            string htmlbody = BodyComposer.RestoreInlineImages(mailbody, context.User, MsgId);
            return bodywithheader.Replace("{{htmlbody}}", htmlbody);
        }


        public static string ComposeHeader(MimeMessage mime, RenderContext context, string timeZoneId)
        {
            string result = "<br/><br/><hr/><div color=\"#000000\" style=\"font-size:12px;background-color:rgba(204,204,204,0.2);padding:8px;\">\r\n";
            result += "<b>From</b>: " + HttpUtility.HtmlEncode(GetHeader(mime, "from")) + "<br/>\r\n";

            // to
            var to = MessageUtility.GetHeaderValue(mime, "to");
            if (!string.IsNullOrWhiteSpace(to))
            {
                result += "<b>To</b>: " + HttpUtility.HtmlEncode(GetHeader(mime, "to")) + "<br/>\r\n";
            }

            // cc. 
            var cc = MessageUtility.GetHeaderValue(mime, "cc");
            if (!string.IsNullOrWhiteSpace(cc))
            {
                result += "<b>Cc</b>: " + HttpUtility.HtmlEncode(GetHeader(mime, "cc")) + "<br/>\r\n";
            }

            result += "<b>Subject</b>: " + HttpUtility.HtmlEncode(GetHeader(mime, "subject")) + "<br/>\r\n";
            var date = TimeZoneInfo.ConvertTimeFromUtc(mime.Date.ToUniversalTime().DateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
            result += "<b>Date</b>: " + date.ToString("yyyy-MM-dd HH:mm:ss") + "<br/>\r\n";
            result += "</div><br/>{{htmlbody}}";
            return result;
        }


        private static string GetHeader(MimeMessage msg, string header)
        {
            string value = MessageUtility.GetHeaderValue(msg, header);
            if (header.ToLower() == "from" || header.ToLower() == "to" || header.ToLower() == "cc")
            {
                value = CheckAndGetDisplayName(value);
                value = value.Replace("\"", "");
                value = value.Replace("'", "");
                //value = value.Replace("<", "&lt;");
                //value = value.Replace(">", "&gt;");
                value = value.Replace("\\", "");
            }
            return value;
        }


        public static string ComposeCalendarPartStatReply(int messageId, Message msg, int partStat, string sender, RenderContext context)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            string fromAddress = GetSenderInfo(sender, context.User);
            headers.Add("From", fromAddress);
            headers.Add("To", msg.From);
            headers.Add("In-Reply-To", msg.SmtpMessageId);
            headers.Add("References", msg.SmtpMessageId);

            var addressName = AddressUtility.GetDisplayName(fromAddress);
            var mime = MessageUtility.ParseMessage(msg.Body);
            string htmlBody = string.Empty;
            if (partStat == 1)
            {
                headers.Add("Subject", "Accepted: " + msg.Subject.Replace("Cancel: ", "").Replace("Updated Invitation: ", "").Replace("Invitation: ", ""));
                htmlBody = $"<style>.hide-reply-calendar-button{{display:none}} .update-container{{display:none;}}</style><div style=\"color:black; padding: 10px 15px 10px 30px; font-size: 16px; font-weight: bolder; margin-top: 10px; margin-bottom: 10px; background-color: antiquewhite;border-radius: 10px;\">\r\n        {addressName} has accepted this invitation\r\n    </div>\r\n" + mime.HtmlBody;
            }
            else if (partStat == 2)
            {
                headers.Add("Subject", "Declined: " + msg.Subject.Replace("Cancel: ", "").Replace("Updated Invitation: ", "").Replace("Invitation: ", ""));
                htmlBody = $"<style>.hide-reply-calendar-button{{display:none}} .update-container{{display:none;}}</style><div style=\"color:black; padding: 10px 15px 10px 30px; font-size: 16px; font-weight: bolder; margin-top: 10px; margin-bottom: 10px; background-color: antiquewhite;border-radius: 10px;\">\r\n        {addressName} has declined this invitation\r\n    </div>\r\n" + mime.HtmlBody;
            }
            else if (partStat == 3)
            {
                headers.Add("Subject", "Tentatively accepted: " + msg.Subject.Replace("Cancel: ", "").Replace("Updated Invitation: ", "").Replace("Invitation: ", ""));
                htmlBody = $"<style>.hide-reply-calendar-button{{display:none}} .update-container{{display:none;}}</style><div style=\"color:black; padding: 10px 15px 10px 30px; font-size: 16px; font-weight: bolder; margin-top: 10px; margin-bottom: 10px; background-color: antiquewhite;border-radius: 10px;\">\r\n        {addressName} has tentatively accepted this invitation\r\n    </div>\r\n" + mime.HtmlBody;
            }
            string strHeader = Kooboo.Mail.Multipart.HeaderComposer.Compose(headers);

            string dealMsg = string.Empty;
            foreach (var part in mime.BodyParts)
            {
                if (part.ContentType.MimeType.Equals("text/calendar", StringComparison.OrdinalIgnoreCase))
                {
                    dealMsg = ICalendarUtility.ReplyCalendarPartStat(((MimeKit.TextPart)part).Text, partStat, sender);
                    break;
                }
            }
            var bodyComposer = new BodyComposer(htmlBody, mime.TextBody, dealMsg, null, context.User);
            string replyMsg = strHeader + bodyComposer.Body();

            return replyMsg;
        }

        private static string GetSenderInfo(string sender, User user)
        {
            var orgdb = DBFactory.OrgDb(user.CurrentOrgId);
            var senderAddress = AddressUtility.GetAddress(sender);

            if (senderAddress != null)
            {
                var dbaddress = orgdb.Email.Get(senderAddress);
                if (dbaddress != null)
                {
                    var composeViewModel = new ComposeViewModel()
                    {
                        From = dbaddress.Id,
                        FromAddress = sender
                    };

                    var from = ComposeUtility.GetFrom(composeViewModel, user);

                    if (from != null)
                    {
                        return from;
                    }
                }
            }

            return sender;
        }
    }
}
