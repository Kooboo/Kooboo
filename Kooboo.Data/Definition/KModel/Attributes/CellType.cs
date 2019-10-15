using System;

namespace Kooboo.Data.Definition.KModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CellType : Attribute, IMetaAttribute
    {
        public EnumCellType Type { get; set; }

        public bool IsHeader => false;

        public string PropertyName => "type";

        public CellType(EnumCellType type)
        {
            Type = type;
        }

        public string Value()
        {
            return Type.ToString();
        }
    }
}