//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class EUCJPContextAnalyser : JapaneseContextAnalyser
    {
        private const byte HIRAGANA_FIRST_BYTE = 164;

        protected override int GetOrder(byte[] buf, int offset, out int charLen)
        {
            byte b = buf[offset];
            if (b == 142 || (b >= 161 && b <= 254))
            {
                charLen = 2;
            }
            else if (b == 191)
            {
                charLen = 3;
            }
            else
            {
                charLen = 1;
            }
            checked
            {
                if (b == 164)
                {
                    byte b2 = buf[offset + 1];
                    if (b2 >= 161 && b2 <= 243)
                    {
                        return (int)(b2 - 161);
                    }
                }
                return -1;
            }
        }

        protected override int GetOrder(byte[] buf, int offset)
        {
            checked
            {
                if (buf[offset] == 164)
                {
                    byte b = buf[offset + 1];
                    if (b >= 161 && b <= 243)
                    {
                        return (int)(b - 161);
                    }
                }
                return -1;
            }
        }
    }
}
