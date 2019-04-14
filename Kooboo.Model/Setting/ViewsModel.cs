using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Model.Components.Table;
using Kooboo.Model.Attributes;
using Kooboo.Model.Components;

namespace Kooboo.Model.Setting
{

    [ModelName("View")]
    [Title("Views")]
    [Api("Kooboo.View.getList")]
    public class ViewsModel : IKoobooModel
    {
        [BreadCrumb("sites", "/_Admin/Sites")]
        [BreadCrumb("dashboard", "/_Admin/Site")]
        [BreadCrumb("VIEWS", "")]
        public string BreadCrumbs { get; set; }

        [TableAction("Create", ButtonActionType.Link, KBColor.Green)]
        [Link("Create", "/Development/View", null, null)]
        public string Create { get; set; }

        [TableAction("Copy", ButtonActionType.Modal, KBColor.Green)]
        [Modal("copy", null, null)]
        public string Copy { get; set; }

        [TableAction("Delete", ButtonActionType.None, KBColor.Red)]
        public string Delete { get; set; }

        [Column("name",CellType.Link)]
        [Link("name", "/Development/View", "id")]
        public string Name { get; set; }

        [Column("Used by", CellType.Array)]
        [NewModal("Relation", ComponentType.KTable, "relations")]
        public string Relations { get; set; }

        [Column("Data Sources", CellType.Badge)]
        [NewModal("", ComponentType.KTable, "datasouceModal")]
        public string DataSourceCount { get; set; }

        [Column("Preview", CellType.Link)]
        [Link("Preview", "preview")]
        public string Preview { get; set; }

        [Column("Last modified", CellType.Text,CellDataType.Date)]
        public string LastModified { get; set; }

        [RowAction("versions", "View all versions", CellType.Button)]
        [Link("versions", "/System/SiteLog/LogVersions", "keyHash", "storeNameHash")]
        public string Versions { get; set; }
        
    }

    [ModelName("datasouceModal")]
    [Title("Data Sources")]
    [Api("Kooboo.View.ViewMethods")]
    public class DatasouceModal : IKoobooModel
    {
        [Column("Name", CellType.Text)]
        [TableSelectable(false)]
        public string AliasName { get; set; }

        [RowAction("Edit", "Edit", CellType.Button)]
        [Link("Edit", "/aa", "Id")]//url 1.additional js function //2.unified url
        public string Edit { get; set; }
    }
}
