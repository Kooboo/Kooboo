using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Dom;
using Kooboo.Dom.CSS;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public abstract class DomModifier : ModifierBase
    {

        public string KoobooId => GetValue("koobooid");

        public string Property => GetValue("property");

        public bool Important => !string.IsNullOrWhiteSpace(GetValue("important"));

        public string StyleSheetUrl => GetValue("url");

        public string MediaRuleList => GetValue("mediarulelist");

        public string Attribute => GetValue("attribute");

        internal virtual IRepository GetRepo(RenderContext context) => context.WebSite.SiteDb().GetRepository(Source);

        internal virtual IDomObject GetDomObject(IRepository repository)
        {
            if (string.IsNullOrWhiteSpace(Id)) return null;
            return repository?.GetByNameOrId(Id) as IDomObject;
        }

        internal virtual Element GetElement(IDomObject domObject)
        {
            if (domObject == null || string.IsNullOrWhiteSpace(KoobooId)) return null;
            var doc = DomParser.CreateDom(domObject.Body);
            var node = Service.DomService.GetElementByKoobooId(doc, KoobooId);
            if (node == null) return null;
            return node as Element;
        }

        internal virtual void UpdateSiteObject(RenderContext context)
        {
            var repo = GetRepo(context);
            var domObject = GetDomObject(repo);
            var element = GetElement(domObject);
            HandleUpdate(context, repo, domObject, element);
        }

        internal virtual void HandleUpdate(RenderContext context, IRepository repo, IDomObject domObject, Element element)
        {
            switch (Action)
            {
                case ActionType.update:
                    UpdateDom(repo, context, domObject, element);
                    break;
                case ActionType.delete:
                    DeleteDom(repo, context, domObject, element);
                    break;
                case ActionType.copy:
                    CopyDom(repo, context, domObject, element);
                    break;
                case ActionType.styleSheet:
                    var styleId = GetStyleId(context, element, domObject);
                    UpdateStyleSheet(context, styleId);
                    break;
                default:
                    break;
            }
        }

        internal virtual void UpdateDom(IRepository repo, RenderContext context, IDomObject domObject, Element element)
        {
            SourceUpdate sourceUpdate;

            if (!string.IsNullOrWhiteSpace(Attribute))
            {
                element.setAttribute(Attribute, Value);
                sourceUpdate = new SourceUpdate
                {
                    StartIndex = element.location.openTokenStartIndex,
                    EndIndex = element.location.openTokenEndIndex,
                    NewValue = Service.DomService.ReSerializeOpenTag(element)
                };
            }
            else if (!string.IsNullOrWhiteSpace(Property))
            {
                var style = element.getAttribute("style");

                var cssDeclar = CSSSerializer.deserializeDeclarationBlock(style);
                var propertise = cssDeclar.item;
                var exist = propertise.Find(o => o.propertyname.ToLower() == Property.ToLower());

                if (exist == null)
                {
                    propertise.Add(new CSSDeclaration
                    {
                        important = Important,
                        propertyname = Property,
                        value = Value
                    });
                }
                else
                {
                    exist.value = Value;
                    exist.important = Important;
                    propertise.Remove(exist);
                    propertise.Add(exist);
                }

                cssDeclar.item = cssDeclar.item.Where(o => !string.IsNullOrWhiteSpace(o.propertyname) && !string.IsNullOrWhiteSpace(o.value)).ToList();
                var newStyle = CSSSerializer.serializeDeclarationBlock(cssDeclar);
                element.setAttribute("style", newStyle);
                sourceUpdate = new SourceUpdate
                {
                    StartIndex = element.location.openTokenStartIndex,
                    EndIndex = element.location.openTokenEndIndex,
                    NewValue = Service.DomService.ReSerializeOpenTag(element)
                };
            }
            else
            {
                sourceUpdate = new SourceUpdate
                {
                    StartIndex = element.location.openTokenEndIndex + 1,
                    EndIndex = element.location.endTokenStartIndex - 1,
                    NewValue = Value
                };
            }

            UpdateDomObject(repo, context, domObject, sourceUpdate);
        }

        internal virtual void UpdateDomObject(IRepository repo, RenderContext context, IDomObject domObject, SourceUpdate sourceUpdate)
        {
            var newDom = Service.DomService.UpdateSource(domObject.Body, new List<SourceUpdate> { sourceUpdate });
            domObject.Body = newDom;
            repo.AddOrUpdate(domObject, context.User.Id);
        }

        internal virtual void CopyDom(IRepository repo, RenderContext context, IDomObject domObject, Element element)
        {
            var sourceUpdate = new SourceUpdate
            {
                StartIndex = element.location.openTokenStartIndex,
                EndIndex = element.location.endTokenEndIndex,
                NewValue = element.OuterHtml + element.OuterHtml
            };

            UpdateDomObject(repo, context, domObject, sourceUpdate);
        }

        internal virtual void DeleteDom(IRepository repo, RenderContext context, IDomObject domObject, Element element)
        {
            var sourceUpdate = new SourceUpdate
            {
                StartIndex = element.location.openTokenStartIndex,
                EndIndex = element.location.endTokenEndIndex,
                NewValue = string.Empty
            };

            UpdateDomObject(repo, context, domObject, sourceUpdate);
        }


        internal Guid? GetStyleId(RenderContext context, Element element, IDomObject domObject)
        {
            Guid? styleId = null;
            if (string.IsNullOrWhiteSpace(StyleSheetUrl))
            {
                var bodyhash = Lib.Security.Hash.ComputeIntCaseSensitive(element.InnerHtml);
                styleId = context.WebSite.SiteDb().Styles.Query.Where(o => o.OwnerObjectId == domObject.Id && o.BodyHash == bodyhash).FirstOrDefault()?.Id;
            }
            else
            {
                styleId = context.WebSite.SiteDb().Styles.GetByUrl(Lib.Helper.UrlHelper.RelativePath(StyleSheetUrl))?.Id;
            }
            return styleId;
        }

        internal void UpdateStyleSheet(RenderContext context, Guid? styleId)
        {
            if (!styleId.HasValue) return;
            var rules = context.WebSite.SiteDb().CssRules.Query.Where(o => o.ParentStyleId == styleId).SelectAll();
            var changelist = new List<CmsCssRuleChanges>();

            if (!string.IsNullOrWhiteSpace(MediaRuleList))
            {
                var mediaRuleId = rules.FirstOrDefault(f => CssSelectorComparer.IsEqual(f.SelectorText, MediaRuleList))?.Id;
                if (!mediaRuleId.HasValue) rules = rules.Where(w => w.ParentCssRuleId == mediaRuleId).ToList();

            }

            rules = rules.Where(w => CssSelectorComparer.IsEqual(w.SelectorText, Selector)).ToList();

            foreach (var rule in rules)
            {
                CSSDeclarationBlock declarations = Service.CssService.ParseStyleDeclaration(rule);
                if (declarations == null) declarations = new CSSDeclarationBlock();

                if (!string.IsNullOrEmpty(Value))
                {
                    declarations.setProperty(Property, Value, Important);
                    var prop = declarations.item.FirstOrDefault(f => f != null && f.propertyname == Property);
                    if (prop != null)
                    {
                        declarations.item.Remove(prop);
                        declarations.item.Add(prop);
                    }
                }
                else
                {
                    declarations.removeProperty(Property);
                }

                var onechange = new CmsCssRuleChanges
                {
                    CssRuleId = rule.Id,
                    selectorText = rule.SelectorText,
                    DeclarationText = declarations.GenerateCssText()
                };

                if (declarations.item.Count() == 0)
                {
                    onechange.ChangeType = ChangeType.Delete;
                }
                else
                {
                    if (rule.Id == default(Guid))
                    {
                        onechange.ChangeType = ChangeType.Add;
                    }
                    else
                    {
                        onechange.ChangeType = ChangeType.Update;
                    }
                }

                changelist.Add(onechange);
            }

            context.WebSite.SiteDb().CssRules.UpdateStyle(changelist, styleId.Value);
        }
    }
}
