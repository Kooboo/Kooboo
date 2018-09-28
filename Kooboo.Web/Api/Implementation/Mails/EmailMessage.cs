using System;
using System.Collections.Generic;
using Kooboo.Mail;
using Kooboo.Api;
using Kooboo.Mail.ViewModel;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailMessageApi : IApi
    {
        public const int PageSize = 30;
        public string ModelName
        {
            get
            {
                return "EmailMessage";
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

        public List<Message> List(ApiCall call)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            int addressid = EmailAddress.ToId(call.GetValue("address"));

            var folderid = Folder.ToId(call.GetValue("Folder"));

            var query = maildb.Messages.Query().Where(o => o.FolderId == folderid);
            if (addressid != 0)
            {
                query.Where(o => o.AddressId == addressid);
            } 
            var list = query.OrderByDescending(o => o.Id).Take(PageSize);

            MarkStatus(maildb, list);

            return list;
        }


        public List<Message> More(ApiCall call)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            var add = call.GetValue("address");

            int addressid = EmailAddress.ToId(add);

            var folderid = Folder.ToId(call.GetValue("folder"));

            var messageid = call.GetValue<int>("messageId");

            if (messageid <= 0)
            {
                messageid = int.MaxValue;
            }

            var query = maildb.Messages.Query().Where(o => o.FolderId == folderid && o.Id < messageid);
           // var list = maildb.Messages.Query().Where(o => o.AddressId == addressid && o.FolderId == folderid && o.Id < messageid).OrderByDescending(o => o.Id).Take(PageSize);
            if (addressid != 0)
            {
                query.Where(o => o.AddressId == addressid);
            } 
            var list = query.OrderByDescending(o => o.Id).Take(PageSize);
              
            MarkStatus(maildb, list);

            return list;
        }
         
        private void MarkStatus(MailDb maildb, List<Message> msgs)
        {
            foreach (var item in msgs)
            {
                var status = maildb.Messages.Store.GetFromColumns(item.Id);
                if (status != null)
                {
                    item.Read = status.Read;
                    item.Answered = status.Answered;
                    item.Deleted = status.Deleted;
                    item.Flagged = status.Flagged;
                }

                if (item.Date == default(DateTime))
                {
                    item.Date = item.CreationTime;
                }
            }
        }

        public ContentViewModel Content(ApiCall call)
        {
            int messageid = call.GetValue<int>("messageId");

            if (messageid <= 0)
            {
                return new ContentViewModel();
            }
            else
            {
                var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);
                maildb.Messages.MarkAsRead(messageid);

                return Mail.Utility.MessageUtility.GetContentViewModel(call.Context.User, messageid);
            }
        }

        [Kooboo.Attributes.RequireModel(typeof(ComposeViewModel))]
        public void Send(ApiCall call)
        { 
            var user = call.Context.User;
            var model = call.Context.Request.Model as Mail.ViewModel.ComposeViewModel;
            var msg = Kooboo.Mail.Utility.ComposeUtility.FromComposeViewModel(model, user);

            string messagebody = Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, user);

            List<string> rcpttos = new List<string>(model.To);
            rcpttos.AddRange(model.Cc);
            rcpttos.AddRange(model.Bcc);

            string fromaddress = Mail.Utility.AddressUtility.GetAddress(msg.From);

            var msginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(messagebody);

            if (rcpttos.Count>0)
            { 
                // verify sending quota.
                if (!Kooboo.Data.Authorization.QuotaControl.CanSendEmail(call.Context.User.CurrentOrgId, rcpttos.Count))
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("you have no enough credit to send emails", call.Context));
                }

                // save sent.. 
                Kooboo.Mail.Transport.Incoming.SaveSent(fromaddress, msginfo, messagebody);

                Kooboo.Mail.Transport.Incoming.Receive(fromaddress, rcpttos, messagebody, msginfo);

                // draft message id. 
                var messageid = call.GetValue<int>("messageId");
                if (messageid > 0)
                {
                    var usermaildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);
                    usermaildb.Messages.Delete(messageid);
                } 

                Kooboo.Data.Authorization.QuotaControl.AddSendEmailCount(call.Context.User.CurrentOrgId, rcpttos.Count);
                 
            } 

        }

        public void Moves(ApiCall call)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            var idsJson = call.GetValue("ids");
            var ids = Lib.Helper.JsonHelper.Deserialize<List<int>>(idsJson);

            var folder = call.Context.Request.GetValue("folder");

            var newfolder = new Folder(folder);

            foreach (var id in ids)
            {
                var msg = maildb.Messages.Get(id);
                if (msg != null)
                {
                    maildb.Messages.Move(msg, newfolder);
                }
            }
        }

        public void Deletes(ApiCall call)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            var idsJson = call.GetValue("ids");
            var ids = Lib.Helper.JsonHelper.Deserialize<List<int>>(idsJson);

            foreach (var id in ids)
            {
                maildb.Messages.Delete(id);
            }
        }

        public ComposeViewModel Forward(ApiCall call)
        {
            int messageId = call.GetValue<int>("sourceId");
            return Kooboo.Mail.Multipart.ReferenceComposer.ComposeForward(messageId, call.Context);
        }

        public ComposeViewModel Reply(ApiCall call)
        {
            int messageId = call.GetValue<int>("sourceId");
            return Kooboo.Mail.Multipart.ReferenceComposer.ComposeReply(messageId, call.Context);
        }

        public void MarkReads(ApiCall call)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            var idsJson = call.GetValue("ids");
            var ids = Lib.Helper.JsonHelper.Deserialize<List<int>>(idsJson);

            var value = call.GetValue<bool>("value");

            foreach (var id in ids)
            {
                var msg = maildb.Messages.Get(id);
                if (msg != null)
                {
                    maildb.Messages.MarkAsRead(msg.Id, value);
                }
            }
        }

    }
}
