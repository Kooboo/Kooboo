//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public abstract class CharDistributionAnalyser
    {
        protected const float SURE_YES = 0.99f;

        protected const float SURE_NO = 0.01f;

        protected const int MINIMUM_DATA_THRESHOLD = 4;

        protected const int ENOUGH_DATA_THRESHOLD = 1024;

        protected bool done;

        protected int freqChars;

        protected int totalChars;

        protected int[] charToFreqOrder;

        protected float typicalDistributionRatio;

        public CharDistributionAnalyser()
        {
            this.Reset();
        }

        public abstract int GetOrder(byte[] buf, int offset);

        public void HandleOneChar(byte[] buf, int offset, int charLen)
        {
            int num = (charLen == 2) ? this.GetOrder(buf, offset) : -1;
            checked
            {
                if (num >= 0)
                {
                    this.totalChars++;
                    if (num < this.charToFreqOrder.Length && 512 > this.charToFreqOrder[num])
                    {
                        this.freqChars++;
                    }
                }
            }
        }

        public virtual void Reset()
        {
            this.done = false;
            this.totalChars = 0;
            this.freqChars = 0;
        }

        public virtual float GetConfidence()
        {
            if (this.totalChars <= 0 || this.freqChars <= 4)
            {
                return 0.01f;
            }
            if (this.totalChars != this.freqChars)
            {
                float num = (float)this.freqChars / ((float)(checked(this.totalChars - this.freqChars)) * this.typicalDistributionRatio);
                if (num < 0.99f)
                {
                    return num;
                }
            }
            return 0.99f;
        }

        public bool GotEnoughData()
        {
            return this.totalChars > 1024;
        }
    }
}
