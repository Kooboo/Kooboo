using Kooboo.Data.Context;

namespace Kooboo.Web.Lighthouse
{
    public abstract class LightHouseItemWithSetting<TSetting> : ILightHouseItem where TSetting : class, ILightHouseItemSetting
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract bool ImgTag { get; }

        public abstract bool ATag { get; }

        public TSetting Setting { get; set; }

        public abstract void Execute(TSetting setting, RenderContext Context);

        public void Execute(RenderContext Context)
        {
            this.Setting = Context.WebSite.GetLighthouseItemSetting<TSetting>(Name);
            Execute(this.Setting, Context);
        }
    }
}
