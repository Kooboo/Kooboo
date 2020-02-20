using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class LabelModifier : ModifierBase
    {
        public override string Source => "label";

        public override void Modify(RenderContext context)
        {
            var repo = context.WebSite.SiteDb().Labels;
            var label = repo.GetByNameOrId(Id);
            if (label == null || Value == null) return;
            label.SetValue(GetCulture(context), Value);
            repo.AddOrUpdate(label, context.User.Id);
        }
    }
}
