//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Ude.Core
{
    public class SBCSGroupProber : CharsetProber
    {
        private const int PROBERS_NUM = 13;

        private CharsetProber[] probers = new CharsetProber[13];

        private bool[] isActive = new bool[13];

        private int bestGuess;

        private int activeNum;

        public SBCSGroupProber()
        {
            this.probers[0] = new SingleByteCharSetProber(new Win1251Model());
            this.probers[1] = new SingleByteCharSetProber(new Koi8rModel());
            this.probers[2] = new SingleByteCharSetProber(new Latin5Model());
            this.probers[3] = new SingleByteCharSetProber(new MacCyrillicModel());
            this.probers[4] = new SingleByteCharSetProber(new Ibm866Model());
            this.probers[5] = new SingleByteCharSetProber(new Ibm855Model());
            this.probers[6] = new SingleByteCharSetProber(new Latin7Model());
            this.probers[7] = new SingleByteCharSetProber(new Win1253Model());
            this.probers[8] = new SingleByteCharSetProber(new Latin5BulgarianModel());
            this.probers[9] = new SingleByteCharSetProber(new Win1251BulgarianModel());
            HebrewProber hebrewProber = new HebrewProber();
            this.probers[10] = hebrewProber;
            this.probers[11] = new SingleByteCharSetProber(new Win1255Model(), false, hebrewProber);
            this.probers[12] = new SingleByteCharSetProber(new Win1255Model(), true, hebrewProber);
            hebrewProber.SetModelProbers(this.probers[11], this.probers[12]);
            this.Reset();
        }

        public override ProbingState HandleData(byte[] buf, int offset, int len)
        {
            byte[] array = CharsetProber.FilterWithoutEnglishLetters(buf, offset, len);
            if (array.Length == 0)
            {
                return this.state;
            }
            checked
            {
                for (int i = 0; i < 13; i++)
                {
                    if (this.isActive[i])
                    {
                        ProbingState probingState = this.probers[i].HandleData(array, 0, array.Length);
                        if (probingState == ProbingState.FoundIt)
                        {
                            this.bestGuess = i;
                            this.state = ProbingState.FoundIt;
                            break;
                        }
                        if (probingState == ProbingState.NotMe)
                        {
                            this.isActive[i] = false;
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
            checked
            {
                switch (this.state)
                {
                    case ProbingState.FoundIt:
                        return 0.99f;
                    case ProbingState.NotMe:
                        return 0.01f;
                    default:
                        for (int i = 0; i < 13; i++)
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
        }

        public override void DumpStatus()
        {
            float confidence = this.GetConfidence();
            Console.WriteLine(" SBCS Group Prober --------begin status");
            checked
            {
                for (int i = 0; i < 13; i++)
                {
                    if (!this.isActive[i])
                    {
                        Console.WriteLine(" inactive: [{0}] (i.e. confidence is too low).", this.probers[i].GetCharsetName());
                    }
                    else
                    {
                        this.probers[i].DumpStatus();
                    }
                }
                Console.WriteLine(" SBCS Group found best match [{0}] confidence {1}.", this.probers[this.bestGuess].GetCharsetName(), confidence);
            }
        }

        public override void Reset()
        {
            int num = 0;
            checked
            {
                for (int i = 0; i < 13; i++)
                {
                    if (this.probers[i] != null)
                    {
                        this.probers[i].Reset();
                        this.isActive[i] = true;
                        num++;
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
    }
}
