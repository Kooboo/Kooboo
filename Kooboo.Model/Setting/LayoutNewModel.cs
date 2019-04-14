using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Kooboo.Data.Models;
using Kooboo.Model.Attributes;
using Kooboo.Model.Operations;
using Kooboo.Model.Components.Table;
using Kooboo.Model.Components;

namespace Kooboo.Model.Setting
{
    //[MetadataType(typeof(LayoutTest))]
    [ModelName("Layout"), Title("Layouts"), Api("Kooboo.Layout.list")]
    [CreateOperation, CopyOperation, DeleteOperation, OtherOperation]
    public class LayoutNewModel
    {
        [Column("Name", CellType.Link)]
        [Link("name", "/Development/Layout", "id")]
        public string Name { get; set; }

        [Column("Used by", CellType.Array)]
        [NewModal("Relation", ComponentType.KTable, "relations")]
        public string Relations { get; set; }

        [Column("Last modified", CellType.Text, CellDataType.Date)]
        public DateTime LastModified { get; set; }

        [RowAction("versions", "View all versions", CellType.Button)]
        [Link("versions", "/System/SiteLog/LogVersions", "keyHash", "storeNameHash")]
        public string Versions { get; set; }
    }

    public class OtherOperation : Attribute, IOperation
    {
        public string Name => "Other";

        public string DisplayName { get; set; }
    }


    public class LayoutTest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; }

        public DateTime LastModified { get; set; }

        public Dictionary<string, int> Relations { get; set; }

    }

}
