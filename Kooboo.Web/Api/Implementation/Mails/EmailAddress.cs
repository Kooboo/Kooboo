//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Kooboo.Mail;
using Kooboo.Data.Models;
using Kooboo.Api;
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

        public List<Domain> Domains(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                return EmailForwardManager.Get<List<Domain>>(this.ModelName, nameof(EmailAddressApi.Domains), apiCall.Context.User, null);
            }

            return Data.GlobalDb.Domains.ListForEmail(apiCall.Context.User);
        }

        public List<AddressItemModel> List(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                return EmailForwardManager.Get<List<AddressItemModel>>(this.ModelName, nameof(EmailAddressApi.List), apiCall.Context.User);
            }
            var user = apiCall.Context.User;
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(user.CurrentOrgId);

            var addlist = orgdb.EmailAddress.Query().Where(o => o.UserId == user.Id).SelectAll();

            List<AddressItemModel> result = new List<AddressItemModel>();

            foreach (var item in addlist)
            {
                var model = AddressItemModel.FromAddress(item);
                result.Add(model);
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
            return orgdb.EmailAddress.Get(model.ToEmail()) == null;
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
                UserId = user.Id
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

            orgdb.EmailAddress.AddOrUpdate(address);

            return AddressItemModel.FromAddress(address);
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
                EmailForwardManager.Get<bool>(this.ModelName, nameof(EmailAddressApi.Deletes), apiCall.Context.User, dic);
                return;
            }

            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(apiCall.Context.User.CurrentOrgId);
            var ids = JsonConvert.DeserializeObject<int[]>(idsJson);

            foreach (var id in ids)
            {
                orgdb.EmailAddress.Delete(id);
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

            return orgdb.EmailAddress.GetMembers(addressid).OrderBy(o => o).ToList();
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

            var address = orgdb.EmailAddress.Get(addressid);
            if (Lib.Helper.StringHelper.IsSameValue(member.MemberAddress, address.Address))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Member address should not be same as group address", call.Context));
            }

            orgdb.EmailAddress.AddMember(addressid, member.MemberAddress);

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
            orgdb.EmailAddress.DeleteMember(addressid, memberAddress);
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
                var email = orgdb.EmailAddress.Get(id);
                if (email != null && email.AddressType == EmailAddressType.Forward)
                {
                    email.ForwardAddress = add;
                    orgdb.EmailAddress.AddOrUpdate(email);
                }
            }

        }

        public class ListMemberModel
        {
            public string MemberAddress { get; set; }
        }

        public class AddressModel
        {
            public string Local { get; set; }

            public string Domain { get; set; }

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
            public EmailAddress AddressOld { get; set; }

            public string Address { get; set; }

            public int Id { get; set; }

            public Guid UserId { get; set; }

            public Guid OrgId { get; set; }

            public string ForwardAddress { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public EmailAddressType AddressType { get; set; }

            public int Count { get; set; }

            public static AddressItemModel FromAddress(EmailAddress address)
            {
                var result = new AddressItemModel
                {
                    AddressOld = address,
                    AddressType = address.AddressType,
                    Address = address.Address,
                    Id = address.Id,
                    UserId = address.UserId,
                    ForwardAddress = address.ForwardAddress,
                };

                if (address.AddressType == EmailAddressType.Group)
                {
                    result.Count = address.Members.Count();
                }

                return result;
            }
        }
    }
}
