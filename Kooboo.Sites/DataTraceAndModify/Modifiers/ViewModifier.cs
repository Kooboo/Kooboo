using Kooboo.Data.Context;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class ViewModifier : ModifierBase
    {
        public override string Source => "view";

        public override void Modify(RenderContext context)
        {
            HandleDom(context);
        }
    }
}