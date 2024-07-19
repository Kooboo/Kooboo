//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.IndexedDB.BTree.Comparer
{
    /// <summary>
    /// consider date time as positive int64 number and compare the number only. 
    /// </summary>
    public class DateTimeComparer : IComparer<byte[]>
    {
        private int len = 8;

        public int Compare(byte[] x, byte[] y)
        {
            for (int i = len - 1; i >= 0; i--)
            {
                if (x[i] == y[i])
                {
                    continue;
                }
                else if (x[i] > y[i])
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            return 0;

        }
    }
}
