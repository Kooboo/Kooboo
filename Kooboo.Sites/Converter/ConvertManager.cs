//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Converter
{
  public  class ConvertManager
    {
        public static string UpdateDomSource(Document doc, List<SourceUpdate> Updates)
        {
            int currentindex = 0;
            int totallen = doc.HtmlSource.Length;
            StringBuilder sb = new StringBuilder();

            // update to real html source. 
            foreach (var item in Updates.OrderBy(o => o.StartIndex))
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

        public static void ConvertComponent(RenderContext context, List<JObject> ConvertResult)
        {
            var page = context.GetItem<Page>(); 

            List<ConvertSourceUpdate> changes = new List<ConvertSourceUpdate>(); 
             
            var converters = GetConverters(); 
            foreach (var item in ConvertResult)
            {
                string converttype = Lib.Helper.JsonHelper.GetString(item, "ConvertToType");
            
                if (!string.IsNullOrEmpty(converttype))
                { 
                    var converter = converters.Find(o => o.Type == converttype.ToLower());
                    if (converter != null)
                    {
                        var response = converter.Convert(context, item);

                        string koobooid = null; 
                        if (!string.IsNullOrEmpty(response.KoobooId))
                        {
                            koobooid = response.KoobooId; 
                        }
                        else
                        {
                            koobooid = Lib.Helper.JsonHelper.GetString(item, "KoobooId");
                        } 
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

        public static string UpdatePageChange(string PageHtmlSource, List<ConvertSourceUpdate> updates)
        { 
            List<SourceUpdate> sourceupdates = new List<SourceUpdate>();
            foreach (var one in updates)
            {
                SourceUpdate newupdate = new SourceUpdate();
                newupdate.StartIndex = one.StartIndex;
                newupdate.EndIndex = one.EndIndex;
                newupdate.NewValue = one.NewValue;
                if (newupdate.StartIndex > 0 && newupdate.EndIndex > 0)
                {
                    sourceupdates.Add(newupdate);
                }
            }
             
                if (!string.IsNullOrEmpty(PageHtmlSource))
                {
                   return Service.DomService.UpdateSource(PageHtmlSource, sourceupdates);
               
                }

            return null;
            
        }

        private static List<IConverter> _converters; 
        public static List<IConverter> GetConverters()
        {
             if (_converters == null)
            {
                _converters = new List<IConverter>();
                _converters.Add(new MenuConverter());
                _converters.Add(new ViewConverter());
                _converters.Add(new HtmlBlockConverter());
                _converters.Add(new ContentListConverter());
            }
            return _converters; 
        }
        
        public static string GetUniqueName(RenderContext Context, string ConvertType, string Name)
        { 
          if (string.IsNullOrEmpty(ConvertType))
            {   ConvertType = Name;  }
            ConvertType = ConvertType.ToLower();

            var sitedb = Context.WebSite.SiteDb(); 

            switch (ConvertType)
            {
                case "htmlblock":
                    {
                        var block = sitedb.HtmlBlocks.GetByNameOrId(Name); 
                        if (block == null)
                        {
                            return Name; 
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            Name = Name + i.ToString();
                            block = sitedb.HtmlBlocks.GetByNameOrId(Name);
                            if (block == null)
                            {
                                return Name;
                            }
                        }

                        Name = Name + System.Guid.NewGuid();

                        return Name; 
                    }

                case "view":
                    {
                        var block = sitedb.Views.GetByNameOrId(Name);
                        if (block == null)
                        {
                            return Name;
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            Name = Name + i.ToString();
                            block = sitedb.Views.GetByNameOrId(Name);
                            if (block == null)
                            {
                                return Name;
                            }
                        }

                        Name = Name + System.Guid.NewGuid();

                        return Name;  
                    }

                case "contentlist":
                    {
                        var block = sitedb.Views.GetByNameOrId(Name);
                        if (block == null)
                        {
                            return Name;
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            Name = Name + i.ToString();
                            block = sitedb.Views.GetByNameOrId(Name);
                            if (block == null)
                            {
                                return Name;
                            }
                        }

                        Name = Name + System.Guid.NewGuid();

                        return Name;
                    }


                case "category":
                    {
                        var block = sitedb.Views.GetByNameOrId(Name);
                        if (block == null)
                        {
                            return Name;
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            Name = Name + i.ToString();
                            block = sitedb.Views.GetByNameOrId(Name);
                            if (block == null)
                            {
                                return Name;
                            }
                        }

                        Name = Name + System.Guid.NewGuid();

                        return Name;
                    }


                case "menu":
                    {
                        var block = sitedb.Menus.GetByNameOrId(Name);
                        if (block == null)
                        {
                            return Name;
                        }

                        for (int i = 0; i < 999; i++)
                        {
                            Name = Name + i.ToString();
                            block = sitedb.Menus.GetByNameOrId(Name);
                            if (block == null)
                            {
                                return Name;
                            }
                        }

                        Name = Name + System.Guid.NewGuid();

                        return Name;
                    }


                default:
                    break;
            }

            return null;
        }
    }
}
