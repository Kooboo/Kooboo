//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Attributes
{
   // indicate one data method require the FolderId Parameter First. 
   [AttributeUsage(AttributeTargets.Method)]
  public  class RequireFolder : Attribute
    {
    }
}
