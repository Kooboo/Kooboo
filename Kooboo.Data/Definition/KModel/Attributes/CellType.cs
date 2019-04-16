using System;
using System.Collections.Generic;
using System.Text;

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
            this.Type = type; 
        }

        public string Value()
        {
            return this.Type.ToString(); 
        }
    }  
}
