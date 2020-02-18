using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    class PageModifier : ModifierBase
    {
        public override string Source => "page";

        public override void Modify(RenderContext context)
        {
            if (Id == null || KoobooId == null) return;
            var repo = context.WebSite.SiteDb().Pages;
            if (repo == null) return;
            var page = repo.GetByNameOrId(Id) as Page;
            if (page == null) return;
            var doc = DomParser.CreateDom(page.Body);
            var node = Service.DomService.GetElementByKoobooId(doc, KoobooId);
            if (node == null) return;
            var element = node as Element;
            if (element == null) return;
            page.Body = GetNewDomBody(page.Body, element);
            repo.AddOrUpdate(page, context.User.Id);

            Task.Run(() =>
            {
                var otherPages = context.WebSite.SiteDb().Pages.All()
               .Where(o => o.Id != page.Id && o.HasLayout == false)
               .ToList();

                foreach (var otherPage in otherPages)
                {
                    var sameBlock = Helper.ElementHelper.FindSameElement(element, otherPage.Dom);

                    if (sameBlock != null)
                    {
                        otherPage.Body = GetNewDomBody(otherPage.Body, sameBlock);
                        repo.AddOrUpdate(otherPage, context.User.Id);
                    }
                }
            });
        }
    }
}
