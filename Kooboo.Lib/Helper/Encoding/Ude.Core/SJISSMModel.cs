//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class SJISSMModel : SMModel
    {
        private static readonly int[] SJIS_cls = new int[]
        {
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 0, 0),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 0, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 1),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 0, 0, 0)
        };

        private static readonly int[] SJIS_st = new int[]
        {
            BitPackage.Pack4bits(1, 0, 0, 3, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 1, 1, 0, 0, 0, 0)
        };

        private static readonly int[] SJISCharLenTable = new int[]
        {
            0,
            1,
            1,
            2,
            0,
            0
        };

        public SJISSMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, SJISSMModel.SJIS_cls), 6, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, SJISSMModel.SJIS_st), SJISSMModel.SJISCharLenTable, "Shift_JIS")
        {
        }
    }
}
