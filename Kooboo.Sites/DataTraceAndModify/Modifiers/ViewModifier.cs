using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class ViewModifier : DomModifier
    {
        public override string Source => "view";

        public override void Modify(RenderContext context)
        {
            UpdateSiteObject(context);
        }
    }
}