//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class ISO2022JPSMModel : SMModel
    {
        private static readonly int[] ISO2022JP_cls = new int[]
        {
            BitPackage.Pack4bits(2, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 2, 2),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 1, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 7, 0, 0, 0),
            BitPackage.Pack4bits(3, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(6, 0, 4, 0, 8, 0, 0, 0),
            BitPackage.Pack4bits(0, 9, 5, 0, 0, 0, 0, 0),
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

        private static readonly int[] ISO2022JP_st = new int[]
        {
            BitPackage.Pack4bits(0, 3, 1, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 1, 1),
            BitPackage.Pack4bits(1, 5, 1, 1, 1, 4, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 6, 2, 1, 2, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 2, 2),
            BitPackage.Pack4bits(1, 1, 1, 2, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 2, 1, 0, 0)
        };

        private static readonly int[] ISO2022JPCharLenTable;

        public ISO2022JPSMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, ISO2022JPSMModel.ISO2022JP_cls), 10, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, ISO2022JPSMModel.ISO2022JP_st), ISO2022JPSMModel.ISO2022JPCharLenTable, "ISO-2022-JP")
        {
        }

        static ISO2022JPSMModel()
        {
            // Note: this type is marked as 'beforefieldinit'.
            int[] iSO2022JPCharLenTable = new int[10];
            ISO2022JPSMModel.ISO2022JPCharLenTable = iSO2022JPCharLenTable;
        }
    }
}
