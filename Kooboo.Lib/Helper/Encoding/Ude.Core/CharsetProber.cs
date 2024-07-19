//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;

namespace Ude.Core
{
    public abstract class CharsetProber
    {
        protected const float SHORTCUT_THRESHOLD = 0.95f;

        private const byte SPACE = 32;

        private const byte CAPITAL_A = 65;

        private const byte CAPITAL_Z = 90;

        private const byte SMALL_A = 97;

        private const byte SMALL_Z = 122;

        private const byte LESS_THAN = 60;

        private const byte GREATER_THAN = 62;

        protected ProbingState state;

        public abstract ProbingState HandleData(byte[] buf, int offset, int len);

        public abstract void Reset();

        public abstract string GetCharsetName();

        public abstract float GetConfidence();

        public virtual ProbingState GetState()
        {
            return this.state;
        }

        public virtual void SetOption()
        {
        }

        public virtual void DumpStatus()
        {
        }

        protected static byte[] FilterWithoutEnglishLetters(byte[] buf, int offset, int len)
        {
            byte[] result = null;
            checked
            {
                using (MemoryStream memoryStream = new MemoryStream(buf.Length))
                {
                    bool flag = false;
                    int num = offset + len;
                    int num2 = offset;
                    int i;
                    for (i = offset; i < num; i++)
                    {
                        byte b = buf[i];
                        if ((b & 128) != 0)
                        {
                            flag = true;
                        }
                        else if (b < 65 || (b > 90 && b < 97) || b > 122)
                        {
                            if (flag && i > num2)
                            {
                                memoryStream.Write(buf, num2, i - num2);
                                memoryStream.WriteByte(32);
                                flag = false;
                            }
                            num2 = i + 1;
                        }
                    }
                    if (flag && i > num2)
                    {
                        memoryStream.Write(buf, num2, i - num2);
                    }
                    memoryStream.SetLength(memoryStream.Position);
                    result = memoryStream.ToArray();
                }
                return result;
            }
        }

        protected static byte[] FilterWithEnglishLetters(byte[] buf, int offset, int len)
        {
            byte[] result = null;
            checked
            {
                using (MemoryStream memoryStream = new MemoryStream(buf.Length))
                {
                    bool flag = false;
                    int num = offset + len;
                    int num2 = offset;
                    int i;
                    for (i = offset; i < num; i++)
                    {
                        byte b = buf[i];
                        if (b == 62)
                        {
                            flag = false;
                        }
                        else if (b == 60)
                        {
                            flag = true;
                        }
                        if ((b & 128) == 0 && (b < 65 || b > 122 || (b > 90 && b < 97)))
                        {
                            if (i > num2 && !flag)
                            {
                                memoryStream.Write(buf, num2, i - num2);
                                memoryStream.WriteByte(32);
                            }
                            num2 = i + 1;
                        }
                    }
                    if (!flag && i > num2)
                    {
                        memoryStream.Write(buf, num2, i - num2);
                    }
                    memoryStream.SetLength(memoryStream.Position);
                    result = memoryStream.ToArray();
                }
                return result;
            }
        }
    }
}
