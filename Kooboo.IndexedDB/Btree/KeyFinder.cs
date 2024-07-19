//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.IndexedDB.BTree
{
    public class KeyFinder
    {
        /// <summary>
        /// Find the bigger key in order to merger, or return null if this key is the biggest. 
        /// If keybytes == null, return the first key. 
        /// </summary>
        /// <param name="keybytes"></param>
        /// <param name="KeyArray"></param>
        /// <returns></returns>
        public static byte[] FindSmallestBiggerKey(byte[] keybytes, Dictionary<byte[], byte[]> KeyArray, IComparer<byte[]> comparer)
        {
            if (keybytes == null)
            {
                return KeyFinder.FindFirstKey(KeyArray, comparer);
            }

            byte[] currentkey = null;
            bool found = false;

            foreach (var item in KeyArray)
            {
                if (comparer.Compare(item.Key, keybytes) > 0)
                {
                    if (!found)
                    {
                        found = true;
                        currentkey = item.Key;
                    }
                    else
                    {
                        if (comparer.Compare(currentkey, item.Key) > 0)
                        {
                            currentkey = item.Key;
                        }
                    }

                }
            }

            if (found)
            {
                return currentkey;
            }
            else
            {
                return null;
            }

        }

        public static byte[] FindBiggestSmallerKey(byte[] keybytes, Dictionary<byte[], byte[]> KeyArray, IComparer<byte[]> comparer)
        {
            if (keybytes == null)
            {
                // this is when it is the previous pointer. 
                return null;
            }

            byte[] currentkey = null;
            bool found = false;

            foreach (var item in KeyArray)
            {
                if (comparer.Compare(keybytes, item.Key) > 0)
                {
                    if (!found)
                    {
                        found = true;
                        currentkey = item.Key;
                    }
                    else
                    {
                        if (comparer.Compare(item.Key, currentkey) > 0)
                        {
                            currentkey = item.Key;
                        }
                    }

                }
            }

            if (found)
            {
                return currentkey;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Find the smallest key in the keyarray.
        /// </summary>
        /// <param name="KeyArray"></param>
        /// <returns></returns>
        public static byte[] FindFirstKey(Dictionary<byte[], byte[]> KeyArray, IComparer<byte[]> comparer)
        {
            byte[] foundkey = null;

            foreach (var item in KeyArray)
            {
                if (foundkey == null)
                {
                    foundkey = item.Key;
                }
                else
                {
                    if (comparer.Compare(foundkey, item.Key) >= 0)
                    {
                        foundkey = item.Key;
                    }
                }

            }

            return foundkey;
        }


        /// <summary>
        /// Find the biggest key in the keyarray.
        /// </summary>
        /// <param name="KeyArray"></param>
        /// <returns></returns>
        public static byte[] FindLastKey(Dictionary<byte[], byte[]> KeyArray, IComparer<byte[]> comparer)
        {
            byte[] foundkey = null;

            foreach (var item in KeyArray)
            {
                if (foundkey == null)
                {
                    foundkey = item.Key;
                }
                else
                {
                    if (comparer.Compare(item.Key, foundkey) >= 0)
                    {
                        foundkey = item.Key;
                    }
                }

            }

            return foundkey;
        }

    }
}
