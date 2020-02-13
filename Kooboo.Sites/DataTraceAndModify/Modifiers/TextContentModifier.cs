using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class TextContentModifier : ModifierBase
    {
        public override string Source => "textcontent";

        public string Id { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }

        public override void Modify(RenderContext context)
        {
            if (Id == null) return;
            if (Path == null) return;

            var textContent = context.WebSite.SiteDb().TextContent.GetByNameOrId(Id);
            if (textContent != null)
            {
                textContent.SetValue(Path, Value);
            }
        }
    }
}
