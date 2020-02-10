//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Helper
{
    public static class ContentHelper
    {
        public static TextContentViewModel ToView(TextContent Content, string lang, List<ContentProperty> Properties)
        {
            if (Content == null)
            {
                return null;
            }
            TextContentViewModel model = new TextContentViewModel();
            model.Id = Content.Id;
            model.ParentId = Content.ParentId;
            model.FolderId = Content.FolderId;
            model.ContentTypeId = Content.ContentTypeId;
            model.UserKey = Content.UserKey;
            model.LastModified = Content.LastModified;
            model.Order = Content.Order;
            model.Online = Content.Online;
            model.Embedded = Content.Embedded;
            model.CreationDate = Content.CreationDate;

            var langcontent = Content.GetContentStore(lang);
            if (langcontent != null)
            {
                model.TextValues = langcontent.FieldValues;
            }

            if (Properties != null)
            {
                foreach (var item in Properties.Where(o => !o.IsSystemField && !o.MultipleLanguage))
                {
                    if (!model.TextValues.ContainsKey(item.Name) || string.IsNullOrEmpty(model.TextValues[item.Name]))
                    {
                        bool found = false;
                        foreach (var citem in Content.Contents)
                        {
                            foreach (var fielditem in citem.FieldValues)
                            {
                                if (fielditem.Key == item.Name)
                                {
                                    model.TextValues[item.Name] = fielditem.Value;
                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                            { break; }
                        }
                    }
                }
            }

            return model;
        }

        public static TextContentViewModel ToListDisplayView(TextContent Content, ContentType ContentType, string lang = null)
        {
            Dictionary<string, string> displayFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var fields = ContentType.Properties.FindAll(o => o.IsSummaryField && !o.IsSystemField);
            if (fields == null || fields.Count() == 0)
            {
                fields = ContentType.Properties.FindAll(o => !o.IsSystemField && o.DataType == Data.Definition.DataTypes.String);
            }

            if (fields == null || fields.Count() == 0)
            {
                fields = ContentType.Properties.FindAll(o => !o.IsSystemField);
            }

            foreach (var item in fields)
            {
                if (!displayFields.ContainsKey(item.Name))
                {
                    displayFields.Add(item.Name, item.DisplayName);
                }
            } 


            if (Content == null)
            {
                return null;
            }
            TextContentViewModel model = new TextContentViewModel();
            model.Id = Content.Id;
            model.ParentId = Content.ParentId;
            model.FolderId = Content.FolderId;
            model.ContentTypeId = Content.ContentTypeId;
            model.UserKey = Content.UserKey;
            model.LastModified = Content.LastModified;
            model.Order = Content.Order;
            model.Online = Content.Online;
            model.Embedded = Content.Embedded;
            model.CreationDate = Content.CreationDate;

            var content = Content.GetContentStore(lang);
            if (content != null)
            {
                foreach (var item in content.FieldValues)
                {
                    if (displayFields.ContainsKey(item.Key))
                    {
                        var displayname = displayFields[item.Key];

                        model.TextValues[displayname] = item.Value;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Content.UserKey))
                {
                    var userKeyField = ContentType.Properties.Find(o => o.Name == "UserKey");
                    model.TextValues[userKeyField.DisplayName] = Content.UserKey;
                }
            }
            return model;
        }

        public static List<TextContentViewModel> ToViews(List<TextContent> Contents, string lang, List<ContentProperty> Properties)
        {
            if (Contents == null || Contents.Count() == 0)
            {
                return new List<TextContentViewModel>();
            }

            List<TextContentViewModel> result = new List<TextContentViewModel>();

            foreach (var item in Contents)
            {
                result.Add(ToView(item, lang, Properties));
            }
            return result;
        }

        public static string GetSummary(TextContent content, string lang = null)
        {
            var view = ToView(content, lang, null);

            string text = string.Empty;

            foreach (var item in view.TextValues)
            {
                text += ShowText(item.Value) + " ";
                if (!string.IsNullOrEmpty(text) && text.Length > 150)
                {
                    return text.Substring(0, 150) + "...";
                }
            }
            return text;
        }

        public static List<EmbeddedContentViewModel> GetEmbedContents(SiteDb sitedb, Guid folderId, Guid textContentId, string language = null)
        {
            List<EmbeddedContentViewModel> embedded = new List<EmbeddedContentViewModel>();
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

                List<Guid> ids = new List<Guid>();
                if (content.Embedded.ContainsKey(item.FolderId))
                {
                    var currentids = content.Embedded[item.FolderId];
                    if (currentids != null && currentids.Count > 0)
                    {
                        ids = currentids;
                    }
                }

                EmbeddedContentViewModel model = new EmbeddedContentViewModel();
                model.EmbeddedFolder = embeddedFolder;

                var contents = sitedb.TextContent.Query.
                            Where(it => it.FolderId == item.FolderId)
                            .WhereIn("Id", ids)
                            .SelectAll();

                // also display use the parent ids. 
                if (textContentId != default(Guid))
                {
                    var subcontents = sitedb.TextContent.Query.Where(o => o.FolderId == item.FolderId && o.ParentId == textContentId).SelectAll();

                    foreach (var subitem in subcontents)
                    {
                        if (contents.Find(o => o.Id == subitem.Id) == null)
                        {
                            contents.Add(subitem);
                        }
                    }
                }
                model.Contents = contents.Select(o => ToView(o, language, sitedb.ContentTypes.GetTitlePropertyByFolder(item.FolderId))).ToList();

                CleanNonSummaryFields(sitedb, model, item.FolderId); 

                model.Alias = item.Alias;

                embedded.Add(model);
            }

            return embedded;
        }

        private static void CleanNonSummaryFields(SiteDb sitedb, EmbeddedContentViewModel model, Guid FolderId)
        {
            var summaryfield = sitedb.ContentTypes.GetTitlePropertyByFolder(FolderId);

            if (summaryfield !=null && summaryfield.Any())
            {
                foreach (var item in model.Contents)
                {
                    List<string> keysToRemove = new List<string>();
                    foreach (var value in item.TextValues)
                    {
                        var find = summaryfield.Find(o => o.Name == value.Key); 
                        if (find == null)
                        {
                            keysToRemove.Add(value.Key); 
                        }
                    }

                    foreach (var key in keysToRemove)
                    {
                        item.TextValues.Remove(key); 
                    }
                }
            }
             
        }


        public static string ShowText(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            var dom = Kooboo.Dom.DomParser.CreateDom(input);
            StringBuilder sb = new StringBuilder();
            _appendtext(dom.documentElement, ref sb);

            string value = sb.ToString();
            return value;
        }

        private static void _appendtext(Node node, ref StringBuilder sb)
        {
            if (node.nodeType == enumNodeType.TEXT)
            {
                var text = node as Kooboo.Dom.Text;
                sb.Append(text.data + " ");
            }
            else if (node.nodeType == enumNodeType.ELEMENT)
            {
                var el = node as Element;
                foreach (var item in el.childNodes.item)
                {
                    _appendtext(item, ref sb);
                }
            }
        }

        public static List<Element> GetByTextContentMedia(Document doc, string url)
        {
            List<Element> result = new List<Element>();
            if (string.IsNullOrWhiteSpace(url))
            {
                return result;
            }

            _GetByTextContentMedia(doc.documentElement, url.ToLower().Trim(), ref result);

            return result;
        }

        private static void _GetByTextContentMedia(Element el, string Url, ref List<Element> result)
        {
            if (OnlyTextSubNode(el))
            {
                var link = el.InnerHtml;
                if (!string.IsNullOrEmpty(link))
                {
                    link = link.ToLower().Trim();

                    if (link == Url)
                    {
                        result.Add(el);
                    }
                    else
                    {
                        if (link.StartsWith("[") && link.EndsWith("]"))
                        {
                            // possible json, multiitem media. 
                            try
                            {
                                List<string> files = Lib.Helper.JsonHelper.Deserialize<List<string>>(link);

                                if (files != null && files.Count() > 0)
                                {
                                    foreach (var item in files)
                                    {
                                        if (!string.IsNullOrWhiteSpace(item) && item.ToLower().Trim() == Url)
                                        {
                                            result.Add(el);
                                        }
                                    }
                                }

                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                }
            }
            else
            {
                foreach (var item in el.childNodes.item)
                {
                    if (item.nodeType == enumNodeType.ELEMENT)
                    {
                        var subel = item as Element;
                        _GetByTextContentMedia(subel, Url, ref result);
                    }
                }
            }





        }

        private static bool OnlyTextSubNode(Element el)
        {
            foreach (var item in el.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    return false;
                }
            }
            return true;
        }

        public static string GetTitle(SiteDb sitedb, TextContentViewModel contentview)
        {
            if (contentview == null || !contentview.TextValues.Any())
            {
                return string.Empty;
            }

            Guid contentTypeId = contentview.ContentTypeId;
            if (contentTypeId == default(Guid))
            {
                var folder = sitedb.ContentFolders.Get(contentview.FolderId);
                if (folder != null)
                {
                    contentTypeId = folder.ContentTypeId;
                }
            }

            if (contentTypeId != default(Guid))
            {
                var columns = sitedb.ContentTypes.GetTitleColumns(contentTypeId);
                if (columns != null && columns.Any())
                {
                    string title = string.Empty;

                    foreach (var item in columns)
                    {
                        var value = contentview.GetValue(item);
                        title += value + " ";
                    }

                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        return title.Trim();
                    }
                }
            }
            return contentview.TextValues.First().Value;
        }

    }
}
