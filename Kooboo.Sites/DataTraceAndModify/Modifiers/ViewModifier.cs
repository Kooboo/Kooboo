using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class ViewModifier : ModifierBase
    {
        public override string Source => "view";

        public override void Modify(RenderContext context)
        {
            if (Id == null || KoobooId == null) return;
            var repo = context.WebSite.SiteDb().Views;
            if (repo == null) return;
            var view = repo.GetByNameOrId(Id) as View;
            if (view == null) return;
            var doc = DomParser.CreateDom(view.Body);
            var node = Service.DomService.GetElementByKoobooId(doc, KoobooId);
            if (node == null) return;
            var element = node as Element;
            if (element == null) return;
            view.Body = GetNewDomBody(view.Body, element);
            repo.AddOrUpdate(view, context.User.Id);
        }
    }
}