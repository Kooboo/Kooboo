//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class SJISProber : CharsetProber
    {
        private CodingStateMachine codingSM;

        private SJISContextAnalyser contextAnalyser;

        private SJISDistributionAnalyser distributionAnalyser;

        private byte[] lastChar = new byte[2];

        public SJISProber()
        {
            this.codingSM = new CodingStateMachine(new SJISSMModel());
            this.distributionAnalyser = new SJISDistributionAnalyser();
            this.contextAnalyser = new SJISContextAnalyser();
            this.Reset();
        }

        public override string GetCharsetName()
        {
            return "Shift-JIS";
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
                    if (num2 == 0)
                    {
                        int currentCharLen = this.codingSM.CurrentCharLen;
                        if (i == offset)
                        {
                            this.lastChar[1] = buf[offset];
                            this.contextAnalyser.HandleOneChar(this.lastChar, 2 - currentCharLen, currentCharLen);
                            this.distributionAnalyser.HandleOneChar(this.lastChar, 0, currentCharLen);
                        }
                        else
                        {
                            this.contextAnalyser.HandleOneChar(buf, i + 1 - currentCharLen, currentCharLen);
                            this.distributionAnalyser.HandleOneChar(buf, i - 1, currentCharLen);
                        }
                    }
                }
                this.lastChar[0] = buf[num - 1];
                if (this.state == ProbingState.Detecting && this.contextAnalyser.GotEnoughData() && this.GetConfidence() > 0.95f)
                {
                    this.state = ProbingState.FoundIt;
                }
                return this.state;
            }
        }

        public override void Reset()
        {
            this.codingSM.Reset();
            this.state = ProbingState.Detecting;
            this.contextAnalyser.Reset();
            this.distributionAnalyser.Reset();
        }

        public override float GetConfidence()
        {
            float confidence = this.contextAnalyser.GetConfidence();
            float confidence2 = this.distributionAnalyser.GetConfidence();
            if (confidence <= confidence2)
            {
                return confidence2;
            }
            return confidence;
        }
    }
}
