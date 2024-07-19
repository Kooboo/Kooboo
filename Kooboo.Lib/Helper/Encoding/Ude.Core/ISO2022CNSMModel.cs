//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class ISO2022CNSMModel : SMModel
    {
        private static readonly int[] ISO2022CN_cls = new int[]
        {
            BitPackage.Pack4bits(2, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 1, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 3, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 4, 0, 0, 0, 0),
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

        private static readonly int[] ISO2022CN_st = new int[]
        {
            BitPackage.Pack4bits(0, 3, 1, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 1, 1, 1, 4, 1),
            BitPackage.Pack4bits(1, 1, 1, 2, 1, 1, 1, 1),
            BitPackage.Pack4bits(5, 6, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 2, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 2, 1, 0)
        };

        private static readonly int[] ISO2022CNCharLenTable;

        public ISO2022CNSMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, ISO2022CNSMModel.ISO2022CN_cls), 9, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, ISO2022CNSMModel.ISO2022CN_st), ISO2022CNSMModel.ISO2022CNCharLenTable, "ISO-2022-CN")
        {
        }

        static ISO2022CNSMModel()
        {
            // Note: this type is marked as 'beforefieldinit'.
            int[] iSO2022CNCharLenTable = new int[9];
            ISO2022CNSMModel.ISO2022CNCharLenTable = iSO2022CNCharLenTable;
        }
    }
}
