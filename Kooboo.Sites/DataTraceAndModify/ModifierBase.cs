using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify
{


    public abstract class ModifierBase
    {
        public enum ActionType
        {
            update,
            delete,
            copy
        }

        public abstract string Source { get; }

        public ActionType Action
        {
            get
            {
                switch (GetValue("action"))
                {
                    case "delete":
                        return ActionType.delete;
                    case "copy":
                        return ActionType.copy;
                    default:
                        return ActionType.update;
                }
            }
        }

        public KeyValuePair<string, string>[] Infos { get; set; }

        public string KoobooId => GetValue("koobooid");

        public string Value => GetValue("value");

        public string Id => GetValue("id");

        public void HandleDom(RenderContext context)
        {
            if (Id == null || KoobooId == null) return;
            var repo = context.WebSite.SiteDb().GetRepository(Source);
            var page = repo.Store.get(Id) as Page;
            if (page == null) return;
            var doc = Kooboo.Dom.DomParser.CreateDom(page.Body);
            var node = Service.DomService.GetElementByKoobooId(doc, KoobooId);
            if (node == null) return;

            switch (Action)
            {
                case ActionType.copy:
                    var copyed = node.cloneNode(true);
                    node.insertBefore(node.nextSibling(), copyed);
                    break;
                case ActionType.delete:
                    node.parentElement?.removeChild(node);
                    break;
                default:
                    if (Value == null) break;
                    node.InnerHtml = Value;
                    break;
            }

            page.Body = doc.OuterHtml;
        }

        public string GetValue(string key)
        {
            var action = Infos.FirstOrDefault(f => f.Key == "action");
            return action.Value;
        }

        public abstract void Modify(RenderContext context);

    }
}
