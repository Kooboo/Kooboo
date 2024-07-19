//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public abstract class SequenceModel
    {
        protected byte[] charToOrderMap;

        protected byte[] precedenceMatrix;

        protected float typicalPositiveRatio;

        protected bool keepEnglishLetter;

        protected string charsetName;

        public float TypicalPositiveRatio
        {
            get
            {
                return this.typicalPositiveRatio;
            }
        }

        public bool KeepEnglishLetter
        {
            get
            {
                return this.keepEnglishLetter;
            }
        }

        public string CharsetName
        {
            get
            {
                return this.charsetName;
            }
        }

        public SequenceModel(byte[] charToOrderMap, byte[] precedenceMatrix, float typicalPositiveRatio, bool keepEnglishLetter, string charsetName)
        {
            this.charToOrderMap = charToOrderMap;
            this.precedenceMatrix = precedenceMatrix;
            this.typicalPositiveRatio = typicalPositiveRatio;
            this.keepEnglishLetter = keepEnglishLetter;
            this.charsetName = charsetName;
        }

        public byte GetOrder(byte b)
        {
            return this.charToOrderMap[(int)b];
        }

        public byte GetPrecedence(int pos)
        {
            return this.precedenceMatrix[pos];
        }
    }
}
