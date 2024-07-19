using Kooboo.Data.Context;

namespace Kooboo.Web.Lighthouse.Items
{
    public class AppendImgWidthHeight : ILightHouseItem
    {
        public string Name => "AppendWidthHeight";

        public string Description => "Append Width Height value for image";

        public bool ImgTag => true;

        public bool ATag => false;

        public void Execute(RenderContext Context)
        {

        }
    }
}
