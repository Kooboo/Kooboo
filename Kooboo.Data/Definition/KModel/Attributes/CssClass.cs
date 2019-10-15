using System;

namespace Kooboo.Data.Definition.KModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CssClass : Attribute, IMetaAttribute
    {
        public string classname { get; set; }

        public bool IsHeader => false;

        public string PropertyName => "Class";

        public CssClass(string classname)
        {
            this.classname = classname;
        }

        public string Value()
        {
            return classname;
        }
    }
}