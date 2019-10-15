using System;

namespace Kooboo.Data.Attributes
{
    public class OptionalParameter : Attribute
    {
        public string name { get; set; }
        public Type clrType { get; set; }

        public OptionalParameter(string name, Type type)
        {
            this.name = name;
            this.clrType = type;
        }
    }
}