//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class EUCJPSMModel : SMModel
    {
        private static readonly int[] EUCJP_cls = new int[]
        {
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 5, 5),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 5, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(5, 5, 5, 5, 5, 5, 5, 5),
            BitPackage.Pack4bits(5, 5, 5, 5, 5, 5, 1, 3),
            BitPackage.Pack4bits(5, 5, 5, 5, 5, 5, 5, 5),
            BitPackage.Pack4bits(5, 5, 5, 5, 5, 5, 5, 5),
            BitPackage.Pack4bits(5, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 5)
        };

        private static readonly int[] EUCJP_st = new int[]
        {
            BitPackage.Pack4bits(3, 4, 3, 5, 0, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 0, 1, 0, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 0, 1, 1, 1, 3, 1),
            BitPackage.Pack4bits(3, 1, 1, 1, 0, 0, 0, 0)
        };

        private static readonly int[] EUCJPCharLenTable = new int[]
        {
            2,
            2,
            2,
            3,
            1,
            0
        };

        public EUCJPSMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, EUCJPSMModel.EUCJP_cls), 6, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, EUCJPSMModel.EUCJP_st), EUCJPSMModel.EUCJPCharLenTable, "EUC-JP")
        {
        }
    }
}
