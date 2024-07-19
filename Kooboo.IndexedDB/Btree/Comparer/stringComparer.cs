//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.IndexedDB.BTree.Comparer
{

    /// <summary>
    /// Compare string. Unicode string comparision only to guarantee that the comparision result will always be the same, but not to sort according to any known library or protocol. As it is not possible to implement. 
    /// </summary>
    public class StringComparer : IComparer<byte[]>
    {
        private int len;
        private int charlen;   // the len of bytes per char.

        public StringComparer(int len, System.Text.Encoding encoding)
        {
            this.len = len;
            byte[] charbyte = encoding.GetBytes("a");
            this.charlen = charbyte.Length;
        }

        public int Compare(byte[] x, byte[] y)
        {
            if (charlen == 1)
            {
                return compareLenOne(x, y);
            }
            else if (charlen == 2)
            {
                return compareLenTwo(x, y);
            }
            else if (charlen == 3)
            {
                return compareLenThree(x, y);
            }
            else
            {
                return compareLenFour(x, y);
            }
        }

        private int compareLenOne(byte[] x, byte[] y)
        {
            for (int i = 0; i < len; i++)
            {
                if (i >= this.len)
                {
                    break;
                }

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


        private int compareLenTwo(byte[] x, byte[] y)
        {
            int i = 0;
            while (true)
            {
                if ((i + 2) > this.len)
                {
                    break;
                }

                if (x[i + 1] > y[i + 1])
                {
                    return 1;
                }
                else if (x[i + 1] < y[i + 1])
                {
                    return -1;
                }
                else
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

                i = i + 2;
            }

            return 0;
        }

        private int compareLenThree(byte[] x, byte[] y)
        {
            int i = 0;
            while (true)
            {
                if ((i + 3) > this.len)
                {
                    break;
                }

                if (x[i + 2] > y[i + 2])
                {
                    return 1;
                }
                else if (x[i + 2] < y[i + 2])
                {
                    return -1;
                }
                else
                {
                    if (x[i + 1] > y[i + 1])
                    {
                        return 1;
                    }
                    else if (x[i + 1] < y[i + 1])
                    {
                        return -1;
                    }
                    else
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

                }

                i = i + 3;
            }

            return 0;
        }

        private int compareLenFour(byte[] x, byte[] y)
        {

            int i = 0;
            while (true)
            {
                if ((i + 4) > this.len)
                {
                    break;
                }
                if (x[i + 3] > y[i + 3])
                {
                    return 1;
                }
                else if (x[i + 3] < y[i + 3])
                {
                    return -1;
                }
                else
                {
                    if (x[i + 2] > y[i + 2])
                    {
                        return 1;
                    }
                    else if (x[i + 2] < y[i + 2])
                    {
                        return -1;
                    }
                    else
                    {
                        if (x[i + 1] > y[i + 1])
                        {
                            return 1;
                        }
                        else if (x[i + 1] < y[i + 1])
                        {
                            return -1;
                        }
                        else
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
                    }
                }

                i = i + 4;
            }

            return 0;
        }

    }
}
