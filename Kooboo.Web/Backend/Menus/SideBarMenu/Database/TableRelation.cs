using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;

namespace Kooboo.Web.Menus.SideBarMenu.Database
{
    public class TableRelation : ISideBarMenu
    {
        public SideBarSection Parent =>  SideBarSection.Database;

        public string Name => "Table Relation";

        public string Icon => "";

        public string Url => "Storage/TableRelation";

        public int Order => 2;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Table Relation", Context);
        }
    }
}

// new MenuItem { Name = Hardcoded.GetValue("Table Relation",context), Url = AdminUrl("Storage/TableRelation", siteDb) },