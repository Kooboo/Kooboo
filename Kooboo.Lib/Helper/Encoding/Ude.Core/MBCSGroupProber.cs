//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Ude.Core
{
    public class MBCSGroupProber : CharsetProber
    {
        private const int PROBERS_NUM = 7;

        private static readonly string[] ProberName = new string[]
        {
            "UTF8",
            "SJIS",
            "EUCJP",
            "GB18030",
            "EUCKR",
            "Big5",
            "EUCTW"
        };

        private CharsetProber[] probers = new CharsetProber[7];

        private bool[] isActive = new bool[7];

        private int bestGuess;

        private int activeNum;

        public MBCSGroupProber()
        {
            this.probers[0] = new UTF8Prober();
            this.probers[1] = new SJISProber();
            this.probers[2] = new EUCJPProber();
            this.probers[3] = new GB18030Prober();
            this.probers[4] = new EUCKRProber();
            this.probers[5] = new Big5Prober();
            this.probers[6] = new EUCTWProber();
            this.Reset();
        }

        public override string GetCharsetName()
        {
            if (this.bestGuess == -1)
            {
                this.GetConfidence();
                if (this.bestGuess == -1)
                {
                    this.bestGuess = 0;
                }
            }
            return this.probers[this.bestGuess].GetCharsetName();
        }

        public override void Reset()
        {
            this.activeNum = 0;
            checked
            {
                for (int i = 0; i < this.probers.Length; i++)
                {
                    if (this.probers[i] != null)
                    {
                        this.probers[i].Reset();
                        this.isActive[i] = true;
                        this.activeNum++;
                    }
                    else
                    {
                        this.isActive[i] = false;
                    }
                }
                this.bestGuess = -1;
                this.state = ProbingState.Detecting;
            }
        }

        public override ProbingState HandleData(byte[] buf, int offset, int len)
        {
            byte[] array = new byte[len];
            int len2 = 0;
            bool flag = true;
            checked
            {
                int num = offset + len;
                for (int i = offset; i < num; i++)
                {
                    if ((buf[i] & 128) != 0)
                    {
                        array[len2++] = buf[i];
                        flag = true;
                    }
                    else if (flag)
                    {
                        array[len2++] = buf[i];
                        flag = false;
                    }
                }
                for (int j = 0; j < this.probers.Length; j++)
                {
                    if (this.isActive[j])
                    {
                        ProbingState probingState = this.probers[j].HandleData(array, 0, len2);
                        if (probingState == ProbingState.FoundIt)
                        {
                            this.bestGuess = j;
                            this.state = ProbingState.FoundIt;
                            break;
                        }
                        if (probingState == ProbingState.NotMe)
                        {
                            this.isActive[j] = false;
                            this.activeNum--;
                            if (this.activeNum <= 0)
                            {
                                this.state = ProbingState.NotMe;
                                break;
                            }
                        }
                    }
                }
                return this.state;
            }
        }

        public override float GetConfidence()
        {
            float num = 0f;
            if (this.state == ProbingState.FoundIt)
            {
                return 0.99f;
            }
            if (this.state == ProbingState.NotMe)
            {
                return 0.01f;
            }
            checked
            {
                for (int i = 0; i < 7; i++)
                {
                    if (this.isActive[i])
                    {
                        float confidence = this.probers[i].GetConfidence();
                        if (num < confidence)
                        {
                            num = confidence;
                            this.bestGuess = i;
                        }
                    }
                }
                return num;
            }
        }

        public override void DumpStatus()
        {
            this.GetConfidence();
            checked
            {
                for (int i = 0; i < 7; i++)
                {
                    if (!this.isActive[i])
                    {
                        Console.WriteLine("  MBCS inactive: {0} (confidence is too low).", MBCSGroupProber.ProberName[i]);
                    }
                    else
                    {
                        float confidence = this.probers[i].GetConfidence();
                        Console.WriteLine("  MBCS {0}: [{1}]", confidence, MBCSGroupProber.ProberName[i]);
                    }
                }
            }
        }
    }
}
