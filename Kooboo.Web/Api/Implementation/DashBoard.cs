//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Permission;
using Kooboo.Sites.Scripting.Global.SiteItem.DashBoard;
using Kooboo.Web.Service;

namespace Kooboo.Web.Api.Implementation
{
    public class DashBoardApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "dashboard";
            }
        }
        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;
            }
        }

        public JsonTextResponse Item(string name, ApiCall call)
        {
            var items = Resource.instance.SiteScript(call.Context.WebSite);
            var item = items.FirstOrDefault(f => f.Name == name);

            if (item != null)
            {
                var content = Kooboo.Sites.Scripting.Global.SiteItem.DashBoard.Manager.ExecuteAsJson(call.Context, item.ScriptBody);
                return new JsonTextResponse(content);
            }

            return null;
        }

        [Permission(Feature.DASH_BOARD, Action = Data.Permission.Action.VIEW)]
        public JsonTextResponse All(ApiCall call)
        {
            var content = Kooboo.Sites.Scripting.Global.SiteItem.DashBoard.Manager.SiteDashBoardSetting(call.Context);

            return new JsonTextResponse(content);
        }

        public DashBoardInfo info(ApiCall call)
        {
            DashBoardInfo item = new DashBoardInfo(call.Context);

            return item;
        }
    }
}