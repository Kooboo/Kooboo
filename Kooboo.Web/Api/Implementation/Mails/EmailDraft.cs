//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Collections.Generic; 
using Kooboo.Mail; 
using Kooboo.Api; 

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailDraft : IApi
    {
        private const int MaxAttachmentSize = 10 * 1024 * 1024;
        private const int MaxImageSize = 1 * 1024 * 1024;

        public   string ModelName
        {
            get
            {
                return "EmailDraft";
            }
        }
         
        public   bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public   bool RequireUser
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
                return new Mail.ViewModel.ComposeViewModel();
            }
            else
            {
                var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);
                var msg = maildb.Messages.Get(messageid);

                var result = Kooboo.Mail.Utility.ComposeUtility.ToComposeViewModel(msg, call.Context.User);
                result.Html = Kooboo.Mail.Utility.ComposeUtility.RestoreHtmlOrText(call.Context.User, messageid);
                return result;
            }
        }
        
        [Kooboo.Attributes.RequireModel(typeof(Mail.ViewModel.ComposeViewModel))]
        public int Post(ApiCall apiCall)
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
              
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user); 
            if (msg.Id > 0)
            {
                maildb.Messages.Update(msg, Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, user));
            }
            else
            {
                maildb.Messages.Add(msg, Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, user));
            }

            return msg.Id; 
        }
    }
}
