//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom;

namespace Kooboo.Sites.Automation.Layout
{
   public class LayoutResult
    {
       public LayoutResult()
       {
           this.Elements = new List<Element>(); 
       }

       public int AffectedPages { get; set; }

       public List<Element> Elements { get; set; }

       /// <summary>
       /// How much match this is for a layout. 
       /// </summary>
       public double Score { get; set; }

    }
}
