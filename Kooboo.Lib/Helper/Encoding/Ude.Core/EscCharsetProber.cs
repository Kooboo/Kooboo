//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class EscCharsetProber : CharsetProber
    {
        private const int CHARSETS_NUM = 4;

        private string detectedCharset;

        private CodingStateMachine[] codingSM;

        private int activeSM;

        public EscCharsetProber()
        {
            this.codingSM = new CodingStateMachine[4];
            this.codingSM[0] = new CodingStateMachine(new HZSMModel());
            this.codingSM[1] = new CodingStateMachine(new ISO2022CNSMModel());
            this.codingSM[2] = new CodingStateMachine(new ISO2022JPSMModel());
            this.codingSM[3] = new CodingStateMachine(new ISO2022KRSMModel());
            this.Reset();
        }

        public override void Reset()
        {
            this.state = ProbingState.Detecting;
            checked
            {
                for (int i = 0; i < 4; i++)
                {
                    this.codingSM[i].Reset();
                }
                this.activeSM = 4;
                this.detectedCharset = null;
            }
        }

        public override ProbingState HandleData(byte[] buf, int offset, int len)
        {
            checked
            {
                int num = offset + len;
                int num2 = offset;
                while (num2 < num && this.state == ProbingState.Detecting)
                {
                    for (int i = this.activeSM - 1; i >= 0; i--)
                    {
                        int num3 = this.codingSM[i].NextState(buf[num2]);
                        if (num3 == 1)
                        {
                            this.activeSM--;
                            if (this.activeSM == 0)
                            {
                                this.state = ProbingState.NotMe;
                                return this.state;
                            }
                            if (i != this.activeSM)
                            {
                                CodingStateMachine codingStateMachine = this.codingSM[this.activeSM];
                                this.codingSM[this.activeSM] = this.codingSM[i];
                                this.codingSM[i] = codingStateMachine;
                            }
                        }
                        else if (num3 == 2)
                        {
                            this.state = ProbingState.FoundIt;
                            this.detectedCharset = this.codingSM[i].ModelName;
                            return this.state;
                        }
                    }
                    num2++;
                }
                return this.state;
            }
        }

        public override string GetCharsetName()
        {
            return this.detectedCharset;
        }

        public override float GetConfidence()
        {
            return 0.99f;
        }
    }
}
