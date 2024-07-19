//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class GB18030Prober : CharsetProber
    {
        private CodingStateMachine codingSM;

        private GB18030DistributionAnalyser analyser;

        private byte[] lastChar;

        public GB18030Prober()
        {
            this.lastChar = new byte[2];
            this.codingSM = new CodingStateMachine(new GB18030SMModel());
            this.analyser = new GB18030DistributionAnalyser();
            this.Reset();
        }

        public override string GetCharsetName()
        {
            return "gb18030";
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
                            this.analyser.HandleOneChar(this.lastChar, 0, currentCharLen);
                        }
                        else
                        {
                            this.analyser.HandleOneChar(buf, i - 1, currentCharLen);
                        }
                    }
                }
                this.lastChar[0] = buf[num - 1];
                if (this.state == ProbingState.Detecting && this.analyser.GotEnoughData() && this.GetConfidence() > 0.95f)
                {
                    this.state = ProbingState.FoundIt;
                }
                return this.state;
            }
        }

        public override float GetConfidence()
        {
            return this.analyser.GetConfidence();
        }

        public override void Reset()
        {
            this.codingSM.Reset();
            this.state = ProbingState.Detecting;
            this.analyser.Reset();
        }
    }
}
