//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.BTree.Comparer
{
    public class FloatComparer : IComparer<byte[]>
    {

        /// <summary>
        /// TODO: For performance reason, this does not sounds like effective enought to cast. 
        /// Should compare byte by byte directly in future. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(byte[] x, byte[] y)
        {
            float floatx = BitConverter.ToSingle(x, 0);

            float floaty = BitConverter.ToSingle(y, 0);

            if (floatx > floaty)
            {
                return 1;
            }
            else if (floatx < floaty)
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
