using Kooboo.Web.Menus;

namespace Kooboo.Web.Authorization
{
    public interface IApiPermissionLink<T> : IPermissionControl where T : ICmsMenu
    {
    }
}