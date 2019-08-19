using Kooboo.Web.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Authorization
{
    public interface IApiPermissionLink<T> : IPermissionControl where T: ICmsMenu
    {
    }
}
