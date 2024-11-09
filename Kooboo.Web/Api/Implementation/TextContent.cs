//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Context;
using Kooboo.Data.Definition;
using Kooboo.Data.Language;
using Kooboo.Data.Permission;
using Kooboo.Data.Typescript;
using Kooboo.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Contents;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Helper;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Sites.ViewModel;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class TextContentApi : SiteObjectApi<TextContent>
    {
        [Attributes.RequireModel(typeof(LangTextContentViewModel))]
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.EDIT)]
        public Guid LangUpdate(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            LangTextContentViewModel updatemodel = call.Context.Request.Model as LangTextContentViewModel;

            var (TypeId, FolderId) = GetRelationId(call);
            var contenttype = sitedb.ContentTypes.Get(TypeId);
            if (contenttype == null)
            {
                return default;
            }

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
                newcontent = new TextContent()
                {
                    FolderId = FolderId,
                    ContentTypeId = contenttype.Id,
                    UserKey = userkey,
                    ParentId = parentId,
                    Order = sequence
                };
                if (call.ObjectId != default)
                {
                    newcontent.UserKey = call.ObjectId.ToString();
                }
                if (!string.IsNullOrEmpty(userkey) && sitedb.TextContent.IsUserKeyExists(userkey))
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("UserKey has been taken", call.Context));
                }
            }

            if (!string.IsNullOrEmpty(userkey) && newcontent.UserKey != userkey)
            {
                var existings = sitedb.TextContent.Get(userkey);
                if (existings != null && existings.Id != newcontent.Id)
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
                    foreach (var lang in newcontent.Contents)
                    {
                        if (!lang.FieldValues.ContainsKey(item.Name)) continue;
                        lang.FieldValues[item.Name] = value;
                    }
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
                    sitedb.ContentCategories.UpdateCategory(newcontent.Id, item.Key, item.Value.ToList(),
                        call.Context.User.Id);
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
                        ContentFieldViewModel model = new ContentFieldViewModel
                        {
                            Name = item.Name,
                            Validations = item.Validations,
                            ControlType = item.ControlType,
                            ToolTip = item.Tooltip,
                            Order = item.Order,
                            Settings = item.Settings,
                            IsMultilingual = item.MultipleLanguage,
                            MultipleValue = item.MultipleValue,
                            selectionOptions = GetSelectionOptions(item, call.Context, null),
                        };
                        result.Add(model);
                    }
                }

                return result;
            }

            return null;
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.VIEW)]
        public PagedTextContentListViewModel ByFolder(string FolderId, ApiCall call)
        {
            int pagesize = ApiHelper.GetPageSize(call, 50);
            int pagenr = ApiHelper.GetPageNr(call);

            string language = call.WebSite.DefaultCulture;
            var siteDb = call.WebSite.SiteDb();
            var folder = siteDb.ContentFolders.Get(FolderId);
            var columns = siteDb
                .ContentTypes
                .GetTitlePropertyByContentType(folder.ContentTypeId)
                .OrderBy(it => it.Order)
                .Select(it => new BaseColumnViewModel
                {
                    Name = it.Name,
                    DisplayName = it.DisplayName,
                    ControlType = it.ControlType,
                    MultipleValue = it.MultipleValue,
                    IsSummaryField = it.IsSummaryField,
                    SelectionOptions = GetSelectionOptions(it, call.Context, null)
                });
            PagedTextContentListViewModel model = new()
            {
                Columns = columns,
                PageNr = pagenr,
                PageSize = pagesize,
                Categories = GetCategoriesOptions(call, folder),
            };
            if (folder == null)
            {
                model.TotalCount = 0;
                model.TotalPages = 0;
                return model;
            }

            var sortField = call.GetValue("sortField") ?? string.Empty;
            var ascending = call.GetValue("ascending")?.ToLower() == "true";
            var exclude = call.GetValue<Guid[]>("exclude") ?? [];

            var categoriesValue = call.GetValue("categories");
            var categories = string.IsNullOrEmpty(categoriesValue) ? null : JsonHelper.Deserialize<Dictionary<Guid, List<Guid>>>(categoriesValue);

            var textContents = siteDb.TextContent.GetSortedTextContentsByFolder(folder, true, sortField, ascending, categories).Where(w => !exclude.Contains(w.Id));

            model.TotalCount = textContents.Count();
            model.TotalPages = ApiHelper.GetPageCount(model.TotalCount, model.PageSize);

            var list = textContents
                .Skip(model.PageNr * model.PageSize - model.PageSize)
                .Take(model.PageSize)
                .ToList();

            model.List = ToTextContentViewModel(siteDb, folder.Id, list, language);
            return model;
        }

        [Attributes.RequireParameters("changes")]
        [Permission(Feature.CONTENT, Action = Data.Permission.Action.EDIT)]
        public void Move(ApiCall call)
        {
            var model = JsonHelper.Deserialize<MoveTextContentViewModel>(call.GetValue("changes"));
            if (model.Source == default || model.FolderId == default || (!model.PrevId.HasValue && !model.NextId.HasValue))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Invalid Parameters", call.Context));
            }

            var siteDb = call.WebSite.SiteDb();

            var folder = siteDb.ContentFolders.Get(model.FolderId);
            if (folder == null)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Invalid Parameters", call.Context));
            }

            var folderContents = siteDb.TextContent.Query
                .Where(o => o.FolderId == folder.Id)
                .SelectAll();
            var toUpdates = TextContentHelper.CalculateToUpdateItems(folderContents, model).ToList();
            foreach (var item in toUpdates)
            {
                siteDb.TextContent.AddOrUpdate(item, call.Context.User.Id);
            }
        }

        private IEnumerable<TextContent> FilterByCategories(ApiCall call, SiteDb siteDb, IEnumerable<TextContent> textContents)
        {
            var categories = call.GetValue("categories");
            if (!string.IsNullOrEmpty(categories))
            {
                var cates = JsonHelper.Deserialize<Dictionary<Guid, List<Guid>>>(categories);
                foreach (var item in cates)
                {
                    if (item.Value.Count == 0)
                    {
                        continue;
                    }

                    var ids = siteDb.ContentCategories
                        .Query
                        .Where(it => it.CategoryFolder == item.Key)
                        .WhereIn(it => it.CategoryId, item.Value)
                        .SelectAll()
                    .Select(it => it.ContentId);
                    textContents = textContents.Where(it => ids.Contains(it.Id));
                }
            }
            return textContents;
        }

        private List<CategoryOptionViewModel> GetCategoriesOptions(ApiCall call, ContentFolder folder)
        {
            var folderIds = folder.Category.Select(it => it.FolderId).ToList();
            var sitedb = call.WebSite.SiteDb();
            var options = sitedb
                .TextContent
                .Query
                .WhereIn(it => it.FolderId, folderIds)
                .SelectAll();
            var categories = new List<CategoryOptionViewModel>();

            foreach (var it in folder.Category)
            {
                var contentType = sitedb.ContentTypes.GetByFolder(it.FolderId);
                var columns = contentType.Properties.Select(it => new BaseColumnViewModel
                {
                    Name = it.Name,
                    DisplayName = it.DisplayName,
                    ControlType = it.ControlType,
                    MultipleValue = it.MultipleValue,
                    IsSummaryField = it.IsSummaryField,
                    SelectionOptions = GetSelectionOptions(it, call.Context, null)
                }).ToArray();


                var item = new CategoryOptionViewModel
                {
                    Alias = it.Alias,
                    Display = it.Display,
                    MultipleChoice = it.Multiple,
                    Id = it.FolderId,
                    Columns = columns,
                    Options = options
                        .Where(o => o.FolderId == it.FolderId)
                        .Select(o => sitedb.TextContent.GetView(o, call.WebSite.DefaultCulture))
                        .ToList()
                };

                categories.Add(item);
            }

            return categories;
        }

        private List<TextContentViewModel> ToTextContentViewModel(SiteDb siteDb, Guid folderId, List<TextContent> list, string language)
        {
            var result = new List<TextContentViewModel>();
            var embedded = Sites.Helper.ContentHelper.GetUsedByContents(siteDb, folderId, list);
            var contenttype = siteDb.ContentTypes.GetByFolder(folderId);
            foreach (var item in list)
            {
                var view = Sites.Helper.ContentHelper.ToListDisplayView(item, contenttype, language);
                view.UsedBy = embedded.GetValueOrDefault(item.Id);
                Sites.Helper.ContentHelper.ShortenValues(view, contenttype);
                result.Add(view);
            }
            return result;
        }

        private List<CategoryContentViewModel> GetCategoryInfo(ApiCall call, Guid folderId, Guid textContentId,
            string language = null)
        {
            var sitedb = call.WebSite.SiteDb();
            List<CategoryContentViewModel> categories = new List<CategoryContentViewModel>();
            var folder = sitedb.ContentFolders.Get(folderId);
            if (folder == null || (folder.Category == null || folder.Category.Count() == 0))
            {
                return categories;
            }
            TextContent content = sitedb.TextContent.Get(textContentId);
            if (content == null)
            {
                content = new TextContent();
            }
            foreach (var item in folder.Category)
            {
                var categoryfolder = sitedb.ContentFolders.Get(item.FolderId);
                var contentType = sitedb.ContentTypes.Get(categoryfolder.ContentTypeId);

                var ids = sitedb.ContentCategories.Query
                    .Where(o => o.ContentId == textContentId && o.CategoryFolder == item.FolderId)
                    .SelectAll()
                    .OrderBy(o => o.Order)
                    .Select(o => o.CategoryId)
                    .ToList();

                var contentsMap = sitedb.TextContent.Query
                    .Where(it => it.FolderId == item.FolderId)
                    .WhereIn("Id", ids)
                    .SelectAll()
                    .ToDictionary(it => it.Id, it => it);

                var contents = ids
                    .Select((id, ix) =>
                    {
                        if (!contentsMap.TryGetValue(id, out var content) || content == null)
                        {
                            return null;
                        }
                        content.Order = ix;
                        return content;
                    })
                    .Where(it => it != null)
                    .ToList();

                var columns = contentType.Properties.Select(it => new BaseColumnViewModel
                {
                    Name = it.Name,
                    DisplayName = it.DisplayName,
                    ControlType = it.ControlType,
                    MultipleValue = it.MultipleValue,
                    IsSummaryField = it.IsSummaryField,
                    SelectionOptions = GetSelectionOptions(it, call.Context, null)
                }).ToArray();

                CategoryContentViewModel model = new CategoryContentViewModel
                {
                    CategoryFolder = categoryfolder,
                    Columns = columns,
                    MultipleChoice = item.Multiple,
                    Contents = contents.Select(o => sitedb.TextContent.GetView(o, language)).ToList(),
                    Alias = item.Alias,
                    Display = item.Display
                };

                categories.Add(model);
            }

            return categories;
        }

        public List<ViewModel.EmbeddedContentViewModel> GetEmbedContents(ApiCall call, Guid folderId, Guid textContentId, string language = null)
        {
            var sitedb = call.WebSite.SiteDb();
            var embedded = new List<ViewModel.EmbeddedContentViewModel>();
            var folder = sitedb.ContentFolders.Get(folderId);
            if (folder == null || (folder.Embedded == null || folder.Embedded.Count() == 0))
            {
                return embedded;
            }
            TextContent content = sitedb.TextContent.Get(textContentId);
            if (content == null)
            {
                content = new TextContent();
            }

            foreach (var item in folder.Embedded)
            {
                var embeddedFolder = sitedb.ContentFolders.Get(item.FolderId);
                var contentType = sitedb.ContentTypes.Get(embeddedFolder.ContentTypeId);

                List<Guid> ids = new List<Guid>();
                if (content.Embedded.ContainsKey(item.FolderId))
                {
                    var currentids = content.Embedded[item.FolderId];
                    if (currentids != null && currentids.Count > 0)
                    {
                        ids = currentids;
                    }
                }

                var model = new ViewModel.EmbeddedContentViewModel();
                model.EmbeddedFolder = embeddedFolder;
                model.Columns = contentType.Properties
                .Select(it => new BaseColumnViewModel
                {
                    Name = it.Name,
                    DisplayName = it.DisplayName,
                    ControlType = it.ControlType,
                    MultipleValue = it.MultipleValue,
                    IsSummaryField = it.IsSummaryField,
                    SelectionOptions = GetSelectionOptions(it, call.Context, null)
                }).ToArray();

                // var contents = sitedb.TextContent.Query.
                //           Where(it => it.FolderId == item.FolderId)
                //          .WhereIn("Id", ids)
                //        .SelectAll();

                var contentsMap = sitedb.TextContent.Store.Filter
                    .WhereEqual(nameof(TextContent.FolderId), item.FolderId)
                    .WhereIn("Id", ids)
                    .SelectAll()
                    .ToDictionary(it => it.Id, it => it);

                var contents = ids
                    .Select((id, index) =>
                    {
                        if (!contentsMap.TryGetValue(id, out var content) || content == null)
                        {
                            return null;
                        }
                        content.Order = index;
                        return content;
                    })
                    .Where(it => it != null)
                    .ToList();

                // also display use the parent ids. 
                if (textContentId != default(Guid))
                {
                    // var subcontents = sitedb.TextContent.Query.Where(o => o.FolderId == item.FolderId && o.ParentId == textContentId).SelectAll();
                    var subcontents = sitedb.TextContent.Store.Filter.WhereEqual(nameof(TextContent.FolderId), item.FolderId).WhereEqual(nameof(TextContent.ParentId), textContentId).SelectAll();


                    foreach (var subitem in subcontents)
                    {
                        if (contents.Find(o => o.Id == subitem.Id) == null)
                        {
                            contents.Add(subitem);
                        }
                    }
                }
                model.Contents = contents.Select(o => ContentHelper.ToView(o, language, sitedb.ContentTypes.GetTitlePropertyByFolder(item.FolderId))).ToList();

                ContentHelper.CleanNonSummaryFields(sitedb, model, item.FolderId);

                model.Alias = item.Alias;
                model.Display = item.Display;

                embedded.Add(model);
            }

            return embedded;
        }

        private IEnumerable<ContentFieldViewModel> GetProperties(ApiCall call, ContentType contenttype)
        {
            var culture = call.WebSite.DefaultCulture;

            List<ContentFieldViewModel> result = new List<ContentFieldViewModel>();

            bool online = true;
            TextContent textcontentnew = null;

            if (call.ObjectId != default(Guid))
            {
                textcontentnew = call.WebSite.SiteDb().TextContent.Get(call.ObjectId);
                if (textcontentnew != default) online = textcontentnew.Online;
            }

            if (textcontentnew == null)
            {
                foreach (var item in contenttype.Properties)
                {
                    if (item.Editable)
                    {
                        ContentFieldViewModel model = new ContentFieldViewModel
                        {
                            Name = item.Name,
                            DisplayName = item.DisplayName,
                            Validations = item.Validations,
                            Settings = item.Settings,
                            ControlType = item.ControlType,
                            ToolTip = item.Tooltip,
                            Order = item.Order
                        };

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
                        model.selectionOptions = GetSelectionOptions(item, call.Context, null);
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
                        ContentFieldViewModel model = new ContentFieldViewModel
                        {
                            Name = item.Name,
                            Validations = item.Validations,
                            Settings = item.Settings,
                            ControlType = item.ControlType,
                            DisplayName = item.DisplayName,
                            IsMultilingual = item.MultipleLanguage,
                            ToolTip = item.Tooltip,
                            Order = item.Order,
                            selectionOptions = GetSelectionOptions(
                                item,
                                call.Context,
                                textcontentnew == null ? null : textcontentnew.Id.ToString()
                            ),
                            MultipleValue = item.MultipleValue
                        };

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

            return result.OrderBy(it => it.Order);
        }

        private string GetSelectionOptions(ContentProperty item, RenderContext context, string contentId)
        {
            var empty = "[]";
            if (ControlTypes.OptionControlTypes.Contains(item.ControlType, StringComparer.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(item.Settings))
                {
                    return item.selectionOptions;
                }

                var setting = JsonHelper.Deserialize<Dictionary<string, string>>(item.Settings);
                if (setting.TryGetValue("isDynamicOptions", out var isDynamicOptions) && "true".Equals(isDynamicOptions, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        if (!setting.TryGetValue("code", out var code) || string.IsNullOrWhiteSpace(code))
                        {
                            return empty;
                        }

                        var codeBlock = new TypescriptCode(code.ToHashGuid().ToString(), code);
                        var engine = Sites.Scripting.Manager.GetJsEngine(context);
                        if (contentId != default) engine.SetValue("self", contentId);
                        var result = engine.ExecuteCode(codeBlock, Data.ScriptType.Classic, false)?.ToObject();
                        if (result == null)
                        {
                            return empty;
                        }
                        var type = result.GetType();
                        if (type == typeof(string))
                        {
                            throw new ArgumentException(result.ToString());
                        }

                        var errorMessage = Hardcoded.GetValue("Options_Code_Should_Return_KeyValue_Array", context);
                        if (!type.IsArray)
                        {
                            throw new ArrayTypeMismatchException(errorMessage);
                        }

                        var array = (object[])result;
                        foreach (var entity in array)
                        {
                            if (entity is not IDictionary<string, object> obj)
                            {
                                throw new ArrayTypeMismatchException(errorMessage);
                            }

                            if (!obj.ContainsKey("key") || !obj.ContainsKey("value"))
                            {
                                throw new ArrayTypeMismatchException(errorMessage);
                            }
                        }

                        return JsonHelper.Serialize(result);
                    }
                    catch (Exception ex)
                    {
                        return JsonHelper.Serialize(new[] { new KeyValuePair<string, string>(ex.Message, string.Empty) });
                    }
                }
            }

            return item.selectionOptions;
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.VIEW)]
        public ContentEditViewModel GetEdit(ApiCall call)
        {
            var (TypeId, FolderId) = GetRelationId(call);
            var contenttype = call.WebSite.SiteDb().ContentTypes.Get(TypeId);

            ContentEditViewModel model = new()
            {
                FolderId = FolderId,
                Properties = GetProperties(call, contenttype).ToList()
            };

            if (FolderId != default)
            {
                model.Categories = GetCategoryInfo(
                               call,
                               FolderId,
                               call.ObjectId,
                               call.WebSite.DefaultCulture
                           );

                model.Embedded = GetEmbedContents(
                    call,
                    FolderId,
                    call.ObjectId,
                    call.WebSite.DefaultCulture
                );
            }

            return model;
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.VIEW)]
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

        private (Guid TypeId, Guid FolderId) GetRelationId(ApiCall call)
        {
            Guid typeId = call.GetValue<Guid>("typeId");
            if (typeId != default) return (typeId, Guid.Empty);

            Guid folderId = call.GetValue<Guid>("FolderId");
            if (folderId != default)
            {
                var folder = call.WebSite.SiteDb().ContentFolders.Get(folderId);
                if (folder != default) return (folder.ContentTypeId, folderId);
            }

            if (call.ObjectId != default)
            {
                var content = call.WebSite.SiteDb().TextContent.Get(call.ObjectId);
                if (content != default) return (content.ContentTypeId, content.FolderId);
            }

            throw new Exception("not found content relation");
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

        public PagedTextContentListViewModel Search(string Keyword, ApiCall call)
        {
            var folderId = call.GetValue("folderId");
            if (string.IsNullOrWhiteSpace(Keyword))
            {
                return new PagedTextContentListViewModel();
            }
            var siteDb = call.WebSite.SiteDb();

            ContentFolder folder = null;

            if (Guid.TryParse(folderId, out var id))
            {
                folder = siteDb.ContentFolders.Get(id);
            }
            else
            {
                folder = siteDb.ContentFolders.GetByName(folderId);
            }

            if (folder == null)
            {
                return new PagedTextContentListViewModel();
            }

            int pagesize = ApiHelper.GetPageSize(call, 50);
            int pagenr = ApiHelper.GetPageNr(call);

            string language = string.IsNullOrEmpty(call.Context.Culture)
                ? call.WebSite.DefaultCulture
                : call.Context.Culture;
            var columns = siteDb
                .ContentTypes
                .GetTitlePropertyByFolder(folder.Id)
                .OrderBy(it => it.Order)
                .Select(it => new BaseColumnViewModel
                {
                    Name = it.Name,
                    DisplayName = it.DisplayName,
                    ControlType = it.ControlType,
                    MultipleValue = it.MultipleValue,
                    IsSummaryField = it.IsSummaryField,
                });
            PagedTextContentListViewModel model = new()
            {
                Columns = columns,
                PageNr = pagenr,
                PageSize = pagesize,
                Categories = GetCategoriesOptions(call, folder),
            };

            var sortField = call.GetValue("sortField") ?? string.Empty;
            var ascending = call.GetValue("ascending")?.ToLower() == "true";
            var exclude = call.GetValue<Guid[]>("exclude") ?? [];

            var categoriesValue = call.GetValue("categories");
            var categories = string.IsNullOrEmpty(categoriesValue) ? null : JsonHelper.Deserialize<Dictionary<Guid, List<Guid>>>(categoriesValue);

            var textContents = siteDb.TextContent.GetSortedTextContentsByFolder(folder, true, sortField, ascending, categories)
                .Where(w => !exclude.Contains(w.Id))
                .Where(o => o.Body.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) > -1);

            model.TotalCount = textContents.Count();
            model.TotalPages = ApiHelper.GetPageCount(model.TotalCount, model.PageSize);

            var list = textContents
                .Skip(model.PageNr * model.PageSize - model.PageSize)
                .Take(model.PageSize)
                .ToList();

            model.List = ToTextContentViewModel(siteDb, folder.Id, list, language);

            return model;
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.DELETE)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            var siteDb = call.WebSite.SiteDb();
            var all = siteDb.TextContent.All();
            var parentId = call.GetGuidValue("parentId");
            return Delete(all, siteDb, call.ObjectId, parentId, call.Context.User.Id);
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            var siteDb = call.WebSite.SiteDb();
            string json = call.GetValue("ids");
            var parentId = call.GetGuidValue("parentId");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }
            List<Guid> ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);

            if (ids != null && ids.Count() > 0)
            {
                foreach (var item in ids)
                {
                    var all = siteDb.TextContent.All();
                    Delete(all, siteDb, item, parentId, call.Context.User.Id);
                }
                return true;
            }
            return false;
        }

        private bool Delete(List<TextContent> all, SiteDb siteDb, Guid id, Guid parentId, Guid userId)
        {
            var textContent = siteDb.TextContent.Get(id);

            if (textContent != default)
            {
                if (parentId == default)
                {
                    DeleteEmbedded(all, siteDb, textContent, userId);
                    return true;
                }
                else
                {
                    var useByOther = all.Where(w => w.Id != parentId).Any(a => a.Embedded.Any(aa => aa.Value?.Contains(textContent.Id) ?? false));

                    if (!useByOther)
                    {
                        DeleteEmbedded(all, siteDb, textContent, userId);
                        return true;
                    }
                }
            }

            return false;
        }

        private void DeleteEmbedded(List<TextContent> all, SiteDb siteDb, TextContent textContent, Guid userId)
        {
            if (textContent.Embedded != null && textContent.Embedded.Any())
            {
                foreach (var embedded in textContent.Embedded)
                {
                    if (embedded.Value == null)
                    {
                        continue;
                    }
                    foreach (var item in embedded.Value)
                    {
                        Delete(all, siteDb, item, textContent.Id, userId);
                    }
                }
            }

            siteDb.TextContent.Delete(textContent.Id, userId);
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            return base.List(call);
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.CONTENT, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }

        public record GetByIdsResult(BaseColumnViewModel[] Columns, TextContentViewModel[] List);
        public GetByIdsResult GetByIds(ApiCall call, string folderId, Guid[] ids)
        {
            var siteDb = call.WebSite.SiteDb();
            var folder = siteDb.ContentFolders.Get(folderId);
            if (folder == default) return new GetByIdsResult([], []);
            var columns = siteDb
                .ContentTypes
                .GetTitlePropertyByContentType(folder.ContentTypeId)
                .OrderBy(it => it.Order)
                .Select(it => new BaseColumnViewModel
                {
                    Name = it.Name,
                    DisplayName = it.DisplayName,
                    ControlType = it.ControlType,
                    MultipleValue = it.MultipleValue,
                    IsSummaryField = it.IsSummaryField,
                }).ToArray();

            var list = new List<TextContentViewModel>();
            var lang = call.WebSite.DefaultCulture;
            foreach (var id in ids ?? [])
            {
                var content = siteDb.TextContent.GetView(id, lang);
                if (content != default) list.Add(content);
            }
            return new GetByIdsResult(columns, [.. list]);
        }
    }
}