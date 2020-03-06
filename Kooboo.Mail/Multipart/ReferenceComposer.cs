//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Mail.Utility;
using Kooboo.Data.Language;
using LumiSoft.Net.MIME;
using Kooboo.Mail.ViewModel;

namespace Kooboo.Mail.Multipart
{
    public static class ReferenceComposer
    {

        public static ComposeViewModel ComposeForward(int MsgId, RenderContext context)
        {
            ComposeViewModel model = new ComposeViewModel();

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(context.User);

            var msg = maildb.Messages.Get(MsgId);

            model.Subject = msg.Subject;
            // model.Attachments = msg.Attachments;
            model.From = msg.AddressId;

            if (!model.Subject.ToLower().StartsWith("fw:"))
            {
                model.Subject = "Fw:" + model.Subject;
            }

            model.Html = ComposeRefMsg(MsgId, context); 

            return model;
        }

        public static ComposeViewModel ComposeReply(int MsgId, RenderContext context)
        {
            ComposeViewModel model = new ComposeViewModel();

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(context.User);

            var msg = maildb.Messages.Get(MsgId);

            model.Subject = msg.Subject;
           // model.Attachments = msg.Attachments;
            model.From = msg.AddressId;

            if (!model.Subject.ToLower().StartsWith("re:"))
            {
                model.Subject = "Re:" + model.Subject;
            }

            var msgbody = maildb.MsgBody.Get(msg.BodyPosition);

            var mime = MessageUtility.ParseMineMessage(msgbody);

            model.Html = ComposeRefMsg(mime, context, MsgId);

            var replyto = GetHeader(mime, "reply-to");
            if (string.IsNullOrEmpty(replyto))
            {
                replyto = msg.MailFrom;
            }

            if (msg.FolderId != Folder.ToId("Sent"))
            {
                model.To.Add(Utility.AddressUtility.GetAddress(replyto));
            }

            return model;
        }
         
        public static string ComposeRefMsg(int MsgId, RenderContext context)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(context.User);

            var msgbody = maildb.Messages.GetContent(MsgId);
            if (msgbody == null)
            {
                return null;
            }

            return ComposeRefMsg(context, msgbody, MsgId);
        }

        public static string ComposeRefMsg(RenderContext context, string msgbody, int MsgId)
        {
            var mime = MessageUtility.ParseMineMessage(msgbody);
            return ComposeRefMsg(mime, context, MsgId);
        }

        public static string ComposeRefMsg(MIME_Message mime, RenderContext context, int MsgId)
        {
            var bodywithheader = ComposeHeader(mime, context);

            string mailbody = MessageUtility.GetHtmlBody(mime); 
            if (mailbody == null)
            {
                mailbody = MessageUtility.GetTextBody(mime); 
            }

            if (mailbody == null)
            {
                mailbody = MessageUtility.GetAnyTextBody(mime);
            } 

            string htmlbody = BodyComposer.RestoreInlineImages(mailbody, context.User, MsgId);
            return bodywithheader.Replace("{{htmlbody}}", htmlbody);
        }

        public static string ComposeHeader(string OrginalMessageBody, RenderContext context)
        {
            if (string.IsNullOrEmpty(OrginalMessageBody))
            {
                return null;
            }
            var mime = Kooboo.Mail.Utility.MessageUtility.ParseMineMessage(OrginalMessageBody);
            return ComposeHeader(mime, context);
        }

        public static string ComposeHeader(MIME_Message mime, RenderContext context)
        {
            string result = "<br/><br/><hr/><div color=\"#000000\" style=\"font-size:12px;background:#efefef;padding:8px;\">\r\n";

            result += "<b>" + Hardcoded.GetValue("From", context) + "</b>: " + GetHeader(mime, "from") + "</br>\r\n";
            result += "<b>" + Hardcoded.GetValue("To", context) + "</b>: " + GetHeader(mime, "to") + "</br>\r\n";
            result += "<b>" + Hardcoded.GetValue("Subject", context) + "</b>: " + GetHeader(mime, "subject") + "</br>\r\n";
            result += "<b>" + Hardcoded.GetValue("Date", context) + "</b>: " + GetHeader(mime, "date") + "</br>\r\n";
            result += "</div><div>{{htmlbody}}</div>";
            return result;
        }

        private static string GetHeader(MIME_Message msg, string header)
        {
            string value = MessageUtility.GetHeaderValue(msg, header);
            if (header.ToLower() == "from" || header.ToLower() == "to")
            {
                value = value.Replace("\"", "");
                value = value.Replace("'", "");
                value = value.Replace("<", "&lt;");
                value = value.Replace(">", "&gt;");
            }
            return value;
        }

    }
}
