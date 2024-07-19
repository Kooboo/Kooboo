//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Ude.Core
{
    public class SingleByteCharSetProber : CharsetProber
    {
        private const int SAMPLE_SIZE = 64;

        private const int SB_ENOUGH_REL_THRESHOLD = 1024;

        private const float POSITIVE_SHORTCUT_THRESHOLD = 0.95f;

        private const float NEGATIVE_SHORTCUT_THRESHOLD = 0.05f;

        private const int SYMBOL_CAT_ORDER = 250;

        private const int NUMBER_OF_SEQ_CAT = 4;

        private const int POSITIVE_CAT = 3;

        private const int NEGATIVE_CAT = 0;

        protected SequenceModel model;

        private bool reversed;

        private byte lastOrder;

        private int totalSeqs;

        private int totalChar;

        private int[] seqCounters = new int[4];

        private int freqChar;

        private CharsetProber nameProber;

        public SingleByteCharSetProber(SequenceModel model) : this(model, false, null)
        {
        }

        public SingleByteCharSetProber(SequenceModel model, bool reversed, CharsetProber nameProber)
        {
            this.model = model;
            this.reversed = reversed;
            this.nameProber = nameProber;
            this.Reset();
        }

        public override ProbingState HandleData(byte[] buf, int offset, int len)
        {
            checked
            {
                int num = offset + len;
                for (int i = offset; i < num; i++)
                {
                    byte order = this.model.GetOrder(buf[i]);
                    if (order < 250)
                    {
                        this.totalChar++;
                    }
                    if (order < 64)
                    {
                        this.freqChar++;
                        if (this.lastOrder < 64)
                        {
                            this.totalSeqs++;
                            if (!this.reversed)
                            {
                                this.seqCounters[(int)this.model.GetPrecedence((int)(this.lastOrder * 64 + order))]++;
                            }
                            else
                            {
                                this.seqCounters[(int)this.model.GetPrecedence((int)(order * 64 + this.lastOrder))]++;
                            }
                        }
                    }
                    this.lastOrder = order;
                }
                if (this.state == ProbingState.Detecting && this.totalSeqs > 1024)
                {
                    float confidence = this.GetConfidence();
                    if (confidence > 0.95f)
                    {
                        this.state = ProbingState.FoundIt;
                    }
                    else if (confidence < 0.05f)
                    {
                        this.state = ProbingState.NotMe;
                    }
                }
                return this.state;
            }
        }

        public override void DumpStatus()
        {
            Console.WriteLine("  SBCS: {0} [{1}]", this.GetConfidence(), this.GetCharsetName());
        }

        public override float GetConfidence()
        {
            if (this.totalSeqs > 0)
            {
                float num = 1f * (float)this.seqCounters[3] / (float)this.totalSeqs / this.model.TypicalPositiveRatio;
                num = num * (float)this.freqChar / (float)this.totalChar;
                if (num >= 1f)
                {
                    num = 0.99f;
                }
                return num;
            }
            return 0.01f;
        }

        public override void Reset()
        {
            this.state = ProbingState.Detecting;
            this.lastOrder = 255;
            checked
            {
                for (int i = 0; i < 4; i++)
                {
                    this.seqCounters[i] = 0;
                }
                this.totalSeqs = 0;
                this.totalChar = 0;
                this.freqChar = 0;
            }
        }

        public override string GetCharsetName()
        {
            if (this.nameProber != null)
            {
                return this.nameProber.GetCharsetName();
            }
            return this.model.CharsetName;
        }
    }
}
