//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class UTF8SMModel : SMModel
    {
        private static readonly int[] UTF8_cls = new int[]
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
            BitPackage.Pack4bits(2, 2, 2, 2, 3, 3, 3, 3),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(4, 4, 4, 4, 4, 4, 4, 4),
            BitPackage.Pack4bits(5, 5, 5, 5, 5, 5, 5, 5),
            BitPackage.Pack4bits(5, 5, 5, 5, 5, 5, 5, 5),
            BitPackage.Pack4bits(5, 5, 5, 5, 5, 5, 5, 5),
            BitPackage.Pack4bits(5, 5, 5, 5, 5, 5, 5, 5),
            BitPackage.Pack4bits(0, 0, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(7, 8, 8, 8, 8, 8, 8, 8),
            BitPackage.Pack4bits(8, 8, 8, 8, 8, 9, 8, 8),
            BitPackage.Pack4bits(10, 11, 11, 11, 11, 11, 11, 11),
            BitPackage.Pack4bits(12, 13, 13, 13, 14, 15, 0, 0)
        };

        private static readonly int[] UTF8_st = new int[]
        {
            BitPackage.Pack4bits(1, 0, 1, 1, 1, 1, 12, 10),
            BitPackage.Pack4bits(9, 11, 8, 7, 6, 5, 4, 3),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(1, 1, 5, 5, 5, 5, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 5, 5, 5, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 7, 7, 7, 7, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 7, 7, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 9, 9, 9, 9, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 9, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 12, 12, 12, 12, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 12, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 12, 12, 12, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 0, 0, 0, 0, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1)
        };

        private static readonly int[] UTF8CharLenTable = new int[]
        {
            0,
            1,
            0,
            0,
            0,
            0,
            2,
            3,
            3,
            3,
            4,
            4,
            5,
            5,
            6,
            6
        };

        public UTF8SMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, UTF8SMModel.UTF8_cls), 16, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, UTF8SMModel.UTF8_st), UTF8SMModel.UTF8CharLenTable, "UTF-8")
        {
        }
    }
}
