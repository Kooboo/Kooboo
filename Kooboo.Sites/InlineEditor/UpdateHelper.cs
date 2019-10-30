//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.InlineEditor
{
    public static class UpdateHelper
    {
        public static string Update(string source, List<InlineSourceUpdate> updates)
        {
            List<SourceUpdate> sourceupdates = new List<SourceUpdate>();
            var doc = Kooboo.Dom.DomParser.CreateDom(source);

            foreach (var item in updates.GroupBy(o => o.KoobooId))
            {
                string koobooid = item.Key;
                var node = Service.DomService.GetElementByKoobooId(doc, koobooid);
                if (node != null)
                {
                    var element = node as Element;
                    var kooboolist = item.ToList();
                    if (element != null && kooboolist != null && kooboolist.Any())
                    {
                        var koobooupdate = GetElementUpdate(doc, element, item.ToList());
                        sourceupdates.AddRange(koobooupdate);
                    }
                }
            }

            return Service.DomService.UpdateSource(source, sourceupdates);
        }

        private static List<SourceUpdate> GetElementUpdate(Document doc, Element element, List<InlineSourceUpdate> koobooIdUpdates)
        {
            List<SourceUpdate> sourceupdates = new List<SourceUpdate>();
            bool hasAttChange = false;
            foreach (var item in koobooIdUpdates)
            {
                if (item.IsDelete)
                {
                    int start = element.location.openTokenStartIndex;
                    int end = element.location.endTokenEndIndex;
                    if (end < start)
                    {
                        end = -1;
                    }
                    sourceupdates.Add(new SourceUpdate()
                    {
                        StartIndex = start,
                        EndIndex = end,
                        NewValue = ""
                    });
                }
                else
                {
                    if (string.IsNullOrEmpty(item.AttributeName))
                    {
                        int start = element.location.openTokenEndIndex + 1;
                        int end = element.location.endTokenStartIndex - 1;
                        if (end < start)
                        {
                            end = -1;
                        }
                        sourceupdates.Add(new SourceUpdate()
                        {
                            StartIndex = start,
                            EndIndex = end,
                            NewValue = item.Value
                        });
                    }
                    else
                    {
                        hasAttChange = true;
                        element.setAttribute(item.AttributeName, item.Value);
                    }
                }
            }

            if (hasAttChange)
            {
                SourceUpdate update = new SourceUpdate
                {
                    StartIndex = element.location.openTokenStartIndex,
                    EndIndex = element.location.openTokenEndIndex,
                    NewValue = Kooboo.Sites.Service.DomService.ReSerializeOpenTag(element)
                };
                sourceupdates.Add(update);
            }
            return sourceupdates;
        }

        public static string UpdateOrAppendInlineCss(string inlineCss, List<Kooboo.Dom.CSS.CSSDeclaration> newDeclarations)
        {
            var currentblock = Kooboo.Dom.CSS.CSSSerializer.deserializeDeclarationBlock(inlineCss);
            var currentItems = currentblock.item;

            foreach (var item in newDeclarations)
            {
                var exist = currentItems.Find(o => o.propertyname.ToLower() == item.propertyname.ToLower());
                if (exist == null)
                {
                    currentItems.Add(item);
                }
                else
                {
                    exist.value = item.value;
                    exist.important = item.important;
                    currentItems.Remove(exist);
                    currentItems.Add(exist);
                }
            }

            currentblock.item = currentblock.item.Where(o => !string.IsNullOrWhiteSpace(o.propertyname)).ToList();
            currentblock.item = currentblock.item.Where(o => !string.IsNullOrWhiteSpace(o.value)).ToList();

            return Kooboo.Dom.CSS.CSSSerializer.serializeDeclarationBlock(currentblock);
        }
    }
}