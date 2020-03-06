using dotless.Core.Parser.Tree;
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Dom.CSS;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    class PageModifier : DomModifier
    {
        public override string Source => "page";

        internal override void UpdateSiteObject(RenderContext context)
        {
            var repo = GetRepo(context);
            var domObject = GetDomObject(repo);
            var element = GetElement(domObject);
            HandleUpdate(context, repo, domObject, element);

            if (element == null) return;

            Task.Run(() =>
            {
                var otherPages = context.WebSite.SiteDb().Pages.All()
               .Where(o => o.Id != domObject.Id && o.HasLayout == false)
               .ToList();

                foreach (var otherPage in otherPages)
                {
                    var sameBlock = Helper.ElementHelper.FindSameElement(element, otherPage.Dom);

                    if (sameBlock != null)
                    {
                        HandleUpdate(context, repo, otherPage, sameBlock);
                    }
                }
            });
        }

        public override void Modify(RenderContext context)
        {
            UpdateSiteObject(context);
        }

    }
}
