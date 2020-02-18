using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class MenuModifier : ModifierBase
    {
        public override string Source => "menu";

        public override void Modify(RenderContext context)
        {
            var repo = context.WebSite.SiteDb().Menus;
            if (repo == null) return;
            var menu = repo.GetByNameOrId(Id) as Menu;
            if (menu == null) return;
            //var dom = menu.
            //var doc = DomParser.CreateDom(dom);
            //var node = Service.DomService.GetElementByKoobooId(doc, KoobooId);
            //if (node == null) return;
            //var element = node as Element;
            //if (element == null) return;
            //var newBody = GetNewDomBody(dom, element);
            //menu.SetValue(context.Culture, newBody);
            //repo.AddOrUpdate(menu, context.User.Id);
        }
    }
}
