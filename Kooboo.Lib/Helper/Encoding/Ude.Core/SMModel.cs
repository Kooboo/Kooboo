//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public abstract class SMModel
    {
        public const int START = 0;

        public const int ERROR = 1;

        public const int ITSME = 2;

        public BitPackage classTable;

        public BitPackage stateTable;

        public int[] charLenTable;

        private string name;

        private int classFactor;

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public int ClassFactor
        {
            get
            {
                return this.classFactor;
            }
        }

        public SMModel(BitPackage classTable, int classFactor, BitPackage stateTable, int[] charLenTable, string name)
        {
            this.classTable = classTable;
            this.classFactor = classFactor;
            this.stateTable = stateTable;
            this.charLenTable = charLenTable;
            this.name = name;
        }

        public int GetClass(byte b)
        {
            return this.classTable.Unpack((int)b);
        }
    }
}
