//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Automation.Notation
{
   public class HasBackground : INotation
    {
        public string Name
        {
            get { return "HasBackground"; }
        }

        public Type ReturnType
        {
            get { return typeof(bool); }
        }

        public object Execute(Dom.Element element)
        {
           return element.RawComputedStyle.hasPartialProperty("background");
        }

        public List<string> ReturnStringValueList
        {
            get { return null; }
        }
    }
}
