//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.IndexedDB.BTree.Comparer
{
    public class EqualityComparer : IEqualityComparer<byte[]>
    {
        private int keylen;

        public EqualityComparer(int keylen)
        {
            this.keylen = keylen;
        }

        public bool Equals(byte[] x, byte[] y)
        {
            return ByteEqualComparer.isEqual(x, y, keylen);
        }

        public int GetHashCode(byte[] obj)
        {
            int returnvalue = 0;
            for (int i = 0; i < keylen; i++)
            {
                returnvalue += obj[i];
            }

            return returnvalue;
        }
    }
}
