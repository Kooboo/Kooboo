//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class EUCTWSMModel : SMModel
    {
        private static readonly int[] EUCTW_cls = new int[]
        {
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 0, 0),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 0, 2, 2, 2, 2),
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
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 6, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 3, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(5, 5, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 3, 1, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 0)
        };

        private static readonly int[] EUCTW_st = new int[]
        {
            BitPackage.Pack4bits(1, 1, 0, 3, 3, 3, 4, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 1, 0, 1),
            BitPackage.Pack4bits(0, 0, 0, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(5, 1, 1, 1, 0, 1, 0, 0),
            BitPackage.Pack4bits(0, 1, 0, 0, 0, 0, 0, 0)
        };

        private static readonly int[] EUCTWCharLenTable = new int[]
        {
            0,
            0,
            1,
            2,
            2,
            2,
            3
        };

        public EUCTWSMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, EUCTWSMModel.EUCTW_cls), 7, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, EUCTWSMModel.EUCTW_st), EUCTWSMModel.EUCTWCharLenTable, "EUC-TW")
        {
        }
    }
}
