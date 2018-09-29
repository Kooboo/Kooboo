//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Automation.Notation
{
   public class HasMinMaxWidth : INotation
    {
        public string Name
        {
            get { return "HasMinMaxWidth"; }
        }

        public Type ReturnType
        {
            get { return typeof(bool); }
        }

        public object Execute(Dom.Element element)
        {
           
              string  maxwidthvalue = element.RawComputedStyle.getPropertyValue("max-width");
        
              string minwidthvalue =   element.RawComputedStyle.getPropertyValue("min-width");

              if (!string.IsNullOrEmpty(maxwidthvalue) || !string.IsNullOrEmpty(minwidthvalue))
              {
                  return true; 
              }

              return false; 
            
        }

        public List<string> ReturnStringValueList
        {
            get { throw new NotImplementedException(); }
        }
    }
}
