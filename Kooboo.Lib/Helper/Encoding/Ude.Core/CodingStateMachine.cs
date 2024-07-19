//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class CodingStateMachine
    {
        private int currentState;

        private SMModel model;

        private int currentCharLen;

        private int currentBytePos;

        public int CurrentCharLen
        {
            get
            {
                return this.currentCharLen;
            }
        }

        public string ModelName
        {
            get
            {
                return this.model.Name;
            }
        }

        public CodingStateMachine(SMModel model)
        {
            this.currentState = 0;
            this.model = model;
        }

        public int NextState(byte b)
        {
            int @class = this.model.GetClass(b);
            if (this.currentState == 0)
            {
                this.currentBytePos = 0;
                this.currentCharLen = this.model.charLenTable[@class];
            }
            checked
            {
                this.currentState = this.model.stateTable.Unpack(this.currentState * this.model.ClassFactor + @class);
                this.currentBytePos++;
                return this.currentState;
            }
        }

        public void Reset()
        {
            this.currentState = 0;
        }
    }
}
