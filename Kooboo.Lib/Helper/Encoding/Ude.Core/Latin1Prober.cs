//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Ude.Core
{
    public class Latin1Prober : CharsetProber
    {
        private const int FREQ_CAT_NUM = 4;

        private const int UDF = 0;

        private const int OTH = 1;

        private const int ASC = 2;

        private const int ASS = 3;

        private const int ACV = 4;

        private const int ACO = 5;

        private const int ASV = 6;

        private const int ASO = 7;

        private const int CLASS_NUM = 8;

        private static readonly byte[] Latin1_CharToClass = new byte[]
        {
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,
            1,
            1,
            1,
            1,
            1,
            1,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            1,
            1,
            1,
            1,
            1,
            1,
            0,
            1,
            7,
            1,
            1,
            1,
            1,
            1,
            1,
            5,
            1,
            5,
            0,
            5,
            0,
            0,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            7,
            1,
            7,
            0,
            7,
            5,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            4,
            4,
            4,
            4,
            4,
            4,
            5,
            5,
            4,
            4,
            4,
            4,
            4,
            4,
            4,
            4,
            5,
            5,
            4,
            4,
            4,
            4,
            4,
            1,
            4,
            4,
            4,
            4,
            4,
            5,
            5,
            5,
            6,
            6,
            6,
            6,
            6,
            6,
            7,
            7,
            6,
            6,
            6,
            6,
            6,
            6,
            6,
            6,
            7,
            7,
            6,
            6,
            6,
            6,
            6,
            1,
            6,
            6,
            6,
            6,
            6,
            7,
            7,
            7
        };

        private static readonly byte[] Latin1ClassModel = new byte[]
        {
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            0,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            0,
            3,
            3,
            3,
            1,
            1,
            3,
            3,
            0,
            3,
            3,
            3,
            1,
            2,
            1,
            2,
            0,
            3,
            3,
            3,
            3,
            3,
            3,
            3,
            0,
            3,
            1,
            3,
            1,
            1,
            1,
            3,
            0,
            3,
            1,
            3,
            1,
            1,
            3,
            3
        };

        private byte lastCharClass;

        private int[] freqCounter = new int[4];

        public Latin1Prober()
        {
            this.Reset();
        }

        public override string GetCharsetName()
        {
            return "windows-1252";
        }

        public override void Reset()
        {
            this.state = ProbingState.Detecting;
            this.lastCharClass = 1;
            checked
            {
                for (int i = 0; i < 4; i++)
                {
                    this.freqCounter[i] = 0;
                }
            }
        }

        public override ProbingState HandleData(byte[] buf, int offset, int len)
        {
            byte[] array = CharsetProber.FilterWithEnglishLetters(buf, offset, len);
            checked
            {
                for (int i = 0; i < array.Length; i++)
                {
                    byte b = Latin1Prober.Latin1_CharToClass[(int)array[i]];
                    byte b2 = Latin1Prober.Latin1ClassModel[(int)(this.lastCharClass * 8 + b)];
                    if (b2 == 0)
                    {
                        this.state = ProbingState.NotMe;
                        break;
                    }
                    this.freqCounter[(int)b2]++;
                    this.lastCharClass = b;
                }
                return this.state;
            }
        }

        public override float GetConfidence()
        {
            if (this.state == ProbingState.NotMe)
            {
                return 0.01f;
            }
            int num = 0;
            checked
            {
                for (int i = 0; i < 4; i++)
                {
                    num += this.freqCounter[i];
                }
            }
            float num2;
            if (num <= 0)
            {
                num2 = 0f;
            }
            else
            {
                num2 = (float)this.freqCounter[3] * 1f / (float)num;
                num2 -= (float)this.freqCounter[1] * 20f / (float)num;
            }
            if (num2 >= 0f)
            {
                return num2 * 0.5f;
            }
            return 0f;
        }

        public override void DumpStatus()
        {
            Console.WriteLine(" Latin1Prober: {0} [{1}]", this.GetConfidence(), this.GetCharsetName());
        }
    }
}
