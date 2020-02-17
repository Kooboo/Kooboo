using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class HtmlblockModifier : ModifierBase
    {
        public override string Source => "htmlblock";

        public override void Modify(RenderContext context)
        {
            var repo = context.WebSite.SiteDb().GetRepository(Source);
            if (repo == null) return;
            var domObject = repo.GetByNameOrId(Id) as HtmlBlock;
            if (domObject == null) return;
            var dom = domObject.GetValue(context.Culture).ToString();
            var doc = DomParser.CreateDom(dom);
            var node = Service.DomService.GetElementByKoobooId(doc, KoobooId);
            if (node == null) return;
            var element = node as Element;
            if (element == null) return;
        }
    }
}
