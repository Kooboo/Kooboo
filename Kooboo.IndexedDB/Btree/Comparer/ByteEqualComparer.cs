//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB.BTree.Comparer
{
    public class ByteEqualComparer
    {

        /// <summary>
        /// This only to check whether two byte[] are the same or not. it can be used for any datatype. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool isEqual(byte[] x, byte[] y, int len)
        {
            if (len == int.MaxValue)
            {
                if (x.Length != y.Length)
                {
                    return false;
                }
                else
                {
                    len = x.Length;
                }
            }

            for (int i = 0; i < len; i++)
            {
                if (x[i] == y[i])
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

    }
}
