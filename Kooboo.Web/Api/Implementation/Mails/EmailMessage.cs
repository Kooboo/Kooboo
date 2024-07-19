//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Mail;
using Kooboo.Mail.Utility;
using Kooboo.Mail.ViewModel;
using Kooboo.Web.Api.Implementation.Mails.ViewModel;
using MimeKit;

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

        public bool RequireSite => false;

        public bool RequireUser => true;

        public List<MessageViewModel> More(ApiCall call)
        {
            string keyword = call.GetValue("keyword");
            var add = call.GetValue("address");

            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();

                if (add != null)
                {
                    dic.Add("address", add);
                }

                if (keyword != null)
                {
                    dic.Add("keyword", keyword);
                }

                dic.Add("folder", call.GetValue("folder"));
                dic.Add("messageId", call.GetValue("messageId"));

                return EmailForwardManager.Get<List<MessageViewModel>>(this.ModelName, nameof(EmailMessageApi.More), call.Context.User, dic);
            }

            var maildb = Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            if (add != null)
            {
                add = add.Replace(" ", "+");
            }

            int addressid = EmailAddress.ToId(add);

            string folderName = call.GetValue("folder");
            var folderid = Folder.ToId(folderName);

            var messageid = call.GetValue<int>("messageId");

            if (messageid <= 0)
            {
                messageid = int.MaxValue;
            }

            SqlWhere<Message> query = maildb.Message2.Query.Where(o => o.MsgId < messageid);

            if (folderName != null && folderName.ToLower() != "searchemail" && folderName.ToLower() != "allfolder")
            {
                query.Where(o => o.FolderId == folderid);
            }

            if (folderid == 0)
            {
                var ids = Folder.ReservedFolder.Keys.ToList();
                query.WhereNotIn(nameof(Message.FolderId), ids);
            }

            if (addressid != 0)
            {
                query = query.Where(o => o.AddressId == addressid);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.AND });

                string condition = " ([" + nameof(Message.From) + "] like '%" + keyword + "%' OR " + nameof(Message.Subject) + " like '%" + keyword + "%')";

                query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.ConditionText, Sql = condition });

            }

            query.OrderByDescending(o => o.MsgId);

            var list = query.Take(PageSize).ToList();

            list = maildb.Message2.ToFullMessage(list);

            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(maildb.OrganizationId);

            return list.Select(o => MessageViewModel.FromMessage(o, maildb, orgdb)).ToList();
        }


        private static SqlWhere<Message> GetMessages(MailDb maildb, ApiCall call)
        {
            var messageid = call.GetValue<int>("messageId");

            if (messageid <= 0)
            {
                messageid = int.MaxValue;
            }

            var folder = call.GetValue("folder");
            if (string.IsNullOrEmpty(folder))
            {
                var ids = Folder.ReservedFolder.Keys.ToList();
                return maildb
                    .Message2
                    .Query
                    .Where(it => it.MsgId < messageid)
                    .AddOperator(SqlWhere<Message>.OperatorType.AND)
                    .WhereNotIn(nameof(Message.FolderId), ids);
            }
            else if (folder.Equals("searchEmail", StringComparison.OrdinalIgnoreCase))
            {
                return maildb.Message2.Query.Where(o => o.MsgId < messageid);
            }
            folder = System.Web.HttpUtility.UrlDecode(folder);
            var folderid = Folder.ToId(folder);
            return maildb.Message2.Query.Where(o => o.FolderId == folderid && o.MsgId < messageid);
        }

        public long LatestMsgId(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                return EmailForwardManager.Get<long>(this.ModelName, nameof(EmailMessageApi.LatestMsgId), call.Context.User);
            }
            var maildb = Mail.Factory.DBFactory.UserMailDb(call.Context.User);
            return maildb.Message2.LastKey;
        }

        public ContentViewModel Content(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("messageId", call.GetValue("messageId"));
                return EmailForwardManager.Get<ContentViewModel>(this.ModelName, nameof(EmailMessageApi.Content), call.Context.User, dic);

            }
            int messageid = call.GetValue<int>("messageId");

            if (messageid <= 0)
            {
                return new ContentViewModel();
            }
            else
            {
                var maildb = Mail.Factory.DBFactory.UserMailDb(call.Context.User);
                maildb.Message2.MarkAsRead(messageid);
                return Mail.Utility.MessageUtility.GetContentViewModel(call.Context.User, messageid);
            }
        }

        public string ViewSource(string messageId, ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("messageId", call.GetValue("messageId"));
                return EmailForwardManager.Get<string>(this.ModelName, nameof(EmailMessageApi.ViewSource), call.Context.User, dic);

            }
            int messageid = call.GetValue<int>("messageId");

            if (messageid <= 0)
            {
                return null;
            }
            else
            {
                var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

                var msg = maildb.Message2.Get(messageid);

                if (msg != null)
                {
                    return msg.Body;
                }
            }
            return null;
        }


        [Kooboo.Attributes.RequireModel(typeof(ComposeViewModel))]
        public void Send(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("messageId", call.GetValue("messageId"));

                var json = Kooboo.Lib.Helper.JsonHelper.Serialize(call.Context.Request.Model);

                EmailForwardManager.Post<string>(this.ModelName, nameof(EmailMessageApi.Send), call.Context.User, json, dic);
                return;
            }

            var user = call.Context.User;

            // blacklist.
            if (Kooboo.Mail.SecurityControl.BlackList.IsUserBanned(user))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("you have been blacklisted", call.Context));
            }


            var model = call.Context.Request.Model as Mail.ViewModel.ComposeViewModel;
            /// model.Html = Kooboo.Sites.Service.HTMLService.SplitBase64Lines(model.Html); 
            string msgid = Kooboo.Mail.Utility.SmtpUtility.GenerateMessageId(model.FromAddress);

            if (model.MessageId.HasValue && model.MessageId.Value > 0)
            {
                model.Html = EmailHelper.ReplaceOldMsg(model.Html, model.MessageId.Value);
            }


            string messageBody = Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, user, false, msgid);

            string msgSaveSent = Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, user, true, msgid);

            List<string> rcpttos = new List<string>(model.To);
            rcpttos.AddRange(model.Cc);
            rcpttos.AddRange(model.Bcc);

            var msginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(msgSaveSent);

            string fromaddress = Mail.Utility.AddressUtility.GetAddress(msginfo.From);

            if (rcpttos.Count > 0)
            {
                var cansend = Kooboo.Data.Infrastructure.InfraManager.Test(call.Context.User.CurrentOrgId, Data.Infrastructure.InfraType.Email, rcpttos.Count);

                if (!cansend)
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("you have no enough credit to send emails, please contact system administrators", call.Context));
                }

                // save sent.. 
                Kooboo.Mail.Transport.Incoming.SaveSent(fromaddress, msginfo, msgSaveSent, user);

                msginfo.Bcc = null;
                msginfo.Read = false;

                _ = Kooboo.Mail.Transport.Incoming.Receive(fromaddress, rcpttos, messageBody, msginfo);

                var usermaildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

                // draft message id. 
                var messageid = call.GetValue<int>("messageId");
                if (messageid > 0)
                {
                    usermaildb.Message2.Delete(messageid);
                }

                usermaildb.AddBook.AddList(rcpttos);

                Kooboo.Data.Infrastructure.InfraManager.Add(call.Context.User.CurrentOrgId, Data.Infrastructure.InfraType.Email, rcpttos.Count, string.Join(",", rcpttos));

            }

        }


        public void Moves(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>
                {
                    { "ids", call.GetValue("ids") },
                    { "folder", call.Context.Request.GetValue("folder") }
                };
                EmailForwardManager.Get<bool>(this.ModelName, nameof(EmailMessageApi.Moves), call.Context.User, dic);
                return;
            }

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            var idsJson = call.GetValue("ids");
            var ids = Lib.Helper.JsonHelper.Deserialize<List<int>>(idsJson);

            var folder = call.Context.Request.GetValue("folder");

            var newfolder = new Folder(folder);

            var spamFolder = new Folder(Folder.Spam);


            foreach (var id in ids)
            {
                var msg = maildb.Message2.Get(id);
                if (msg != null)
                {
                    if (msg.FolderId == spamFolder.Id && newfolder.Id != spamFolder.Id)
                    {
                        // move out of spam, should add to contact user.

                        maildb.AddBook.Add(msg.From);
                    }

                    maildb.Message2.Move(msg, newfolder);

                }
            }
        }

        public bool Deletes(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>
                {
                    { "ids", call.GetValue("ids") }
                };
                return EmailForwardManager.Post<bool>(this.ModelName, nameof(EmailMessageApi.Deletes), call.Context.User, dic);
            }

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            var idsJson = call.GetValue("ids");
            var ids = Lib.Helper.JsonHelper.Deserialize<List<int>>(idsJson);

            foreach (var id in ids)
            {
                maildb.Message2.Delete(id);
            }

            return true;
        }

        public ComposeViewModel Forward(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("sourceId", call.GetValue("sourceId"));
                dic.Add("timeZoneId", call.GetValue("timeZoneId"));
                return EmailForwardManager.Get<ComposeViewModel>(this.ModelName, nameof(EmailMessageApi.Forward), call.Context.User, dic);
            }
            int messageId = call.GetValue<int>("sourceId");
            var timeZoneId = call.GetValue<string>("timeZoneId");

            return Kooboo.Mail.Multipart.ReferenceComposer.ComposeForward(messageId, call.Context, timeZoneId);
        }

        public ComposeViewModel Reply(ApiCall call)
        {
            var messageId = call.GetValue<int>("messageId");
            if (messageId == 0)
            {
                messageId = call.GetValue<int>("sourceId");
            }

            bool ReplyAll = false;
            var type = call.GetValue("type");
            if (type != null && type.ToLower() == "all")
            {
                ReplyAll = true;
            }
            var timeZoneId = call.GetValue("timeZoneId");

            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("sourceId", messageId.ToString());
                dic.Add("type", type);
                dic.Add("timeZoneId", timeZoneId);
                return EmailForwardManager.Get<ComposeViewModel>(this.ModelName, nameof(EmailMessageApi.Reply), call.Context.User, dic);
            }


            return Kooboo.Mail.Multipart.ReferenceComposer.ComposeReply(messageId, call.Context, ReplyAll, timeZoneId);
        }

        public ComposeViewModel ReEdit(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>
                {
                    { "messageId", call.GetValue("messageId") },
                    { "sourceId", call.GetValue("sourceId") }
                };
                return EmailForwardManager.Get<ComposeViewModel>(this.ModelName, nameof(EmailMessageApi.ReEdit), call.Context.User, dic);
            }

            var messageId = call.GetValue<int>("messageId");
            if (messageId == 0)
            {
                messageId = call.GetValue<int>("sourceId");
            }
            if (messageId <= 0)
            {
                return new ComposeViewModel();
            }

            return Kooboo.Mail.Multipart.ReferenceComposer.ComposeReEdit(call.Context.User, messageId, call.Context);
        }

        public void MarkReads(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("ids", call.GetValue("ids"));
                dic.Add("value", call.GetValue("value"));
                EmailForwardManager.Post<bool>(this.ModelName, nameof(EmailMessageApi.MarkReads), call.Context.User, dic);
                return;
            }

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            var idsJson = call.GetValue("ids");
            var ids = Lib.Helper.JsonHelper.Deserialize<List<int>>(idsJson);

            var value = call.GetValue<bool>("value");

            foreach (var id in ids)
            {
                var msg = maildb.Message2.Get(id);
                if (msg != null)
                {
                    maildb.Message2.MarkAsRead(msg.MsgId, value);
                }
            }
        }

        public AdvanceSearchViewModel AdvancedSearch(ApiCall call, MailAdvancedSearch model)
        {
            var searchModel = call.GetValue("model");
            var messageId = call.GetValue<int>("messageId");
            if (messageId <= 0)
            {
                messageId = int.MaxValue;
            }
            var maildb = Mail.Factory.DBFactory.UserMailDb(call.Context.User);
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("model", call.GetValue("model"));
                dic.Add("messageId", call.GetValue("messageId"));
                return EmailForwardManager.Post<AdvanceSearchViewModel>(this.ModelName, nameof(EmailMessageApi.More), call.Context.User, dic);
            }

            var list = AdvancedSearchByMultipleCondition(maildb, call, model);
            list = list.OrderBy(o => o.MsgId).ToList();
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(maildb.OrganizationId);
            var viewModel = list.OrderByDescending(o => o.MsgId).Where(o => o.MsgId < messageId).Select(o => MessageViewModel.FromMessage(o, maildb, orgdb)).Take(PageSize).ToList();
            var advanceSearchViewModel = new AdvanceSearchViewModel()
            {
                Count = list.Count,
                Data = viewModel
            };
            return advanceSearchViewModel;
        }

        public BinaryResponse ExportEmlFile(ApiCall call)
        {
            var messageId = Convert.ToInt32(call.Command.Value);
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);
            var subject = call.GetValue<string>("subject");
            if (string.IsNullOrEmpty(subject))
                subject = "No subject";
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dict = new Dictionary<string, string>()
                {
                    { "subject", System.Web.HttpUtility.UrlEncode(subject) }
                };
                var method = nameof(ExportEmlFile) + "/" + messageId;
                var bytes = EmailForwardManager.Post(this.ModelName, method, call.Context.User, null, dict);
                var response = new BinaryResponse();
                response.ContentType = "application/octet-stream";
                response.Headers.Add("Content-Disposition", $"filename={System.Web.HttpUtility.UrlEncode(subject)}.eml");
                response.BinaryBytes = bytes;
                return response;
            }

            string filename = System.IO.Path.Combine(maildb.MsgFolder, messageId.ToString() + ".eml");
            if (System.IO.File.Exists(filename))
            {
                var response = new BinaryResponse();
                response.ContentType = "application/octet-stream";
                response.Headers.Add("Content-Disposition", $"filename={System.Web.HttpUtility.UrlEncode(subject)}.eml");
                response.BinaryBytes = System.IO.File.ReadAllBytes(filename);
                return response;
            }

            return null;
        }

        private List<Message> AdvancedSearchByMultipleCondition(MailDb maildb, ApiCall call, MailAdvancedSearch model)
        {
            var messageid = call.GetValue<int>("messageId");

            if (messageid <= 0)
            {
                messageid = int.MaxValue;
            }
            var query = maildb
                    .Message2
                    .Query;
            query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.ConditionText, Sql = "1=1" });


            if (!string.IsNullOrEmpty(model.SearchFolder) && !model.SearchFolder.Equals("AllEmails", StringComparison.OrdinalIgnoreCase))
            {
                var folderId = Folder.ToId(model.SearchFolder);
                string condition = $"[FolderId] == {folderId}";
                query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.AND });
                query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.ConditionText, Sql = condition });
            }

            if (!string.IsNullOrWhiteSpace(model.From))
            {
                string condition = $"[{nameof(model.From)}] like '%{model.From}%'";
                query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.AND });
                query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.ConditionText, Sql = condition });
            }

            if (!string.IsNullOrWhiteSpace(model.To))
            {
                string condition = $"([{nameof(model.To)}] like '%{model.To}%' OR [Cc] like '%{model.To}%' OR [Bcc] like '%{model.To}%')";
                query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.AND });
                query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.ConditionText, Sql = condition });
            }

            if (model.ReadOrUnRead >= 0)
            {
                query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.AND });
                string condition = string.Empty;
                switch (model.ReadOrUnRead)
                {
                    case 0:
                        condition = $"[Read] == 0";
                        query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.ConditionText, Sql = condition });
                        break;
                    case 1:
                        condition = $"[Read] == 1";
                        query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.ConditionText, Sql = condition });
                        break;
                }
            }

            if (model.DateType != DateType.NoLimit)
            {
                query.WhereConditions.Add(new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.AND });
                var timeQuery = GenerateTimeQuery(model.DateType, model.StartDate, model.EndDate);
                query.WhereConditions.Add(timeQuery);
            }

            query.OrderByDescending(o => o.MsgId);

            var list = query.Take(int.MaxValue).ToList();
            list = maildb.Message2.ToFullMessage(list);


            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                list = SearchByKeywordAndPosition(list, model.Keyword, model.Position);
            }

            return list;
        }

        private List<Message> SearchByKeywordAndPosition(List<Message> messages, string keyword, string position)
        {
            keyword = keyword.Trim();
            var mimeMessages = new List<MimeMessage>();
            var result = new List<Message>();

            messages.ForEach(v =>
            {
                var mimeMessage = MailKitUtility.LoadMessage(v.Body);
                mimeMessages.Add(mimeMessage);
            });

            switch (position)
            {
                case "subject":
                    for (int i = 0; i <= messages.Count - 1; i++)
                    {
                        if (!string.IsNullOrEmpty(mimeMessages[i].Subject) && mimeMessages[i].Subject.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            result.Add(messages[i]);
                    }
                    break;
                case "emailBody":
                    for (int i = 0; i <= messages.Count - 1; i++)
                    {
                        if (!string.IsNullOrEmpty(mimeMessages[i].TextBody) && mimeMessages[i].TextBody.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            result.Add(messages[i]);
                    }
                    break;
                case "attachments":
                    for (int i = 0; i <= messages.Count - 1; i++)
                    {
                        if (messages[i].Attachments.Any())
                        {
                            foreach (var item in messages[i].Attachments)
                            {
                                if (item is not null && !string.IsNullOrEmpty(item.FileName) && item.FileName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                                    result.Add(messages[i]);
                            }
                        }
                    }
                    break;
                default:
                    foreach (var item in messages)
                    {
                        if (!string.IsNullOrEmpty(item.Body))
                        {
                            var mimeMessage = MailKitUtility.LoadMessage(item.Body);
                            if (mimeMessage.From.FirstOrDefault() is not null && mimeMessage.From.First().ToString().Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            {
                                result.Add(item);
                                continue;
                            }
                            if (!string.IsNullOrEmpty(mimeMessage.TextBody) && mimeMessage.TextBody.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            {
                                result.Add(item);
                                continue;
                            }
                            if (!string.IsNullOrEmpty(mimeMessage.Subject) && mimeMessage.Subject.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            {
                                result.Add(item);
                                continue;
                            }
                            if (item.Attachments.Any())
                            {
                                foreach (var attachment in item.Attachments)
                                {
                                    if (attachment is not null && !string.IsNullOrEmpty(attachment.FileName) && attachment.FileName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                                    {
                                        result.Add(item);
                                        continue;
                                    }
                                }
                            }
                            if (mimeMessage.To is not null)
                            {
                                var rcptToList = mimeMessage.To;
                                rcptToList.AddRange(mimeMessage.Cc);
                                rcptToList.AddRange(mimeMessage.Bcc);
                                foreach (var rcptTo in rcptToList)
                                {
                                    if (rcptTo.ToString().Contains(keyword))
                                    {
                                        result.Add(item);
                                        break;
                                    }
                                }

                                continue;
                            }
                        }
                    }
                    break;
            }
            mimeMessages.Clear();
            return result;
        }

        private SqlWhere<Message>.WhereTerm GenerateTimeQuery(DateType dateType, string startDate, string endDate)
        {
            string condition = string.Empty;
            switch (dateType)
            {
                case DateType.OneDay:
                    condition = $"[Date] >= '{DateTime.UtcNow.Date:yyyy-MM-dd 00:00:00}'  AND [Date] < '{DateTime.UtcNow.AddDays(1).Date:yyyy-MM-dd 00:00:00}' ";
                    break;
                case DateType.ThreeDay:
                    condition = $"[Date] >= '{DateTime.UtcNow.AddDays(-2).Date:yyyy-MM-dd 00:00:00}'  AND [Date] < '{DateTime.UtcNow.AddDays(1).Date:yyyy-MM-dd 00:00:00}' ";
                    break;
                case DateType.OneWeek:
                    condition = $"[Date] >= '{DateTime.UtcNow.AddDays(-6).Date:yyyy-MM-dd 00:00:00}'  AND [Date] < '{DateTime.UtcNow.AddDays(1).Date:yyyy-MM-dd 00:00:00}' ";
                    break;
                case DateType.TwoWeek:
                    condition = $"[Date] >= '{DateTime.UtcNow.Date.AddDays(-13).Date:yyyy-MM-dd 00:00:00}'  AND [Date] < '{DateTime.UtcNow.AddDays(1).Date:yyyy-MM-dd 00:00:00}' ";
                    break;
                case DateType.OneMonth:
                    condition = $"[Date] >= '{DateTime.UtcNow.Date.AddMonths(-1).AddDays(2).Date:yyyy-MM-dd 00:00:00}'  AND [Date] < '{DateTime.UtcNow.AddDays(1).Date:yyyy-MM-dd 00:00:00}' ";
                    break;
                default:
                    condition = $"[Date] >= '{startDate}'  AND [Date] < '{endDate}' ";
                    break;
            }

            return new SqlWhere<Message>.WhereTerm() { Type = SqlWhere<Message>.WhereTerm.TermType.ConditionText, Sql = condition };
        }

    }
}
