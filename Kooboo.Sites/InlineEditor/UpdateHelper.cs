//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.InlineEditor
{
    public static class UpdateHelper
    {
        //public static string Update(string source, List<InlineSourceUpdate> updates)
        //{
        //    List<SourceUpdate> sourceupdates = new List<SourceUpdate>();
        //    var doc = Kooboo.Dom.DomParser.CreateDom(source);

        //    foreach (var item in updates.GroupBy(o => o.KoobooId))
        //    {
        //        string koobooid = item.Key;
        //        var node = Service.DomService.GetElementByKoobooId(doc, koobooid);
        //        if (node != null)
        //        {
        //            var element = node as Element;
        //            var kooboolist = item.ToList();
        //            if (element != null && kooboolist != null && kooboolist.Count() > 0)
        //            {
        //                var koobooupdate = GetElementUpdate(doc, element, item.ToList());
        //                sourceupdates.AddRange(koobooupdate);
        //            }
        //        }
        //    }

        //    return Service.DomService.UpdateSource(source, sourceupdates);
        //}

        //private static List<SourceUpdate> GetElementUpdate(Document doc, Element element, List<InlineSourceUpdate> KoobooIdUpdates)
        //{
        //    List<SourceUpdate> sourceupdates = new List<SourceUpdate>();
        //    bool HasAttChange = false;
        //    foreach (var item in KoobooIdUpdates)
        //    {
        //        if (item.IsDelete)
        //        {
        //            int start = element.location.openTokenStartIndex;
        //            int end = element.location.endTokenEndIndex;
        //            if (end < start)
        //            {
        //                end = -1;
        //            }
        //            sourceupdates.Add(new SourceUpdate()
        //            {
        //                StartIndex = start,
        //                EndIndex = end,
        //                NewValue = ""
        //            });
        //        }
        //        else
        //        {
        //            if (string.IsNullOrEmpty(item.AttributeName))
        //            {
        //                int start = element.location.openTokenEndIndex + 1;
        //                int end = element.location.endTokenStartIndex - 1;
        //                if (end < start)
        //                {
        //                    end = -1;
        //                }
        //                sourceupdates.Add(new SourceUpdate()
        //                {
        //                    StartIndex = start,
        //                    EndIndex = end,
        //                    NewValue = item.Value
        //                });
        //            }
        //            else
        //            {
        //                HasAttChange = true;
        //                element.setAttribute(item.AttributeName, item.Value);
        //            }
        //        }
        //    }

        //    if (HasAttChange)
        //    {
        //        SourceUpdate update = new SourceUpdate();
        //        update.StartIndex = element.location.openTokenStartIndex;
        //        update.EndIndex = element.location.openTokenEndIndex;
        //        update.NewValue = Kooboo.Sites.Service.DomService.ReSerializeOpenTag(element);
        //        sourceupdates.Add(update);
        //    }
        //    return sourceupdates;
        //}

        public static string UpdateOrAppendInlineCss(string InlineCss, List<Kooboo.Dom.CSS.CSSDeclaration> NewDeclarations)
        {
            var currentblock = Kooboo.Dom.CSS.CSSSerializer.deserializeDeclarationBlock(InlineCss);
            var CurrentItems = currentblock.item;

            foreach (var item in NewDeclarations)
            {
                var exist = CurrentItems.Find(o => o.propertyname.ToLower() == item.propertyname.ToLower());
                if (exist == null)
                {
                    CurrentItems.Add(item);
                }
                else
                {
                    exist.value = item.value;
                    exist.important = item.important;
                    CurrentItems.Remove(exist);
                    CurrentItems.Add(exist);
                }
            }

            currentblock.item = currentblock.item.Where(o => !string.IsNullOrWhiteSpace(o.propertyname)).ToList();
            currentblock.item = currentblock.item.Where(o => !string.IsNullOrWhiteSpace(o.value)).ToList();

            return Kooboo.Dom.CSS.CSSSerializer.serializeDeclarationBlock(currentblock);

        }
    }

}
