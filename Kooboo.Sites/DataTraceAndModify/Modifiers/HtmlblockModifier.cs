using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Dom;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class HtmlblockModifier : DomModifier
    {
        public override string Source => "htmlblock";
        RenderContext _context;
        string _culture;
        string _currentCultureDom;

        internal override Element GetElement(IDomObject domObject)
        {
            if (string.IsNullOrWhiteSpace(KoobooId)) return null;
            _currentCultureDom = ((HtmlBlock)domObject).GetValue(_culture).ToString();
            var doc = DomParser.CreateDom(_currentCultureDom);
            var node = Service.DomService.GetElementByKoobooId(doc, KoobooId);
            if (node == null) return null;
            return node as Element;
        }

        internal override void UpdateDomObject(IRepository repo, RenderContext context, IDomObject domObject, SourceUpdate sourceUpdate)
        {
            var newDom = Service.DomService.UpdateSource(_currentCultureDom, new List<SourceUpdate> { sourceUpdate });
            ((HtmlBlock)domObject).SetValue(_culture, newDom);
            repo.AddOrUpdate(domObject, context.User.Id);
        }

        public override void Modify(RenderContext context)
        {
            _context = context;
            _culture = GetCulture(_context);
            UpdateSiteObject(context);
        }
    }
}
