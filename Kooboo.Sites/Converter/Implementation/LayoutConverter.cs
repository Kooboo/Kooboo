//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Models; 

namespace Kooboo.Sites.Converter
{
    public class LayoutConverter
    {
        public static string ConvertLayout(SiteDb SiteDb, Guid PageId, List<LayoutResult> Results)
        {
            var page = SiteDb.Pages.Get(PageId);
            if (page == null)
            {
                return null;
            }

            var dom = Kooboo.Dom.DomParser.CreateDom(page.Body);

            var updates = new List<SourceUpdate>();

            foreach (var item in Results)
            {
                if (item.KoobooIds == null || item.KoobooIds.Count == 0)
                {
                    continue;
                }

                if (item.IsContainer)
                {
                    var koobooid = item.KoobooIds[0];
                    if (!string.IsNullOrEmpty(koobooid))
                    {
                        var node = Service.DomService.GetElementByKoobooId(dom, koobooid);

                        if (node != null && node.nodeType == enumNodeType.ELEMENT)
                        {
                            var element = node as Element;
                            element.setAttribute(ConstTALAttributes.placeholder, item.Name);

                            string newtag = Service.DomService.ReSerializeElement(element, "");

                            updates.Add(new SourceUpdate { StartIndex = element.location.openTokenStartIndex, EndIndex = element.location.endTokenEndIndex, NewValue = newtag });

                        }
                    }
                }
                else
                {
                    var koobooids = item.KoobooIds;
                    int startindex = int.MaxValue;
                    int endindex = int.MinValue;

                    foreach (var id in koobooids)
                    {
                        var node = Service.DomService.GetElementByKoobooId(dom, id);

                        if (node != null && node.nodeType == enumNodeType.ELEMENT)
                        {
                            var element = node as Element;

                            if (element.location.openTokenStartIndex < startindex)
                            {
                                startindex = element.location.openTokenStartIndex;
                            }

                            if (element.location.endTokenEndIndex > endindex)
                            {
                                endindex = element.location.endTokenEndIndex;
                            }

                        }
                    }

                    if (startindex < int.MaxValue && endindex > int.MinValue)
                    {
                        var layouttag = "<div " + ConstTALAttributes.placeholder + "='" + item.Name + "' " + ConstTALAttributes.omitTag + "></div>";

                        var update = new SourceUpdate();
                        update.StartIndex = startindex;
                        update.EndIndex = endindex;
                        update.NewValue = layouttag;

                        updates.Add(update);
                    }
                }
            }

            if (updates.Any())
            {
                string layoutbody = ConvertManager.UpdateDomSource(dom, updates);
                var name = GetLayoutName(SiteDb);
                Layout layout = new Layout();
                layout.Name = name;
                layout.Body = layoutbody;
                SiteDb.Layouts.AddOrUpdate(layout); 
            }
            return null;
        }
         
        private static string GetLayoutName(SiteDb sitedb, string currentName="")
        {
            if (string.IsNullOrEmpty(currentName))
            {
                currentName = "layout"; 
            }
             
            var existing = sitedb.Layouts.GetByNameOrId(currentName); 
            if (existing == null)
            { return currentName;  }

            for (int i =  1; i < 999; i++)
            {
                var newname = currentName + i.ToString();
                existing = sitedb.Layouts.GetByNameOrId(newname); 
                if (existing == null)
                {
                    return newname;  
                }
            }

            return null; 
        }
  
    }


    public class LayoutResult
    {
        public List<string> KoobooIds;
        public bool IsContainer;
        public string Name;
    }




}
