using Kooboo.Data.Context;
using Kooboo.Web.Lighthouse.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Lighthouse
{
    public abstract class LightHouseItemWithSetting<TSetting> : ILightHouseItem where TSetting : class, ILightHouseItemSetting
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract void Execute(TSetting setting, RenderContext Context);

        public void Execute(RenderContext Context)
        {
            var setting = Context.WebSite.GetLighthouseItemSetting<TSetting>(Name);
            Execute(setting, Context);
        }
    }
}
