//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom; 

namespace Kooboo.Sites.Automation.Notation
{
  public  class HasBoard : INotation
    {
      
        public string Name
        {
            get { return "HasBorder"; }
        }

        public Type ReturnType
        {
            get { return typeof(bool); }
        }

        public object Execute(Element element)
        {

             return element.RawComputedStyle.hasPartialProperty("border");

        }

        public List<string> ReturnStringValueList
        {
            get { return new List<string>(); }
        }

    }
}
