//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB.Btree.Comparer
{
  public  class ByteComparer : IComparer<byte[]>
    {
        public int Compare(byte[] x, byte[] y)
        {
            if (x[0] > y[0])
            {
                return 1;
            }
            else if (y[0] > x[0])
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
