using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Definition.KModel.Attributes
{
     
    [AttributeUsage(AttributeTargets.Property)]
    public class HeaderClass : Attribute, IMetaAttribute
    {
        public string name { get; set; }

        public bool IsHeader => true;

        public string PropertyName => "class";

        public HeaderClass(string classname)
        {
            this.name = classname;
        }

        public string Value()
        {
            return this.name;
        }
    }


}
