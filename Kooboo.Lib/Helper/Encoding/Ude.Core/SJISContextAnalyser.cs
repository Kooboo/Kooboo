//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class SJISContextAnalyser : JapaneseContextAnalyser
    {
        private const byte HIRAGANA_FIRST_BYTE = 130;

        protected override int GetOrder(byte[] buf, int offset, out int charLen)
        {
            if ((buf[offset] >= 129 && buf[offset] <= 159) || (buf[offset] >= 224 && buf[offset] <= 252))
            {
                charLen = 2;
            }
            else
            {
                charLen = 1;
            }
            checked
            {
                if (buf[offset] == 130)
                {
                    byte b = buf[offset + 1];
                    if (b >= 159 && b <= 241)
                    {
                        return (int)(b - 159);
                    }
                }
                return -1;
            }
        }

        protected override int GetOrder(byte[] buf, int offset)
        {
            checked
            {
                if (buf[offset] == 130)
                {
                    byte b = buf[offset + 1];
                    if (b >= 159 && b <= 241)
                    {
                        return (int)(b - 159);
                    }
                }
                return -1;
            }
        }
    }
}
