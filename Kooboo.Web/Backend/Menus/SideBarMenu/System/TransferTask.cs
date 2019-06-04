using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class TransferTask : ISideBarMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "TransferTask";

        public string Icon => "";

        public string Url => "System/TransferTask";

        public int Order => 2;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("TransferTask", Context); 
        }
    }
}
 

///new MenuItem{ Name = Hardcoded.GetValue("TransferTask",context), Url = AdminUrl("System/TransferTask", siteDb), ActionRights = Sites.Authorization.Actions.Systems.Settings },