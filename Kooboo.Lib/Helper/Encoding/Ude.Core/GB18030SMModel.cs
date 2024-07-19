//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class GB18030SMModel : SMModel
    {
        private static readonly int[] GB18030_cls = new int[]
        {
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 0, 0),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 0, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(3, 3, 3, 3, 3, 3, 3, 3),
            BitPackage.Pack4bits(3, 3, 1, 1, 1, 1, 1, 1),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 2, 2, 4),
            BitPackage.Pack4bits(5, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 6, 6, 0)
        };

        private static readonly int[] GB18030_st = new int[]
        {
            BitPackage.Pack4bits(1, 0, 0, 0, 0, 0, 3, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 1, 1, 2, 2),
            BitPackage.Pack4bits(2, 2, 2, 2, 2, 1, 1, 0),
            BitPackage.Pack4bits(4, 1, 0, 0, 1, 1, 1, 1),
            BitPackage.Pack4bits(1, 1, 5, 1, 1, 1, 2, 1),
            BitPackage.Pack4bits(1, 1, 0, 0, 0, 0, 0, 0)
        };

        private static readonly int[] GB18030CharLenTable = new int[]
        {
            0,
            1,
            1,
            1,
            1,
            1,
            2
        };

        public GB18030SMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, GB18030SMModel.GB18030_cls), 7, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, GB18030SMModel.GB18030_st), GB18030SMModel.GB18030CharLenTable, "GB18030")
        {
        }
    }
}
