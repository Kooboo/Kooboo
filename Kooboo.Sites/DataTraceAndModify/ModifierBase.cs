using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string Attribute => GetValue("attribute");

        public string Id => GetValue("id");

        public void HandleDom(RenderContext context)
        {
            if (Id == null || KoobooId == null) return;
            var repo = context.WebSite.SiteDb().GetRepository(Source);
            if (repo == null) return;
            var domObject = repo.GetByNameOrId(Id) as IDomObject;
            if (domObject == null) return;
            var doc = DomParser.CreateDom(domObject.Body);
            var node = Service.DomService.GetElementByKoobooId(doc, KoobooId);
            if (node == null) return;
            var element = node as Element;
            if (element == null) return;
            UpdateDomObject(domObject, element);
            repo.AddOrUpdate(domObject, context.User.Id);

            if (repo.ModelType == typeof(Page))
            {
                Task.Run(() =>
                {
                    var otherPages = context.WebSite.SiteDb().Pages.All()
                   .Where(o => o.Id != domObject.Id && o.HasLayout == false)
                   .ToList();

                    foreach (var page in otherPages)
                    {
                        var sameBlock = Helper.ElementHelper.FindSameElement(element, page.Dom);

                        if (sameBlock != null)
                        {
                            UpdateDomObject(page, sameBlock);
                            repo.AddOrUpdate(page, context.User.Id);
                        }
                    }
                });
            }
        }

        private void UpdateDomObject(IDomObject domObject, Element element)
        {
            SourceUpdate sourceUpdate = null;

            switch (Action)
            {
                case ActionType.copy:
                    var elementStr = Service.DomService.ReSerializeOpenTag(element);
                    sourceUpdate = new SourceUpdate
                    {
                        StartIndex = element.location.openTokenStartIndex,
                        EndIndex = element.location.endTokenEndIndex,
                        NewValue = elementStr + elementStr
                    };
                    break;
                case ActionType.delete:
                    sourceUpdate = new SourceUpdate
                    {
                        StartIndex = element.location.openTokenStartIndex,
                        EndIndex = element.location.endTokenEndIndex,
                        NewValue = string.Empty
                    };
                    break;
                default:
                    if (Value == null) break;
                    sourceUpdate = HandleDomUpdate(element, element);
                    break;
            }

            domObject.Body = Service.DomService.UpdateSource(domObject.Body, new List<SourceUpdate> { sourceUpdate });
        }

        private SourceUpdate HandleDomUpdate(Node node, Element element)
        {
            SourceUpdate sourceUpdate;
            if (string.IsNullOrWhiteSpace(Attribute))
            {
                sourceUpdate = new SourceUpdate
                {
                    StartIndex = node.location.openTokenEndIndex + 1,
                    EndIndex = node.location.endTokenStartIndex - 1,
                    NewValue = Value
                };
            }
            else
            {
                element.setAttribute(Attribute, Value);
                sourceUpdate = new SourceUpdate
                {
                    StartIndex = node.location.openTokenStartIndex,
                    EndIndex = node.location.endTokenEndIndex,
                    NewValue = Service.DomService.ReSerializeOpenTag(element)
                };

            }

            return sourceUpdate;
        }

        public string GetValue(string key)
        {
            var action = Infos.FirstOrDefault(f => f.Key == key);
            return action.Value;
        }

        public abstract void Modify(RenderContext context);

    }
}
