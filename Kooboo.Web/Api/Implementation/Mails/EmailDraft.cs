//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.Mail;
using Kooboo.Mail.Utility;
using Kooboo.Mail.ViewModel;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailDraft : IApi
    {
        private const int MaxAttachmentSize = 10 * 1024 * 1024;
        private const int MaxImageSize = 1 * 1024 * 1024;

        public string ModelName
        {
            get
            {
                return "EmailDraft";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        public List<TargetAddress> TargetAddresses(ApiCall apiCall)
        {
            //TODO: Return all potential suggestion address.. 
            //var uc = apiCall.UserContext(); 
            //return uc.MailDb.TargetAddresses.All().ToList(); 
            return new List<TargetAddress>();
        }

        public Kooboo.Mail.ViewModel.ComposeViewModel Compose(ApiCall call)
        {
            int messageid = call.GetValue<int>("messageId");
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("messageid", messageid.ToString());
                return EmailForwardManager.Get<Kooboo.Mail.ViewModel.ComposeViewModel>(this.ModelName, nameof(EmailDraft.Compose), call.Context.User, dic);
            }

            if (messageid <= 0)
            {
                return new Mail.ViewModel.ComposeViewModel
                {
                    Subject = string.Empty,
                    Html = string.Empty,
                };
            }
            else
            {
                var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);
                var msg = maildb.Message2.Get(messageid);

                var result = Kooboo.Mail.Utility.ComposeUtility.ToComposeViewModel(msg, call.Context.User);
                result.Html = Kooboo.Mail.Utility.ComposeUtility.RestoreHtmlOrText(call.Context.User, messageid);

                //check to ensure attachment exists... to prevent the attachment was from IMAP client
                if (msg.Attachments != null)
                {
                    foreach (var item in msg.Attachments)
                    {
                        var bytes = Kooboo.Mail.MultiPart.FileService.Get(call.Context.User, item.FileName);

                        var msgBytes = MessageUtility.GetFileBinary(msg.Body, item.FileName);
                        if (msgBytes != null)
                        {
                            if (bytes == null || bytes.Length != msgBytes.Length)
                            {

                                Mail.MultiPart.FileService.Upload(call.Context.User, item.FileName, msgBytes);

                            }
                        }

                    }
                }

                return result;
            }
        }

        [Kooboo.Attributes.RequireModel(typeof(Mail.ViewModel.ComposeViewModel))]
        public long Post(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var json = Kooboo.Lib.Helper.JsonHelper.Serialize(apiCall.Context.Request.Model);
                return EmailForwardManager.Post<int>(this.ModelName, nameof(EmailDraft.Post), apiCall.Context.User, json, null);
            }

            var user = apiCall.Context.User;
            var model = apiCall.Context.Request.Model as Mail.ViewModel.ComposeViewModel;

            var msg = Kooboo.Mail.Utility.ComposeUtility.FromComposeViewModel(model, user);

            msg.FolderId = Folder.ToId(Folder.Drafts);

            msg.Read = true;
            msg.CreationTime = DateTime.UtcNow;

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);

            SetOrCreateSmtpMesageId(maildb, msg, model, user);

            msg.Date = DateTime.UtcNow;
            msg.CreationTime = DateTime.UtcNow;

            if (msg.MsgId > 0)
            {
                model.Html = EmailHelper.ReplaceOldMsg(model.Html, msg.MsgId);
            }

            var newbody = Mail.Utility.ComposeUtility.ComposeMessageBody(model, user, false, msg.SmtpMessageId);

            if (msg.MsgId > 0)
            {
                maildb.Message2.Delete(msg.MsgId);
            }
            maildb.Message2.Add(msg, newbody);

            return msg.MsgId;
        }

        public void SetOrCreateSmtpMesageId(MailDb maildb, Message addMsg, ComposeViewModel viewmodel, User user)
        {
            if (addMsg.MsgId > 0)
            {
                var current = maildb.Message2.Get(addMsg.MsgId);
                if (current != null && !string.IsNullOrWhiteSpace(current.SmtpMessageId))
                {
                    addMsg.SmtpMessageId = current.SmtpMessageId;
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(addMsg.SmtpMessageId))
            {
                string fromAdd = Mail.Utility.ComposeUtility.GetFrom(viewmodel, user);
                addMsg.SmtpMessageId = Kooboo.Mail.Utility.SmtpUtility.GenerateMessageId(fromAdd);
            }
        }
    }
}
