using System.Collections.Generic;

namespace Kooboo.IndexedDB.BTree.Comparer
{

    public class ByteArrayComparer : IComparer<byte[]>
    {
        public int Compare(byte[] x, byte[] y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] > y[i])
                {
                    return 1;
                }
                else if (x[i] < y[i])
                {
                    return -1;
                }
            }

            return 0;
        }
    }

}
