//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class ISO2022KRSMModel : SMModel
    {
        private static readonly int[] ISO2022KR_cls = new int[]
        {
            BitPackage.Pack4bits(2, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 1, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 3, 0, 0, 0),
            BitPackage.Pack4bits(0, 4, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 5, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2)
        };

        private static readonly int[] ISO2022KR_st = new int[]
        {
            BitPackage.Pack4bits(0, 3, 1, 0, 0, 0, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 1, 1, 1, 4, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 5, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 2, 0, 0, 0, 0)
        };

        private static readonly int[] ISO2022KRCharLenTable;

        public ISO2022KRSMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, ISO2022KRSMModel.ISO2022KR_cls), 6, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, ISO2022KRSMModel.ISO2022KR_st), ISO2022KRSMModel.ISO2022KRCharLenTable, "ISO-2022-KR")
        {
        }

        static ISO2022KRSMModel()
        {
            // Note: this type is marked as 'beforefieldinit'.
            int[] iSO2022KRCharLenTable = new int[6];
            ISO2022KRSMModel.ISO2022KRCharLenTable = iSO2022KRCharLenTable;
        }
    }
}
