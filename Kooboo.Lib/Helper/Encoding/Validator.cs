//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Text;

namespace Kooboo.Lib.Helper.EncodingHelper
{
    public static class Validator
    {

        public static Encoding TryGetEncoding(byte[] input)
        {
            foreach (var item in System.Text.Encoding.GetEncodings())
            {
                var encoding = item.GetEncoding();

                if (IsRightEncoding(input, encoding))
                {
                    return encoding;
                }
            }
            return null;
        }

        public static bool IsRightEncoding(byte[] input, System.Text.Encoding encoding)
        {
            var text = encoding.GetString(input);

            var bytes = encoding.GetBytes(text);

            if (IsSameArray(input, bytes))
            {
                return true;
            }
            return false;
        }

        public static bool IsSameArray(byte[] x, byte[] y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            int len = x.Length;
            if (len != y.Length)
            {
                return false;
            }

            for (int i = 0; i < len; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }

            }

            return true;
        }


    }
}
