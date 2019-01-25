//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.ViewModel;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class TextContentApi : SiteObjectApi<TextContent>
    {
        [Attributes.RequireModel(typeof(LangTextContentViewModel))]
        public Guid LangUpdate(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            LangTextContentViewModel updatemodel = call.Context.Request.Model as LangTextContentViewModel;

            Guid folderId = GetFolderId(call);
            var contenttype = sitedb.ContentTypes.GetByFolder(folderId);
            if (contenttype == null || folderId == default(Guid))
            { return default(Guid); }

            Guid parentId = call.GetValue<Guid>("ParentId");

            string userkey = ExtraValue(updatemodel, "userkey");
            if (!string.IsNullOrEmpty(userkey))
            {
                userkey = Kooboo.Sites.Contents.UserKeyHelper.ToSafeUserKey(userkey);
            }

            string stronline = ExtraValue(updatemodel, "online");
            bool online = true;
            if (!string.IsNullOrEmpty(stronline) && stronline.ToLower() == "false")
            {
                online = false;
            }

            int sequence = 0;
            var strsequence = ExtraValue(updatemodel, "sequence");
            if (!string.IsNullOrEmpty(strsequence))
            {
                int.TryParse(strsequence, out sequence);
            }

            TextContent newcontent = sitedb.TextContent.Get(call.ObjectId);
            if (newcontent == null)
            {
                newcontent = new TextContent() { FolderId = folderId, ContentTypeId = contenttype.Id, UserKey = userkey, ParentId = parentId, Order = sequence };
                if (!string.IsNullOrEmpty(userkey) && sitedb.TextContent.IsUserKeyExists(userkey))
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("UserKey has been taken", call.Context));
                }
            }

            if (!string.IsNullOrEmpty(userkey) && newcontent.UserKey != userkey)
            {
                var existings = sitedb.TextContent.Get(userkey); 
                if (existings !=null && existings.Id != newcontent.Id)
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("UserKey has been taken", call.Context));
                }
                sitedb.TextContent.Delete(newcontent.Id);
                newcontent.UserKey = userkey;
            }

            newcontent.Online = online;
            newcontent.Order = sequence;
            newcontent.Embedded = updatemodel.Embedded;

            foreach (var item in contenttype.Properties.Where(o => !o.IsSystemField && !o.MultipleLanguage))
            {
                var value = ExtraValue(updatemodel, item.Name);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    newcontent.SetValue(item.Name, value, call.WebSite.DefaultCulture);
                }
            }

            foreach (var langDict in updatemodel.Values)
            {
                string lang = langDict.Key;
                foreach (var item in langDict.Value)
                {
                    string value = item.Value == null ? string.Empty : item.Value.ToString();
                    newcontent.SetValue(item.Key, value, lang);
                }
            }

            // sitedb.TextContent.EusureNonLangContent(newcontent, contenttype);

            sitedb.TextContent.AddOrUpdate(newcontent, call.Context.User.Id);

            if (updatemodel.Categories.Count > 0)
            {
                foreach (var item in updatemodel.Categories)
                {
                    sitedb.ContentCategories.UpdateCategory(newcontent.Id, item.Key, item.Value.ToList(), call.Context.User.Id);
                }
            }
            return newcontent.Id;
        }

        [Attributes.RequireParameters("FolderId")]
        public List<ContentFieldViewModel> GetFields(ApiCall call)
        {
            Guid FolderId = call.GetGuidValue("FolderId");

            if (FolderId != default(Guid))
            {
                var contenttype = call.WebSite.SiteDb().ContentTypes.GetByFolder(FolderId);
                List<ContentFieldViewModel> result = new List<ContentFieldViewModel>();

                foreach (var item in contenttype.Properties)
                {
                    if (item.Editable)
                    {
                        ContentFieldViewModel model = new ContentFieldViewModel();
                        model.Name = item.Name;
                        model.Validations = item.Validations;
                        model.ControlType = item.ControlType;
                        model.ToolTip = item.Tooltip;
                        model.Order = item.Order;
                        // model.Value = content.GetValue(item.Name);
                        model.IsMultilingual = item.MultipleLanguage;
                        model.MultipleValue = item.MultipleValue;
                        model.selectionOptions = item.selectionOptions;
                        result.Add(model);
                    }
                }
                return result;
            }
            return null;
        }


        public PagedListViewModel<TextContentViewModel> ByFolder(Guid FolderId, ApiCall call)
        {
            int pagesize = ApiHelper.GetPageSize(call, 50);
            int pagenr = ApiHelper.GetPageNr(call);

            string language =  Kooboo.Data.Language.LanguageSetting.GetCmsSiteLangCode(call.Context, call.WebSite);
                                           
            PagedListViewModel<TextContentViewModel> model = new PagedListViewModel<TextContentViewModel>();
            model.PageNr = pagenr;
            model.PageSize = pagesize;

            var textContents = call.WebSite.SiteDb().TextContent.Query.Where(it => it.FolderId == FolderId).SelectAll();

            model.TotalCount = textContents.Count();
            model.TotalPages = ApiHelper.GetPageCount(model.TotalCount, model.PageSize);

            var list = textContents.OrderByDescending(o => o.LastModified).Skip(model.PageNr * model.PageSize - model.PageSize).Take(model.PageSize).ToList();

            var contenttype = call.Context.WebSite.SiteDb().ContentTypes.GetByFolder(FolderId);

            model.List = new List<TextContentViewModel>();
            foreach (var item in list)
            {
                model.List.Add(Sites.Helper.ContentHelper.ToListDisplayView(item, contenttype, language));
            }

            return model;
        }

        private List<CategoryContentViewModel> GetCategoryInfo(ApiCall call, Guid folderId, Guid textContentId, string language = null)
        {
            var sitedb = call.WebSite.SiteDb();
            List<CategoryContentViewModel> categories = new List<CategoryContentViewModel>();
            var folder = sitedb.ContentFolders.Get(folderId);
            if (folder == null || (folder.Category == null || folder.Category.Count() == 0))
            {
                return categories;
            }

            foreach (var item in folder.Category)
            {
                var categoryfolder = sitedb.ContentFolders.Get(item.FolderId);

                var ids = sitedb.ContentCategories.Query.Where(o => o.ContentId == textContentId && o.CategoryFolder == item.FolderId).SelectAll().Select(o => o.CategoryId).ToList();

                CategoryContentViewModel model = new CategoryContentViewModel();
                model.CategoryFolder = categoryfolder;

                var contents = sitedb.TextContent.Query.
                            Where(it => it.FolderId == item.FolderId)
                            .WhereIn("Id", ids)
                            .SelectAll();

                model.MultipleChoice = item.Multiple;
                model.Contents = contents.Select(o => sitedb.TextContent.GetView(o, language)).ToList();
                model.Alias = item.Alias;

                categories.Add(model);
            }

            return categories;
        }


        private List<ContentFieldViewModel> GetProperties(ApiCall call, Guid FolderId)
        {
            var culture = call.WebSite.DefaultCulture;

            List<ContentFieldViewModel> result = new List<ContentFieldViewModel>();
            var contenttype = call.WebSite.SiteDb().ContentTypes.GetByFolder(FolderId);

            bool online = true;
            TextContent textcontentnew = null;

            if (call.ObjectId != default(Guid))
            {
                textcontentnew = call.WebSite.SiteDb().TextContent.Get(call.ObjectId);
                online = textcontentnew.Online;
            }

            if (textcontentnew == null)
            {
                foreach (var item in contenttype.Properties)
                {
                    if (item.Editable)
                    {
                        ContentFieldViewModel model = new ContentFieldViewModel();
                        model.Name = item.Name;
                        model.DisplayName = item.DisplayName;
                        model.Validations = item.Validations;
                        model.ControlType = item.ControlType;
                        model.ToolTip = item.Tooltip;
                        model.Order = item.Order;

                        if (item.MultipleLanguage)
                        {
                            foreach (var cultureitem in call.WebSite.Culture.Keys.ToList())
                            {
                                model.Values.Add(cultureitem, "");
                            }
                        }
                        else
                        {
                            if (item.DataType == Data.Definition.DataTypes.Bool)
                            {
                                model.Values.Add(call.WebSite.DefaultCulture, "true");
                            }
                            else
                            {
                                model.Values.Add(call.WebSite.DefaultCulture, "");
                            }
                        }
                        model.IsMultilingual = item.MultipleLanguage;
                        model.selectionOptions = item.selectionOptions;
                        model.MultipleValue = item.MultipleValue;

                        result.Add(model);
                    }
                }
            }
            else
            {
                foreach (var item in contenttype.Properties)
                {
                    if (item.Editable)
                    {
                        ContentFieldViewModel model = new ContentFieldViewModel();
                        model.Name = item.Name;
                        model.Validations = item.Validations;
                        model.ControlType = item.ControlType;
                        model.DisplayName = item.DisplayName;
                        model.IsMultilingual = item.MultipleLanguage;
                        model.ToolTip = item.Tooltip;
                        model.Order = item.Order;
                        model.selectionOptions = item.selectionOptions;
                        model.MultipleValue = item.MultipleValue;

                        if (item.MultipleLanguage)
                        {
                            foreach (var lang in textcontentnew.Contents)
                            {
                                var itemvalue = textcontentnew.GetValue(model.Name, lang.Lang);
                                model.Values[lang.Lang] = itemvalue != null ? itemvalue.ToString() : string.Empty;
                            }
                            foreach (var sitelang in call.WebSite.Culture.Keys.ToList())
                            {
                                if (!model.Values.ContainsKey(sitelang))
                                {
                                    model.Values.Add(sitelang, "");
                                }
                            }
                        }
                        else
                        {
                            var itemvalue = textcontentnew.GetValue(model.Name, culture);
                            model.Values[culture] = itemvalue != null ? itemvalue.ToString() : null;
                        }
                        result.Add(model);
                    }
                }
            }

            var onlineitem = result.Find(o => o.Name.ToLower() == "online");
            if (onlineitem != null)
            {
                result.Remove(onlineitem);

                onlineitem.ControlType = "boolean";
                result.Add(onlineitem);
            }

            return result;
        }

        public ContentEditViewModel GetEdit(ApiCall call)
        {
            Guid FolderId = GetFolderId(call);
            if (FolderId == default(Guid))
            {
                return null;
            }
            ContentEditViewModel model = new ContentEditViewModel();
            model.FolderId = FolderId;
            model.Properties = GetProperties(call, FolderId).OrderBy(o => o.Order).ToList();

            model.Categories = GetCategoryInfo(call, FolderId, call.ObjectId, call.Context.Culture);
            model.Embedded = Sites.Helper.ContentHelper.GetEmbedContents(call.WebSite.SiteDb(), FolderId, call.ObjectId, call.Context.Culture);
            return model;
        }

        public Guid GetFolderId(ApiCall call)
        {
            Guid folderId = call.GetValue<Guid>("FolderId");
            if (folderId == default(Guid))
            {
                if (call.ObjectId != default(Guid))
                {
                    var content = call.WebSite.SiteDb().TextContent.Get(call.ObjectId);
                    if (content != null)
                    {
                        folderId = content.FolderId;
                    }
                }
            }
            return folderId;
        }

        public string ExtraValue(LangTextContentViewModel updatemodel, string FieldName)
        {
            if (string.IsNullOrWhiteSpace(FieldName))
            {
                return null;
            }
            FieldName = FieldName.ToLower();

            string key = null;

            foreach (var langitem in updatemodel.Values)
            {
                foreach (var fielditem in langitem.Value)
                {
                    if (fielditem.Key.ToLower() == FieldName)
                    {
                        key = fielditem.Value;
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(key))
            {
                // remove the key 
                foreach (var langitem in updatemodel.Values)
                {
                    List<string> keysToRemove = new List<string>();

                    foreach (var fielditem in langitem.Value)
                    {
                        if (fielditem.Key.ToLower() == FieldName)
                        {
                            keysToRemove.Add(fielditem.Key);
                        }
                    }

                    foreach (var item in keysToRemove)
                    {
                        langitem.Value.Remove(item);
                    }
                }
            }
            return key;
        }


        public PagedListViewModel<TextContentViewModel> Search(Guid FolderId, string Keyword, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            if (string.IsNullOrWhiteSpace(Keyword) || FolderId == default(Guid))
            {
                return new PagedListViewModel<TextContentViewModel>();
            }

            int pagesize = ApiHelper.GetPageSize(call, 50);
            int pagenr = ApiHelper.GetPageNr(call);

            string language = string.IsNullOrEmpty(call.Context.Culture) ? call.WebSite.DefaultCulture : call.Context.Culture;

            PagedListViewModel<TextContentViewModel> model = new PagedListViewModel<TextContentViewModel>();
            model.PageNr = pagenr;
            model.PageSize = pagesize;

            var all = sitedb.TextContent.Query.Where(o => o.FolderId == FolderId).SelectAll();

            var textContents = all.Where(o => o.Body.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase)>-1).OrderByDescending(o => o.LastModified);

            model.TotalCount = textContents.Count();
            model.TotalPages = ApiHelper.GetPageCount(model.TotalCount, model.PageSize);

            var list = textContents.OrderByDescending(o => o.LastModified).Skip(model.PageNr * model.PageSize - model.PageSize).Take(model.PageSize).ToList();

            var contenttype = call.Context.WebSite.SiteDb().ContentTypes.GetByFolder(FolderId);

            model.List = new List<TextContentViewModel>();
            foreach (var item in list)
            {
                model.List.Add(Sites.Helper.ContentHelper.ToListDisplayView(item, contenttype, language));
            }

            return model;
        }

    }

}
