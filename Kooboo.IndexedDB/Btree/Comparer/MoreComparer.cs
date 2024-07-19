//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.IndexedDB.BTree.Comparer
{
    public class MoreComparer
    {
        /// <summary>
        /// test whether input start with comparevalue or not. 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="compareValue"></param>
        /// <param name="length">the length of comparevalue without tailing zero byte.</param>
        /// <returns></returns>
        public static bool StartWith(byte[] input, byte[] compareValue, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (input[i] != compareValue[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// test whether input contains substring the compare value or not.
        /// TODO: string match has a better algo which jump i+2/3/4 instead of i+1, to be implemented.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="comparevalue"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool Contains(byte[] input, byte[] comparevalue, int ValueLength, int MaxInputLength)
        {
            int maxi = MaxInputLength - ValueLength + 1;   // when this i reach, return false. 

            byte startbyte = comparevalue[0];

            for (int i = 0; i < maxi; i++)
            {
                if (input[i] == startbyte)
                {
                    bool allmatched;
                    allmatched = true;
                    for (int j = 0; j < ValueLength; j++)
                    {
                        if (input[i + j] != comparevalue[j])
                        {
                            allmatched = false;
                            break;
                        }
                    }
                    if (allmatched)
                    {
                        return true;
                    }

                    allmatched = true;
                }

            }

            return false;

        }

    }
}
