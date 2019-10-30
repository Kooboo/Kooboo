//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.InlineEditor.Converter
{
    public class ConvertManager
    {
        public static string UpdateDomSource(Document doc, List<SourceUpdate> updates)
        {
            int currentindex = 0;
            int totallen = doc.HtmlSource.Length;
            StringBuilder sb = new StringBuilder();

            // update to real html source.
            foreach (var item in updates.OrderBy(o => o.StartIndex))
            {
                string currentString = doc.HtmlSource.Substring(currentindex, item.StartIndex - currentindex);
                if (!string.IsNullOrEmpty(currentString))
                {
                    sb.Append(currentString);
                }
                sb.Append(item.NewValue);

                currentindex = item.EndIndex;
            }

            if (currentindex < totallen - 1)
            {
                sb.Append(doc.HtmlSource.Substring(currentindex, totallen - currentindex));
            }

            return sb.ToString();
        }

        public static void ConvertComponent(RenderContext context, List<JObject> convertResult)
        {
            var page = context.GetItem<Page>();

            List<ConvertSourceUpdate> changes = new List<ConvertSourceUpdate>();

            var converters = GetConverters();
            foreach (var item in convertResult)
            {
                string converttype = Lib.Helper.JsonHelper.GetString(item, "ConvertToType");

                if (!string.IsNullOrEmpty(converttype))
                {
                    var converter = converters.Find(o => o.Type == converttype.ToLower());
                    if (converter != null)
                    {
                        var response = converter.Convert(context, item);

                        string koobooid = null;
                        koobooid = !string.IsNullOrEmpty(response.KoobooId) ? response.KoobooId : Lib.Helper.JsonHelper.GetString(item, "KoobooId");
                        var element = Service.DomService.GetElementByKoobooId(page.Dom, koobooid);

                        changes.Add(new ConvertSourceUpdate
                        {
                            PageId = page.Id,
                            StartIndex = element.location.openTokenStartIndex,
                            EndIndex = element.location.endTokenEndIndex,
                            NewValue = response.Tag
                        });
                    }
                }
            }

            UpdateChanges(context, changes);
        }

        public static void UpdateChanges(RenderContext context, List<ConvertSourceUpdate> updates)
        {
            if (updates.Count == 0)
            {
                return;
            }

            var sitedb = context.WebSite.SiteDb();

            foreach (var item in updates.GroupBy(o => o.PageId))
            {
                var pageid = item.Key;
                var convertsourceupdates = item.ToList();

                var page = context.WebSite.SiteDb().Pages.Get(pageid);

                if (page != null)
                {
                    var htmlbody = page.Body;
                    if (!string.IsNullOrEmpty(htmlbody))
                    {
                        string newbody = UpdatePageChange(htmlbody, item.ToList());
                        page.Body = newbody;
                        sitedb.Pages.AddOrUpdate(page);
                    }
                }
            }
        }

        public static string UpdatePageChange(string pageHtmlSource, List<ConvertSourceUpdate> updates)
        {
            List<SourceUpdate> sourceupdates = new List<SourceUpdate>();
            foreach (var one in updates)
            {
                SourceUpdate newupdate = new SourceUpdate
                {
                    StartIndex = one.StartIndex, EndIndex = one.EndIndex, NewValue = one.NewValue
                };
                if (newupdate.StartIndex > 0 && newupdate.EndIndex > 0)
                {
                    sourceupdates.Add(newupdate);
                }
            }

            if (!string.IsNullOrEmpty(pageHtmlSource))
            {
                return Service.DomService.UpdateSource(pageHtmlSource, sourceupdates);
            }

            return null;
        }

        private static List<IConverter> _converters;

        public static List<IConverter> GetConverters()
        {
            if (_converters == null)
            {
                _converters = new List<IConverter>
                {
                    new MenuConverter(), new ViewConverter(), new HtmlBlockConverter(), new ContentListConverter()
                };
            }
            return _converters;
        }

        public static string GetUniqueName(RenderContext context, string convertType, string name)
        {
            if (string.IsNullOrEmpty(convertType))
            { convertType = name; }
            convertType = convertType.ToLower();

            var sitedb = context.WebSite.SiteDb();

            switch (convertType)
            {
                case "htmlblock":
                    {
                        var block = sitedb.HtmlBlocks.GetByNameOrId(name);
                        if (block == null)
                        {
                            return name;
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            name += i.ToString();
                            block = sitedb.HtmlBlocks.GetByNameOrId(name);
                            if (block == null)
                            {
                                return name;
                            }
                        }

                        name += System.Guid.NewGuid();

                        return name;
                    }

                case "view":
                    {
                        var block = sitedb.Views.GetByNameOrId(name);
                        if (block == null)
                        {
                            return name;
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            name += i.ToString();
                            block = sitedb.Views.GetByNameOrId(name);
                            if (block == null)
                            {
                                return name;
                            }
                        }

                        name += System.Guid.NewGuid();

                        return name;
                    }

                case "contentlist":
                    {
                        var block = sitedb.Views.GetByNameOrId(name);
                        if (block == null)
                        {
                            return name;
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            name += i.ToString();
                            block = sitedb.Views.GetByNameOrId(name);
                            if (block == null)
                            {
                                return name;
                            }
                        }

                        name += System.Guid.NewGuid();

                        return name;
                    }

                case "category":
                    {
                        var block = sitedb.Views.GetByNameOrId(name);
                        if (block == null)
                        {
                            return name;
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            name += i.ToString();
                            block = sitedb.Views.GetByNameOrId(name);
                            if (block == null)
                            {
                                return name;
                            }
                        }

                        name += System.Guid.NewGuid();

                        return name;
                    }

                case "menu":
                    {
                        var block = sitedb.Menus.GetByNameOrId(name);
                        if (block == null)
                        {
                            return name;
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            name += i.ToString();
                            block = sitedb.Menus.GetByNameOrId(name);
                            if (block == null)
                            {
                                return name;
                            }
                        }

                        name += System.Guid.NewGuid();

                        return name;
                    }

                default:
                    break;
            }

            return null;
        }
    }
}