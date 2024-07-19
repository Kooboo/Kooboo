//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.IndexedDB.BTree.Comparer
{
    public class IntComparer : IComparer<byte[]>
    {
        private int bytelen;

        public IntComparer(IntType IntBit)
        {
            if (IntBit == IntType.Int32)
            {
                bytelen = 4;
            }
            else if (IntBit == IntType.Int16)
            {
                bytelen = 2;
            }
            else
            {
                bytelen = 8;
            }
        }

        public int Compare(byte[] x, byte[] y)
        {
            bool xpositive = false;
            if (x[bytelen - 1] < 128)
            {
                xpositive = true;
            }

            bool yposition = false;

            if (y[bytelen - 1] < 128)
            {
                yposition = true;
            }

            if (xpositive && !yposition)
            {
                return 1;
            }
            else if (yposition && !xpositive)
            {
                return -1;
            }
            else
            {

                return comparebyte(x, y);
            }

        }

        public int comparebyte(byte[] byteone, byte[] bytetwo)
        {
            for (int i = bytelen - 1; i >= 0; i--)
            {
                if (byteone[i] == bytetwo[i])
                {
                    continue;
                }
                else if (byteone[i] > bytetwo[i])
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

    public enum IntType
    {
        Int32 = 0,
        Int16 = 1,
        Int64 = 2
    }
}
