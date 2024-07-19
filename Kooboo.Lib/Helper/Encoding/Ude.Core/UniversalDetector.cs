//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public abstract class UniversalDetector
    {
        protected const int FILTER_CHINESE_SIMPLIFIED = 1;

        protected const int FILTER_CHINESE_TRADITIONAL = 2;

        protected const int FILTER_JAPANESE = 4;

        protected const int FILTER_KOREAN = 8;

        protected const int FILTER_NON_CJK = 16;

        protected const int FILTER_ALL = 31;

        protected const float SHORTCUT_THRESHOLD = 0.95f;

        protected const float MINIMUM_THRESHOLD = 0.2f;

        protected const int PROBERS_NUM = 3;

        protected static int FILTER_CHINESE = 3;

        protected static int FILTER_CJK = 15;

        internal InputState inputState;

        protected bool start;

        protected bool gotData;

        protected bool done;

        protected byte lastChar;

        protected int bestGuess;

        protected int languageFilter;

        protected CharsetProber[] charsetProbers = new CharsetProber[3];

        protected CharsetProber escCharsetProber;

        protected string detectedCharset;

        public UniversalDetector(int languageFilter)
        {
            this.start = true;
            this.inputState = InputState.PureASCII;
            this.lastChar = 0;
            this.bestGuess = -1;
            this.languageFilter = languageFilter;
        }

        public virtual void Feed(byte[] buf, int offset, int len)
        {
            if (this.done)
            {
                return;
            }
            if (len > 0)
            {
                this.gotData = true;
            }
            if (this.start)
            {
                this.start = false;
                if (len > 3)
                {
                    byte b = buf[0];
                    if (b != 0)
                    {
                        if (b != 239)
                        {
                            switch (b)
                            {
                                case 254:
                                    if (255 == buf[1] && buf[2] == 0 && buf[3] == 0)
                                    {
                                        this.detectedCharset = "X-ISO-10646-UCS-4-3412";
                                    }
                                    else if (255 == buf[1])
                                    {
                                        this.detectedCharset = "UTF-16BE";
                                    }
                                    break;
                                case 255:
                                    if (254 == buf[1] && buf[2] == 0 && buf[3] == 0)
                                    {
                                        this.detectedCharset = "UTF-32LE";
                                    }
                                    else if (254 == buf[1])
                                    {
                                        this.detectedCharset = "UTF-16LE";
                                    }
                                    break;
                            }
                        }
                        else if (187 == buf[1] && 191 == buf[2])
                        {
                            this.detectedCharset = "UTF-8";
                        }
                    }
                    else if (buf[1] == 0 && 254 == buf[2] && 255 == buf[3])
                    {
                        this.detectedCharset = "UTF-32BE";
                    }
                    else if (buf[1] == 0 && 255 == buf[2] && 254 == buf[3])
                    {
                        this.detectedCharset = "X-ISO-10646-UCS-4-2143";
                    }
                }
                if (this.detectedCharset != null)
                {
                    this.done = true;
                    return;
                }
            }
            checked
            {
                for (int i = 0; i < len; i++)
                {
                    if ((buf[i] & 128) != 0 && buf[i] != 160)
                    {
                        if (this.inputState != InputState.Highbyte)
                        {
                            this.inputState = InputState.Highbyte;
                            if (this.escCharsetProber != null)
                            {
                                this.escCharsetProber = null;
                            }
                            if (this.charsetProbers[0] == null)
                            {
                                this.charsetProbers[0] = new MBCSGroupProber();
                            }
                            if (this.charsetProbers[1] == null)
                            {
                                this.charsetProbers[1] = new SBCSGroupProber();
                            }
                            if (this.charsetProbers[2] == null)
                            {
                                this.charsetProbers[2] = new Latin1Prober();
                            }
                        }
                    }
                    else
                    {
                        if (this.inputState == InputState.PureASCII && (buf[i] == 27 || (buf[i] == 123 && this.lastChar == 126)))
                        {
                            this.inputState = InputState.EscASCII;
                        }
                        this.lastChar = buf[i];
                    }
                }
                switch (this.inputState)
                {
                    case InputState.EscASCII:
                        {
                            if (this.escCharsetProber == null)
                            {
                                this.escCharsetProber = new EscCharsetProber();
                            }
                            ProbingState probingState = this.escCharsetProber.HandleData(buf, offset, len);
                            if (probingState == ProbingState.FoundIt)
                            {
                                this.done = true;
                                this.detectedCharset = this.escCharsetProber.GetCharsetName();
                                return;
                            }
                            break;
                        }
                    case InputState.Highbyte:
                        for (int j = 0; j < 3; j++)
                        {
                            if (this.charsetProbers[j] != null)
                            {
                                ProbingState probingState = this.charsetProbers[j].HandleData(buf, offset, len);
                                if (probingState == ProbingState.FoundIt)
                                {
                                    this.done = true;
                                    this.detectedCharset = this.charsetProbers[j].GetCharsetName();
                                    return;
                                }
                            }
                        }
                        break;
                    default:
                        return;
                }
            }
        }

        public virtual void DataEnd()
        {
            if (!this.gotData)
            {
                return;
            }
            if (this.detectedCharset != null)
            {
                this.done = true;
                this.Report(this.detectedCharset, 1f);
                return;
            }
            checked
            {
                if (this.inputState == InputState.Highbyte)
                {
                    float num = 0f;
                    int num2 = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        if (this.charsetProbers[i] != null)
                        {
                            float confidence = this.charsetProbers[i].GetConfidence();
                            if (confidence > num)
                            {
                                num = confidence;
                                num2 = i;
                            }
                        }
                    }
                    if (num > 0.2f)
                    {
                        this.Report(this.charsetProbers[num2].GetCharsetName(), num);
                        return;
                    }
                }
                else if (this.inputState == InputState.PureASCII)
                {
                    this.Report("ASCII", 1f);
                }
            }
        }

        public virtual void Reset()
        {
            this.done = false;
            this.start = true;
            this.detectedCharset = null;
            this.gotData = false;
            this.bestGuess = -1;
            this.inputState = InputState.PureASCII;
            this.lastChar = 0;
            if (this.escCharsetProber != null)
            {
                this.escCharsetProber.Reset();
            }
            checked
            {
                for (int i = 0; i < 3; i++)
                {
                    if (this.charsetProbers[i] != null)
                    {
                        this.charsetProbers[i].Reset();
                    }
                }
            }
        }

        protected abstract void Report(string charset, float confidence);
    }
}
