//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class FormApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "form";
            }
        }

        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        [Permission(Feature.FORM, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<FormListItemViewModel> External(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();
            int storenameHash = Lib.Security.Hash.ComputeInt(sitedb.Forms.StoreName);
            List<IEmbeddableItemListViewModel> result = new List<IEmbeddableItemListViewModel>();

            foreach (var item in sitedb.Forms.GetExternals().SortByNameOrLastModified(apiCall))
            {
                IEmbeddableItemListViewModel model = new IEmbeddableItemListViewModel(sitedb, item);
                model.KeyHash = Sites.Service.LogService.GetKeyHash(item.Id);
                model.StoreNameHash = storenameHash;

                model.Type = item.FormType.ToString();

                result.Add(model);
            }

            return ToFormList(sitedb, result, false);
        }

        [Permission(Feature.FORM, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<FormListItemViewModel> Embedded(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();
            var list = sitedb
                .Forms
                .GetEmbeddeds()
                .SortByBodyOrLastModified(apiCall)
                .Select(o => new IEmbeddableItemListViewModel(sitedb, o))
                .ToList();
            return ToFormList(sitedb, list, true);
        }

        private IEnumerable<FormListItemViewModel> ToFormList(SiteDb sitedb, IEnumerable<IEmbeddableItemListViewModel> items, bool isEmbedded)
        {
            foreach (var item in items)
            {
                yield return new FormListItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    LastModified = item.LastModified,
                    ValueCount = sitedb.FormValues.Query.Where(o => o.FormId == item.Id).Count(),
                    Source = item.Source,
                    IsEmbedded = isEmbedded,
                    References = item.References,
                    FormType = item.Type,
                    KeyHash = item.KeyHash,
                    StoreNameHash = item.StoreNameHash
                };
            }
        }

        [Kooboo.Attributes.RequireParameters("id")]
        [Permission(Feature.FORM, Action = Data.Permission.Action.VIEW)]
        public PagedListViewModel<FormValue> FormValues(ApiCall call)
        {
            PagedListViewModel<FormValue> result = new PagedListViewModel<Sites.Models.FormValue>();

            var pager = ApiHelper.GetPager(call, 50);

            var total = call.WebSite.SiteDb().FormValues.Query.Where(o => o.FormId == call.ObjectId).Count();

            result.TotalCount = total;
            result.TotalPages = ApiHelper.GetPageCount(total, pager.PageSize);

            result.PageNr = pager.PageNr;

            result.List = call.WebSite.SiteDb().FormValues.Query.Where(o => o.FormId == call.ObjectId).OrderByDescending(o => o.CreationDate).Skip(pager.SkipCount).Take(pager.PageSize);
            return result;
        }

        [Kooboo.Attributes.RequireParameters("id")]
        [Permission(Feature.FORM, Action = Data.Permission.Action.VIEW)]
        public FormValue FormValue(ApiCall call)
        {
            return call.WebSite.SiteDb().FormValues.Get(call.ObjectId);
        }

        [Kooboo.Attributes.RequireParameters("id")]
        [Permission(Feature.FORM, Action = Data.Permission.Action.VIEW)]
        public FormEditViewModel GetEdit(ApiCall call)
        {
            FormEditViewModel edit = new FormEditViewModel();
            var form = call.WebSite.SiteDb().Forms.Get(call.ObjectId);

            if (form != null)
            {
                edit.Id = form.Id;
                edit.Body = form.Body;
                edit.IsEmbedded = form.IsEmbedded;
                edit.Name = form.Name;
                edit.Fields = form.Fields;
                edit.Style = form.Style;

                foreach (var item in Kooboo.Sites.HtmlForm.FormManager.List)
                {
                    edit.AvailableSubmitters.Add(new FormSubmitterViewModel() { Name = item.Name, Settings = item.Settings(call.Context) });
                }


                // should be ignored.
                edit.Settings = form.Setting;

                if (!string.IsNullOrEmpty(edit.Submitter))
                {
                    var submitter = edit.AvailableSubmitters.Find(o => o.Name == edit.Submitter);

                    if (submitter != null)
                    {
                        foreach (var item in submitter.Settings)
                        {
                            if (edit.Settings.ContainsKey(item.Name))
                            {
                                item.DefaultValue = edit.Settings[item.Name];
                            }
                        }

                    }
                }

                return edit;
            }
            else
            {
                return new FormEditViewModel();
            }
        }

        [Attributes.RequireParameters("Id", "Body", "IsEmbedded")]
        [Permission(Feature.FORM, Action = Data.Permission.Action.EDIT)]
        public Guid Post(ApiCall call)
        {
            bool isEmbedded = call.GetValue<bool>("IsEmbedded");

            var sitedb = call.WebSite.SiteDb();

            string body = call.GetValue("Body");

            string fields = call.GetValue("Fields");

            if (call.ObjectId == default(Guid))
            {
                // create new..
                string name = call.GetValue("Name");
                Form form = new Form() { Name = name, Body = body, Fields = fields, IsEmbedded = isEmbedded };

                if (!string.IsNullOrEmpty(fields))
                {
                    form.FormType = FormType.KoobooForm;
                    form.IsEmbedded = false;
                }

                sitedb.Forms.AddOrUpdate(form, call.Context.User.Id);

                return form.Id;
            }
            else
            {
                // update form. 
                var form = sitedb.Forms.Get(call.ObjectId);
                if (form != null)
                {
                    form.Body = body;
                    form.Fields = fields;
                    sitedb.Forms.AddOrUpdate(form, true, true, call.Context.User.Id);
                }

                return call.ObjectId;
            }
        }

        [Permission(Feature.FORM, Action = Data.Permission.Action.VIEW)]
        public FormSettingEditViewModel GetSetting(ApiCall call)
        {
            var setting = call.WebSite.SiteDb().FormSetting.GetByFormId(call.ObjectId);

            FormSettingEditViewModel result = new FormSettingEditViewModel();

            if (setting == null)
            {
                result = new FormSettingEditViewModel() { FormId = call.ObjectId };
            }
            else
            {
                result.Id = setting.Id;
                result.FormId = setting.FormId;
                result.FormSubmitter = setting.FormSubmitter;
                result.Method = setting.Method;
                result.RedirectUrl = setting.RedirectUrl;
                result.AllowAjax = setting.AllowAjax;
                result.SuccessCallBack = setting.SuccessCallBack;
                result.FailedCallBack = setting.FailedCallBack;
                result.Enable = setting.Enable;
                result.Setting = setting.Setting;
            }

            foreach (var item in Kooboo.Sites.HtmlForm.FormManager.List)
            {
                result.AvailableSubmitters.Add(new FormSubmitterViewModel() { Name = item.Name, Settings = item.Settings(call.Context) });
            }

            if (!string.IsNullOrEmpty(result.FormSubmitter))
            {
                // set default value. 
                if (result.Setting != null && result.Setting.Count() > 0)
                {
                    var available = result.AvailableSubmitters.Find(o => o.Name == result.FormSubmitter);
                    if (available != null)
                    {
                        foreach (var item in available.Settings)
                        {
                            if (result.Setting.ContainsKey(item.Name))
                            {
                                item.DefaultValue = result.Setting[item.Name];
                            }

                        }
                    }
                }

            }


            return result;
        }

        [Attributes.RequireModel(typeof(FormSetting))]
        [Permission(Feature.FORM, Action = Data.Permission.Action.EDIT)]
        public void UpdateSetting(ApiCall call)
        {
            var model = call.Context.Request.Model as FormSetting;
            call.WebSite.SiteDb().FormSetting.AddOrUpdate(model, call.Context.User.Id);
        }


        [Attributes.RequireModel(typeof(KoobooFormEditModel))]
        [Permission(Feature.FORM, Action = Data.Permission.Action.EDIT)]
        public Guid UpdateKoobooForm(ApiCall call)
        {
            var model = call.Context.Request.Model as KoobooFormEditModel;
            var sitedb = call.Context.WebSite.SiteDb();

            Guid formid = model.Id;

            // update form body.... 
            if (formid != default(Guid))
            {
                var form = sitedb.Forms.Get(call.ObjectId);
                if (form != null)
                {
                    form.Body = model.Body;
                    form.Fields = model.Fields;
                    form.Style = model.Style;
                    sitedb.Forms.AddOrUpdate(form, true, true, call.Context.User.Id);
                    formid = form.Id;
                }
                else
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("Form not found", call.Context));
                }
            }
            else
            {
                string name = call.GetValue("Name");
                Form form = new Form() { Name = model.Name, Body = model.Body, Fields = model.Fields, IsEmbedded = model.IsEmbedded };

                if (!string.IsNullOrEmpty(model.Fields))
                {
                    form.FormType = FormType.KoobooForm;
                    form.IsEmbedded = false;
                }

                sitedb.Forms.AddOrUpdate(form, call.Context.User.Id);
                formid = form.Id;
            }

            FormSetting formsetting = new FormSetting();
            formsetting.FormId = formid;
            formsetting.AllowAjax = model.AllowAjax;
            formsetting.FailedCallBack = model.FailedCallBack;
            formsetting.SuccessCallBack = model.SuccessCallBack;

            formsetting.RedirectUrl = model.RedirectUrl;
            formsetting.Method = model.Method;
            formsetting.Setting = model.Setting;
            formsetting.Enable = true;

            formsetting.FormSubmitter = model.FormSubmitter;

            sitedb.FormSetting.AddOrUpdate(formsetting, call.Context.User.Id);

            return formid;
        }

        [Permission(Feature.FORM, Action = Data.Permission.Action.VIEW)]
        public KoobooFormEditModel GetKoobooForm(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            KoobooFormEditModel edit = new KoobooFormEditModel();
            var form = sitedb.Forms.Get(call.ObjectId);
            if (form != null)
            {
                edit.Id = form.Id;
                edit.Body = form.Body;
                edit.IsEmbedded = form.IsEmbedded;
                edit.Name = form.Name;
                edit.Fields = form.Fields;
                edit.Style = form.Style;

                foreach (var item in Kooboo.Sites.HtmlForm.FormManager.List)
                {
                    edit.AvailableSubmitters.Add(new FormSubmitterViewModel() { Name = item.Name, Settings = item.Settings(call.Context) });
                }

                var formsetting = sitedb.FormSetting.GetByFormId(call.ObjectId);

                if (formsetting != null)
                {
                    edit.AllowAjax = formsetting.AllowAjax;
                    edit.Enable = formsetting.Enable;
                    edit.FailedCallBack = formsetting.FailedCallBack;
                    edit.SuccessCallBack = formsetting.SuccessCallBack;
                    edit.RedirectUrl = formsetting.RedirectUrl;
                    edit.Setting = formsetting.Setting;
                    edit.Method = formsetting.Method;
                    edit.FormSubmitter = formsetting.FormSubmitter;

                    if (!string.IsNullOrEmpty(edit.FormSubmitter))
                    {
                        // set default value. 
                        if (edit.Setting != null && edit.Setting.Count() > 0)
                        {
                            var available = edit.AvailableSubmitters.Find(o => o.Name == edit.FormSubmitter);
                            if (available != null)
                            {
                                foreach (var item in available.Settings)
                                {
                                    if (edit.Setting.ContainsKey(item.Name))
                                    {
                                        item.DefaultValue = edit.Setting[item.Name];
                                    }

                                }
                            }
                        }

                    }
                }
                return edit;
            }
            else
            {
                var result = new KoobooFormEditModel();

                foreach (var item in Kooboo.Sites.HtmlForm.FormManager.List)
                {
                    result.AvailableSubmitters.Add(new FormSubmitterViewModel() { Name = item.Name, Settings = item.Settings(call.Context) });
                }

                return result;
            }
        }


        [Kooboo.Attributes.RequireModel(typeof(FormUpdateViewModel))]
        [Permission(Feature.FORM, Action = Data.Permission.Action.EDIT)]
        public void UpdateForm(ApiCall call)
        {
            FormUpdateViewModel model = call.Context.Request.Model as FormUpdateViewModel;

            var form = call.WebSite.SiteDb().Forms.Get(model.Id);

            if (form != null)
            {
                form.Body = model.Body;


                form.FormSubmitter = model.Submitter;
                form.Setting = model.Settings;
                form.Method = model.Method;
                form.AllowAjax = model.AllowAjax;
                form.RedirectUrl = model.RedirectUrl;
                form.IsEmbedded = false;   // once is updated..... 
                form.SuccessCallBack = model.SuccessCallBack;
                form.FailedCallBack = model.FailedCallBack;
                call.WebSite.SiteDb().Forms.AddOrUpdate(form, call.Context.User.Id);
            }
        }

        [Kooboo.Attributes.RequireParameters("ids")]
        [Permission(Feature.FORM, Action = Data.Permission.Action.DELETE)]
        public void DeleteFormValues(ApiCall call)
        {
            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }

            List<Guid> ids = Kooboo.Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);

            if (ids != null && ids.Count() > 0)
            {
                foreach (var item in ids)
                {
                    call.WebSite.SiteDb().FormValues.Delete(item, call.Context.User.Id);
                }
            }
        }

        [Attributes.RequireParameters("ids")]
        [Permission(Feature.FORM, Action = Data.Permission.Action.DELETE)]
        public bool Deletes(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }
            List<Guid> ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);

            if (ids != null && ids.Count() > 0)
            {
                foreach (var item in ids)
                {
                    sitedb.Forms.Delete(item, call.Context.User.Id);
                }
                return true;
            }
            return false;
        }

        public bool IsUniqueName(string name, ApiCall call)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = name.ToLower();

                var value = call.WebSite.SiteDb().Forms.Store.FullScan(o => o.Name != null && o.Name.ToLower() == name).FirstOrDefault();
                if (value != null)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
