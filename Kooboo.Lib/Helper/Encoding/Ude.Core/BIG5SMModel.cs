//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class BIG5SMModel : SMModel
    {
        private static readonly int[] BIG5_cls = new int[]
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
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 0)
        };

        private static readonly int[] BIG5_st = new int[]
        {
            BitPackage.Pack4bits(1, 0, 0, 3, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 2, 2, 2, 2, 2, 1),
            BitPackage.Pack4bits(1, 0, 0, 0, 0, 0, 0, 0)
        };

        private static readonly int[] BIG5CharLenTable = new int[]
        {
            0,
            1,
            1,
            2,
            0
        };

        public BIG5SMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, BIG5SMModel.BIG5_cls), 5, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, BIG5SMModel.BIG5_st), BIG5SMModel.BIG5CharLenTable, "Big5")
        {
        }
    }
}
