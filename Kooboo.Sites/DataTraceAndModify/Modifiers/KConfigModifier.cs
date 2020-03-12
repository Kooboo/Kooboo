using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class KConfigModifier : ModifierBase
    {
        public override string Source => "kconfig";

        public string Key => GetValue("key");
        public string Attribute => GetValue("attribute");

        public override void Modify(RenderContext context)
        {
            var config = context.WebSite.SiteDb().KConfig.GetByNameOrId(Key);
            if (config != null)
            {
                if (string.IsNullOrWhiteSpace(Attribute) && config.Binding.ContainsKey("innerHtml"))
                {

                    config.Binding["innerHtml"] = Value;
                }
                else if (config.Binding.ContainsKey(Attribute))
                {

                    config.Binding[Attribute] = Value;
                }
                else return;

                context.WebSite.SiteDb().KConfig.AddOrUpdate(config);
            }
        }
    }
}
