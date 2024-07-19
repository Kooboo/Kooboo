//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.Lib.Security;
using Kooboo.Mail;
using Kooboo.Mail.Factory;
using Kooboo.Mail.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailAddressApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "EmailAddress";
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

        public List<Data.Models.Domain> Domains(ApiCall apiCall)
        {
            List<Data.Models.Domain> DomainList;

            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                DomainList = EmailForwardManager.Get<List<Data.Models.Domain>>(this.ModelName, nameof(EmailAddressApi.Domains), apiCall.Context.User, null);
            }
            else
            {
                DomainList = Data.GlobalDb.Domains.ListForEmail(apiCall.Context.User);
            }
            return RemoveUnKoobooDns(DomainList);
        }

        private List<Data.Models.Domain> RemoveUnKoobooDns(List<Data.Models.Domain> input)
        {

            if (!Data.AppSettings.IsOnlineServer)
            {
                return input;
            }

            List<Data.Models.Domain> result = new List<Domain>();
            foreach (var item in input)
            {
                if (string.IsNullOrWhiteSpace(item.NameServer))
                {
                    var servers = Kooboo.Lib.Whois.Service.GetDnsServer(item.DomainName);
                    if (servers != null && servers.Any())
                    {
                        item.NameServer = string.Join(',', servers);
                    }
                }
                if (Kooboo.Data.Helper.DomainHelper.hasKoobooDns(item.NameServer))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public List<AddressItemModel> List(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                Dictionary<string, string> folderPara = null;
                string requestFolder = call.GetValue("folder");
                if (!string.IsNullOrEmpty(requestFolder))
                {
                    folderPara = new Dictionary<string, string>();
                    folderPara.Add("folder", requestFolder);
                }
                return EmailForwardManager.Get<List<AddressItemModel>>(this.ModelName, nameof(EmailAddressApi.List), call.Context.User, folderPara);
            }


            var user = call.Context.User;
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(user.CurrentOrgId);

            var addlist = orgdb.Email.ByUser(user.Id);   // orgdb.EmailAddress.Query().Where(o => o.UserId == user.Id).SelectAll();

            List<AddressItemModel> result = new();

            foreach (var item in addlist)
            {
                var model = AddressItemModel.FromAddress(item);
                result.Add(model);
            }

            var maildb = Mail.Factory.DBFactory.UserMailDb(user.Id, user.CurrentOrgId);

            var folder = call.GetValue("folder");
            bool IsInbox = true;
            if (folder != null && folder.ToLower() == "sent")
            {
                IsInbox = false;
            }

            var unreads = Kooboo.Mail.Utility.FolderUtility.AddressUnread(maildb, IsInbox);

            result.OrderBy(o => o.Address).ToList();

            foreach (var item in result)
            {
                var find = unreads.Find(o => o.AddressId == item.Id);
                if (find != null)
                {
                    item.UnRead = find.UnRead;
                }
            }
            return result.OrderBy(o => o.Address).ToList();
        }

        public List<AddressItemModel> FromList(long MessageId, ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("MessageId", MessageId.ToString());
                return EmailForwardManager.Get<List<AddressItemModel>>(this.ModelName, nameof(EmailAddressApi.FromList), apiCall.Context.User, para);
            }

            var user = apiCall.Context.User;
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(user.CurrentOrgId);

            var addlist = orgdb.Email.ByUser(user.Id);   // orgdb.EmailAddress.Query().Where(o => o.UserId == user.Id).SelectAll();

            List<AddressItemModel> result = new();

            List<EmailAddress> wildcardList = new List<EmailAddress>();

            foreach (var item in addlist)
            {

                if (item.AddressType == EmailAddressType.Wildcard)
                {
                    wildcardList.Add(item);
                }
                else
                {
                    var model = AddressItemModel.FromAddress(item);
                    result.Add(model);
                }
            }

            var maildb = Mail.Factory.DBFactory.UserMailDb(user.Id, user.CurrentOrgId);

            // append the message id to address, when it is a reply or forward. 
            if (MessageId != 0 && wildcardList.Any())
            {
                // this is a reply or forward.... 
                var msg = maildb.Message2.Get(MessageId);
                if (msg != null && msg.FolderId != Folder.ToId(Folder.Drafts))
                {
                    var toList = Kooboo.Mail.Utility.MessageUtility.GetAddressModels(msg.RcptTo);

                    var domains = Data.GlobalDb.Domains.ListForEmail(apiCall.Context.User);

                    foreach (var wild in wildcardList)
                    {

                        foreach (var item in toList)
                        {

                            if (Kooboo.Mail.Utility.AddressUtility.WildcardMatch(item.Address, wild.Address))
                            {
                                var model = AddressItemModel.FromAddress(wild);
                                model.Address = item.Address;
                                if (model.DisplayName == null)
                                {
                                    model.DisplayName = Kooboo.Mail.Utility.AddressUtility.GetDisplayName(model.Address);
                                }
                                else
                                {
                                    model.DisplayName = model.DisplayName.Replace(wild.Address, item.Address);
                                }
                                result.Add(model);


                            }
                        }
                    }
                }
            }

            return result.OrderBy(o => o.Address).ToList();

        }

        [Kooboo.Attributes.RequireModel(typeof(AddressModel))]
        public bool IsUniqueName(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var json = Kooboo.Lib.Helper.JsonHelper.Serialize(apiCall.Context.Request.Model);
                return EmailForwardManager.Post<bool>(this.ModelName, nameof(EmailAddressApi.IsUniqueName), apiCall.Context.User, json, null);
            }
            var model = apiCall.Context.Request.Model as AddressModel;
            var orgdb = Mail.Factory.DBFactory.OrgDb(apiCall.Context.User.CurrentOrgId);
            return orgdb.Email.Get(model.ToEmail()) == null;
        }

        [Kooboo.Attributes.RequireModel(typeof(AddressModel))]
        public object Post(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var json = Kooboo.Lib.Helper.JsonHelper.Serialize(apiCall.Context.Request.Model);
                return EmailForwardManager.Post<object>(this.ModelName, nameof(EmailAddressApi.Post), apiCall.Context.User, json, null);
            }

            var model = apiCall.Context.Request.Model as AddressModel;
            var user = apiCall.Context.User;
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(user.CurrentOrgId);
            var address = new EmailAddress
            {
                Address = model.ToEmail().ToLower(),
                ForwardAddress = model.ForwardAddress,
                AddressType = model.AddressType,
                UserId = user.Id,
                Name = model.Name
            };

            if (!Kooboo.Mail.Utility.AddressUtility.IsValidEmailAddress(address.Address))
            {
                return new Kooboo.Api.ApiResponse.JsonResponse
                {
                    Success = false,
                    Messages = new List<string> { Kooboo.Data.Language.Hardcoded.GetValue("Invalid address format", apiCall.Context) }
                };
            }

            if (address.AddressType == EmailAddressType.Forward)
            {
                address.ForwardAddress = address.ForwardAddress.ToLower();
                if (address.Address == address.ForwardAddress)
                {
                    return new Kooboo.Api.ApiResponse.JsonResponse
                    {
                        Success = false,
                        Messages = new List<string> { "Forward to address should not be same as forward from" }
                    };
                }
            }

            orgdb.Email.AddOrUpdate(address);

            return AddressItemModel.FromAddress(address);
        }

        public bool UpdateName(int Id, ApiCall call)
        {
            var name = call.GetValue("name");

            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("Id", Id.ToString());
                if (!string.IsNullOrEmpty(name))
                {
                    dic.Add("name", name);
                }

                return EmailForwardManager.Get<bool>(this.ModelName, nameof(EmailAddressApi.UpdateName), call.Context.User, dic);
            }

            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);
            return orgdb.Email.UpdateName(Id, name);

        }

        public bool SetDefaultSender(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("address", call.GetValue("address"));

                return EmailForwardManager.Get<bool>(this.ModelName, nameof(EmailAddressApi.SetDefaultSender), call.Context.User, dic);
            }

            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);
            string address = call.GetValue("address");
            var findOriginDefaultSender = orgdb.Email.GetDefaultSender(call.Context.User);
            var mail = orgdb.Email.Find(address);

            if (findOriginDefaultSender != null)
            {
                findOriginDefaultSender.IsDefault = false;
                orgdb.Email.Update(findOriginDefaultSender);
            }
            mail.IsDefault = true;
            orgdb.Email.Update(mail);

            return true;
        }

        public IEnumerable<string> AddressComplete(ApiCall call, string part)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>
                {
                    { "part", part }
                };
                return EmailForwardManager.Post<List<string>>(this.ModelName, nameof(EmailAddressApi.AddressComplete), call.Context.User, dic);
            }
            else
            {
                var usermaildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);
                var result = usermaildb.AddBook.Contains(part);
                return result != null ? result.OrderBy(o => o) : new List<string>();
            }
        }

        public List<AddressBook> ContactList(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                Dictionary<string, string> folderPara = null;
                string requestFolder = call.GetValue("folder");
                if (!string.IsNullOrEmpty(requestFolder))
                {
                    folderPara = new Dictionary<string, string>();
                    folderPara.Add("folder", requestFolder);
                }
                return EmailForwardManager.Get<List<AddressBook>>(this.ModelName, nameof(EmailAddressApi.ContactList), call.Context.User, folderPara);
            }
            var maildb = DBFactory.UserMailDb(call.Context.User);

            return maildb.AddBook.All();
        }

        public string ContactPost(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("name", call.GetValue("name"));
                dic.Add("address", call.GetValue("address"));
                return EmailForwardManager.Post<string>(this.ModelName, nameof(EmailAddressApi.ContactPost), call.Context.User, dic);
            }
            var maildb = DBFactory.UserMailDb(call.Context.User);

            string name = call.GetValue("name");
            string mailAddress = call.GetValue("address");
            if (!string.IsNullOrEmpty(name))
            {
                mailAddress = $"{name} <{mailAddress}>";
            }
            if (!string.IsNullOrEmpty(mailAddress))
            {
                maildb.AddBook.Add(mailAddress);
                return mailAddress;
            }
            return null;
        }

        public void ContactUpdate(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var json = call.Context.Request.Body;
                EmailForwardManager.Post<string>(this.ModelName, nameof(EmailAddressApi.ContactUpdate), call.Context.User, json);
                return;
            }

            var maildb = DBFactory.UserMailDb(call.Context.User);
            AddressBook info = Kooboo.Lib.Helper.JsonHelper.Deserialize<AddressBook>(call.Context.Request.Body);
            info.FullAddress = $"\"{info.Name}\" <{info.Address}>";
            maildb.AddBook.AddOrUpdate(info);
        }

        public void ContactDelete(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("id", call.GetValue("id"));
                EmailForwardManager.Get<bool>(this.ModelName, nameof(EmailAddressApi.ContactDelete), call.Context.User, dic);
                return;
            }

            int id = call.GetValue<int>("id");
            var maildb = DBFactory.UserMailDb(call.Context.User);
            maildb.AddBook.Delete(id);
        }

        public void ContactDeletes(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("ids", call.GetValue("ids"));
                EmailForwardManager.Get<bool>(this.ModelName, nameof(EmailAddressApi.ContactDeletes), call.Context.User, dic);
                return;
            }

            string idsJson = call.GetValue("ids");
            if (string.IsNullOrEmpty(idsJson))
            {
                idsJson = call.Context.Request.Body;
            }

            var maildb = DBFactory.UserMailDb(call.Context.User);
            var ids = JsonConvert.DeserializeObject<int[]>(idsJson);

            foreach (var id in ids)
            {
                maildb.AddBook.Delete(id);
            }
        }

        public void Deletes(ApiCall apiCall)
        {
            string idsJson = apiCall.GetValue("ids");
            if (string.IsNullOrEmpty(idsJson))
            {
                idsJson = apiCall.Context.Request.Body;
            }

            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("ids", idsJson);
                EmailForwardManager.Get<string>(this.ModelName, nameof(EmailAddressApi.Deletes), apiCall.Context.User, dic);
                return;
            }

            var ids = JsonConvert.DeserializeObject<int[]>(idsJson);
            var maildb = DBFactory.UserMailDb(apiCall.Context.User);
            var runningJobs = maildb.MailMigrationJob.GetActiveJobsByAddress(ids);
            if (runningJobs.Any())
            {
                throw new Exception("One or more associated jobs are running, please stop them first");
            }

            var orgdb = DBFactory.OrgDb(apiCall.Context.User.CurrentOrgId);
            foreach (var id in ids)
            {
                orgdb.Email.Delete(id);
            }
        }

        public List<string> MemberList(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("addressId", call.GetValue("addressId"));
                return EmailForwardManager.Get<List<string>>(this.ModelName, nameof(EmailAddressApi.MemberList), call.Context.User, dic);
            }

            int addressid = call.GetValue<int>("addressId");
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            return orgdb.Email.GetMembers(addressid).OrderBy(o => o).ToList();
        }

        [Kooboo.Attributes.RequireModel(typeof(ListMemberModel))]
        public object MemberPost(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("addressId", call.GetValue("addressId"));
                var json = Kooboo.Lib.Helper.JsonHelper.Serialize(call.Context.Request.Model);
                return EmailForwardManager.Post<object>(this.ModelName, nameof(EmailAddressApi.MemberPost), call.Context.User, json, dic);
            }

            int addressid = call.GetValue<int>("addressId");
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var member = call.Context.Request.Model as ListMemberModel;
            member.MemberAddress = member.MemberAddress.ToLower();

            var address = orgdb.Email.Get(addressid);
            if (Lib.Helper.StringHelper.IsSameValue(member.MemberAddress, address.Address))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Member address should not be same as group address", call.Context));
            }

            orgdb.Email.AddMember(addressid, member.MemberAddress);

            return member;
        }

        public void MemberDelete(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("addressId", call.GetValue("addressId"));
                dic.Add("memberAddress", call.GetValue("memberAddress"));
                EmailForwardManager.Get<bool>(this.ModelName, nameof(EmailAddressApi.MemberDelete), call.Context.User, dic);
                return;
            }

            int addressid = call.GetValue<int>("addressId");
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var memberAddress = call.GetValue("memberAddress");
            orgdb.Email.DeleteMember(addressid, memberAddress);
        }

        public void UpdateForward(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("id", call.GetValue("id"));
                dic.Add("forwardAddress", call.GetValue("forwardAddress"));
                EmailForwardManager.Get<bool>(this.ModelName, nameof(EmailAddressApi.UpdateForward), call.Context.User, dic);
                return;
            }

            int id = call.GetValue<int>("id");
            string add = call.GetValue("forwardAddress");

            if (id != 0 && !string.IsNullOrEmpty(add))
            {
                var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);
                var email = orgdb.Email.Get(id);
                if (email != null && email.AddressType == EmailAddressType.Forward)
                {
                    email.ForwardAddress = add;
                    orgdb.Email.AddOrUpdate(email);
                }
            }

        }

        public void UpdateSignature(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("addressId", call.GetValue("addressId"));
                dic.Add("signature", call.GetValue("signature"));
                EmailForwardManager.Post<bool>(this.ModelName, nameof(EmailAddressApi.UpdateSignature), call.Context.User, dic);
                return;
            }

            int id = call.GetValue<int>("addressId");
            string signature = call.GetValue("signature");

            if (id != 0)
            {
                var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);
                var email = orgdb.Email.Get(id);
                if (email != null)
                {
                    email.Signature = string.IsNullOrEmpty(signature) ? string.Empty : signature;
                    orgdb.Email.AddOrUpdate(email);
                }
            }
        }


        public string GetSignature(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("addressId", call.GetValue("addressId"));
                return EmailForwardManager.Get<string>(this.ModelName, nameof(EmailAddressApi.GetSignature), call.Context.User, dic) ?? string.Empty;
            }

            int id = call.GetValue<int>("addressId");
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);
            var email = orgdb.Email.Get(id);
            return email?.Signature ?? string.Empty;
        }

        public string SetAuthorizationCode(ApiCall call)
        {
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("address", call.GetValue("address"));
                return EmailForwardManager.Post<string>(this.ModelName, nameof(EmailAddressApi.SetAuthorizationCode), call.Context.User, dic);
            }

            var address = call.GetValue<string>("address");
            var email = orgdb.Email.Find(address);
            if (string.IsNullOrEmpty(address))
            {
                return string.Empty;
            }

            var authorizationCode = ShortGuid.Encode(Guid.NewGuid()).Replace("_", string.Empty);
            email.AuthorizationCode = authorizationCode;
            orgdb.Email.AddOrUpdate(email);
            return authorizationCode;
        }

        public class ListMemberModel
        {
            public string MemberAddress { get; set; }
        }

        public class AddressModel
        {
            public string Local { get; set; }

            public string Domain { get; set; }

            public string Name { get; set; }

            public string ForwardAddress { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public EmailAddressType AddressType { get; set; }

            public string ToEmail()
            {
                if (this.AddressType == EmailAddressType.Wildcard)
                {
                    if (Local == null || !Local.Contains("*"))
                    {
                        Local = "*" + Local;
                    }
                }
                return Local + "@" + Domain;
            }
        }

        public class AddressItemModel
        {
            //public EmailAddress AddressOld { get; set; } 
            public string Address { get; set; }

            public int Id { get; set; }

            public Guid UserId { get; set; }

            public Guid OrgId { get; set; }

            public string Name { get; set; }

            public string ForwardAddress { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public EmailAddressType AddressType { get; set; }

            public int Count { get; set; }

            public string DisplayName { get; set; }

            public int UnRead { get; set; }
            public bool IsDefault { get; set; }

            public static AddressItemModel FromAddress(EmailAddress address)
            {
                var result = new AddressItemModel
                {
                    //AddressOld = address,
                    AddressType = address.AddressType,
                    Name = address.Name,
                    Address = address.Address,
                    Id = address.Id,
                    UserId = address.UserId,
                    ForwardAddress = address.ForwardAddress,
                    IsDefault = address.IsDefault,

                };

                if (address.AddressType == EmailAddressType.Group)
                {
                    result.Count = address.Members.Count();
                }

                string name = address.Address;

                if (!string.IsNullOrWhiteSpace(address.Name))
                {
                    if (address.Address.Contains("<"))
                    {
                        name = address.Name + " " + address.Address;
                    }
                    else
                    {
                        name = address.Name + " <" + address.Address + ">";
                    }
                }
                result.DisplayName = name;

                return result;
            }
        }
    }
}
