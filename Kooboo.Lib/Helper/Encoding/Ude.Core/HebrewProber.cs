//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Ude.Core
{
    public class HebrewProber : CharsetProber
    {
        private const byte FINAL_KAF = 234;

        private const byte NORMAL_KAF = 235;

        private const byte FINAL_MEM = 237;

        private const byte NORMAL_MEM = 238;

        private const byte FINAL_NUN = 239;

        private const byte NORMAL_NUN = 240;

        private const byte FINAL_PE = 243;

        private const byte NORMAL_PE = 244;

        private const byte FINAL_TSADI = 245;

        private const byte NORMAL_TSADI = 246;

        private const int MIN_FINAL_CHAR_DISTANCE = 5;

        private const float MIN_MODEL_DISTANCE = 0.01f;

        protected const string VISUAL_HEBREW_NAME = "ISO-8859-8";

        protected const string LOGICAL_HEBREW_NAME = "windows-1255";

        protected CharsetProber logicalProber;

        protected CharsetProber visualProber;

        protected int finalCharLogicalScore;

        protected int finalCharVisualScore;

        protected byte prev;

        protected byte beforePrev;

        public HebrewProber()
        {
            this.Reset();
        }

        public void SetModelProbers(CharsetProber logical, CharsetProber visual)
        {
            this.logicalProber = logical;
            this.visualProber = visual;
        }

        public override ProbingState HandleData(byte[] buf, int offset, int len)
        {
            if (this.GetState() == ProbingState.NotMe)
            {
                return ProbingState.NotMe;
            }
            checked
            {
                int num = offset + len;
                for (int i = offset; i < num; i++)
                {
                    byte b = buf[i];
                    if (b == 32)
                    {
                        if (this.beforePrev != 32)
                        {
                            if (HebrewProber.IsFinal(this.prev))
                            {
                                this.finalCharLogicalScore++;
                            }
                            else if (HebrewProber.IsNonFinal(this.prev))
                            {
                                this.finalCharVisualScore++;
                            }
                        }
                    }
                    else if (this.beforePrev == 32 && HebrewProber.IsFinal(this.prev) && b != 32)
                    {
                        this.finalCharVisualScore++;
                    }
                    this.beforePrev = this.prev;
                    this.prev = b;
                }
                return ProbingState.Detecting;
            }
        }

        public override string GetCharsetName()
        {
            int num = checked(this.finalCharLogicalScore - this.finalCharVisualScore);
            if (num >= 5)
            {
                return "windows-1255";
            }
            if (num <= -5)
            {
                return "ISO-8859-8";
            }
            float num2 = this.logicalProber.GetConfidence() - this.visualProber.GetConfidence();
            if (num2 > 0.01f)
            {
                return "windows-1255";
            }
            if (num2 < -0.01f)
            {
                return "ISO-8859-8";
            }
            if (num < 0)
            {
                return "ISO-8859-8";
            }
            return "windows-1255";
        }

        public override void Reset()
        {
            this.finalCharLogicalScore = 0;
            this.finalCharVisualScore = 0;
            this.prev = 32;
            this.beforePrev = 32;
        }

        public override ProbingState GetState()
        {
            if (this.logicalProber.GetState() == ProbingState.NotMe && this.visualProber.GetState() == ProbingState.NotMe)
            {
                return ProbingState.NotMe;
            }
            return ProbingState.Detecting;
        }

        public override void DumpStatus()
        {
            Console.WriteLine("  HEB: {0} - {1} [Logical-Visual score]", this.finalCharLogicalScore, this.finalCharVisualScore);
        }

        public override float GetConfidence()
        {
            return 0f;
        }

        protected static bool IsFinal(byte b)
        {
            return b == 234 || b == 237 || b == 239 || b == 243 || b == 245;
        }

        protected static bool IsNonFinal(byte b)
        {
            return b == 235 || b == 238 || b == 240 || b == 244;
        }
    }
}
