//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class EUCTWProber : CharsetProber
    {
        private CodingStateMachine codingSM;

        private EUCTWDistributionAnalyser distributionAnalyser;

        private byte[] lastChar = new byte[2];

        public EUCTWProber()
        {
            this.codingSM = new CodingStateMachine(new EUCTWSMModel());
            this.distributionAnalyser = new EUCTWDistributionAnalyser();
            this.Reset();
        }

        public override ProbingState HandleData(byte[] buf, int offset, int len)
        {
            checked
            {
                int num = offset + len;
                for (int i = 0; i < num; i++)
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
                    if (num2 == 0)
                    {
                        int currentCharLen = this.codingSM.CurrentCharLen;
                        if (i == offset)
                        {
                            this.lastChar[1] = buf[offset];
                            this.distributionAnalyser.HandleOneChar(this.lastChar, 0, currentCharLen);
                        }
                        else
                        {
                            this.distributionAnalyser.HandleOneChar(buf, i - 1, currentCharLen);
                        }
                    }
                }
                this.lastChar[0] = buf[num - 1];
                if (this.state == ProbingState.Detecting && this.distributionAnalyser.GotEnoughData() && this.GetConfidence() > 0.95f)
                {
                    this.state = ProbingState.FoundIt;
                }
                return this.state;
            }
        }

        public override string GetCharsetName()
        {
            return "EUC-TW";
        }

        public override void Reset()
        {
            this.codingSM.Reset();
            this.state = ProbingState.Detecting;
            this.distributionAnalyser.Reset();
        }

        public override float GetConfidence()
        {
            return this.distributionAnalyser.GetConfidence();
        }
    }
}
