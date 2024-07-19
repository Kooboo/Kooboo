//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class EUCKRSMModel : SMModel
    {
        private static readonly int[] EUCKR_cls = new int[]
        {
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 0, 0),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 0, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 3, 3, 3),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 3, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 0)
        };

        private static readonly int[] EUCKR_st = new int[]
        {
            BitPackage.Pack4bits(1, 0, 3, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(2, 2, 2, 2, 1, 1, 0, 0)
        };

        private static readonly int[] EUCKRCharLenTable;

        public EUCKRSMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, EUCKRSMModel.EUCKR_cls), 4, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, EUCKRSMModel.EUCKR_st), EUCKRSMModel.EUCKRCharLenTable, "EUC-KR")
        {
        }

        static EUCKRSMModel()
        {
            // Note: this type is marked as 'beforefieldinit'.
            int[] array = new int[4];
            array[1] = 1;
            array[2] = 2;
            EUCKRSMModel.EUCKRCharLenTable = array;
        }
    }
}
