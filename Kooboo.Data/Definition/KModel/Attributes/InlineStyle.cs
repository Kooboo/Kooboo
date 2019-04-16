using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Definition.KModel.Attributes
{
   
    [AttributeUsage(AttributeTargets.Property)]
    public class InlineStyle : Attribute, IMetaAttribute
    {
        public string inline  { get; set; }

        public bool IsHeader => false;

        public string PropertyName => "Style";

        public InlineStyle(string inlineStyle)
        {
            this.inline = inlineStyle; 
        }

        public string Value()
        {
            return this.inline; 
        }
    }  
}
