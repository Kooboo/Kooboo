//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class UTF8Prober : CharsetProber
    {
        private static float ONE_CHAR_PROB = 0.5f;

        private CodingStateMachine codingSM;

        private int numOfMBChar;

        public UTF8Prober()
        {
            this.numOfMBChar = 0;
            this.codingSM = new CodingStateMachine(new UTF8SMModel());
            this.Reset();
        }

        public override string GetCharsetName()
        {
            return "UTF-8";
        }

        public override void Reset()
        {
            this.codingSM.Reset();
            this.numOfMBChar = 0;
            this.state = ProbingState.Detecting;
        }

        public override ProbingState HandleData(byte[] buf, int offset, int len)
        {
            checked
            {
                int num = offset + len;
                for (int i = offset; i < num; i++)
                {
                    int num2 = this.codingSM.NextState(buf[i]);
                    if (num2 == 1)
                    {
                        this.state = ProbingState.NotMe;
                        break;
                    }
                    if (num2 == 2)
                    {
                        this.state = ProbingState.FoundIt;
                        break;
                    }
                    if (num2 == 0 && this.codingSM.CurrentCharLen >= 2)
                    {
                        this.numOfMBChar++;
                    }
                }
                if (this.state == ProbingState.Detecting && this.GetConfidence() > 0.95f)
                {
                    this.state = ProbingState.FoundIt;
                }
                return this.state;
            }
        }

        public override float GetConfidence()
        {
            float num = 0.99f;
            float result;
            if (this.numOfMBChar < 6)
            {
                for (int i = 0; i < this.numOfMBChar; i = checked(i + 1))
                {
                    num *= UTF8Prober.ONE_CHAR_PROB;
                }
                result = 1f - num;
            }
            else
            {
                result = 0.99f;
            }
            return result;
        }
    }
}
