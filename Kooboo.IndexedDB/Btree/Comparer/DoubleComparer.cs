//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.BTree.Comparer
{
    public class DoubleComparer : IComparer<byte[]>
    {
        public int Compare(byte[] x, byte[] y)
        {

            double doublex = BitConverter.ToDouble(x, 0);

            double doubley = BitConverter.ToDouble(y, 0);

            if (doublex > doubley)
            {
                return 1;
            }
            else if (doublex < doubley)
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
