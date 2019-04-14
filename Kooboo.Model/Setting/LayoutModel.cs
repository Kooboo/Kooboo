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
    //config data setting by model
    [ModelName("Layout")]
    [Title("Layouts")]
    [Api("Kooboo.Layout.list")]
    public class LayoutModel: KoobooSetting, IKoobooModel
    {
        [BreadCrumb("sites", "/_Admin/Sites")]
        [BreadCrumb("dashboard", "/_Admin/Site")]
        [BreadCrumb("Layouts", "")]
        public string BreadCrumbs { get; set; }

        [TableAction("Create", ButtonActionType.Link,KBColor.Green)]
        [Link("Create", "/Development/Layout", null, null)]
        public string Create { get; set; }

        [TableAction("Copy", ButtonActionType.Modal,KBColor.Green)]
        [Modal("copy", null, null)]
        public string Copy { get; set; }

        [TableAction("Delete", ButtonActionType.None,KBColor.Red)]
        public string Delete { get; set; }


        [Column("Name", CellType.Link)]
        [Link("name", "/Development/Layout", "id")]
        public string Name { get; set; }

        [Column("Used by", CellType.Array)]
        [Modal("relation",null,null)]
        [NewModal("Relation",ComponentType.KTable, "relations")]
        public string Relations { get; set; }

        [Column("Last modified", CellType.Text,CellDataType.Date)]
        public DateTime LastModified { get; set; }

        [RowAction("versions","View all versions", CellType.Button)]
        [Link("versions", "/System/SiteLog/LogVersions", "keyHash", "storeNameHash")]
        public string Versions { get; set; }

    }
    
    //[ModelName("LayoutCopyModel")]
    //public class CopyModel:IKoobooModel
    //{
    //    [FormField("Name","Name",ComponentType.Input)]
    //    public string Name { get; set; }

    //}

    [ModelName("relations")]
    [Title("Relation")]
    [Api("Kooboo.Relation.showBy")]
    public class RelationsModel : IKoobooModel
    {
        [Column("Name",CellType.Link)]
        [Link("Name","url")]
        [TableSelectable(false)]
        public string Name { get; set; }

        [Column("Remark", CellType.Text)]
        public string Remark { get; set; }

        [RowAction("Edit","Edit",CellType.Button)]
        [Link("Edit","/aa","Id")]//url 1.additional js function //2.unified url
        public string Edit { get; set; }
    }


}
