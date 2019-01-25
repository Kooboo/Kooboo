//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Query
{
  public   class FilterItem
    {
      public string FieldOrProperty { get; set; }

      public byte[] Value
      {
          get;
          set;
      }

      public Type FieldType { get; set; }

      public int Length { get; set; }

      public Comparer Compare { get; set; }

      public DateTimeScope TimeScope { get; set; }

    }
}
