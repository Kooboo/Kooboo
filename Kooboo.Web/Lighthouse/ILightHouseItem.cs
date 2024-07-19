using Kooboo.Data.Context;

namespace Kooboo.Web.Lighthouse
{
    public interface ILightHouseItem
    {
        string Name { get; }

        string Description { get; }

        void Execute(RenderContext Context);

        bool ImgTag { get; }

        bool ATag { get; }
    }
}
