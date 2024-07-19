//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class UCS2BESMModel : SMModel
    {
        private static readonly int[] UCS2BE_cls = new int[]
        {
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 1, 0, 0, 2, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 3, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 3, 3, 3, 3, 3, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 0, 0),
            BitPackage.Pack4bits(0, 0, 0, 0, 0, 0, 4, 5)
        };

        private static readonly int[] UCS2BE_st = new int[]
        {
            BitPackage.Pack4bits(5, 7, 7, 1, 4, 3, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 6, 6, 6, 6, 1, 1),
            BitPackage.Pack4bits(6, 6, 6, 6, 6, 2, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 5, 7, 7, 1),
            BitPackage.Pack4bits(5, 8, 6, 6, 1, 6, 6, 6),
            BitPackage.Pack4bits(6, 6, 6, 6, 1, 1, 0, 0)
        };

        private static readonly int[] UCS2BECharLenTable = new int[]
        {
            2,
            2,
            2,
            0,
            2,
            2
        };

        public UCS2BESMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, UCS2BESMModel.UCS2BE_cls), 6, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, UCS2BESMModel.UCS2BE_st), UCS2BESMModel.UCS2BECharLenTable, "UTF-16BE")
        {
        }
    }
}
