//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Ude.Core
{
    public class UCS2LESMModel : SMModel
    {
        private static readonly int[] UCS2LE_cls = new int[]
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

        private static readonly int[] UCS2LE_st = new int[]
        {
            BitPackage.Pack4bits(6, 6, 7, 6, 4, 3, 1, 1),
            BitPackage.Pack4bits(1, 1, 1, 1, 2, 2, 2, 2),
            BitPackage.Pack4bits(2, 2, 5, 5, 5, 1, 2, 1),
            BitPackage.Pack4bits(5, 5, 5, 1, 5, 1, 6, 6),
            BitPackage.Pack4bits(7, 6, 8, 8, 5, 5, 5, 1),
            BitPackage.Pack4bits(5, 5, 5, 1, 1, 1, 5, 5),
            BitPackage.Pack4bits(5, 5, 5, 1, 5, 1, 0, 0)
        };

        private static readonly int[] UCS2LECharLenTable = new int[]
        {
            2,
            2,
            2,
            2,
            2,
            2
        };

        public UCS2LESMModel() : base(new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, UCS2LESMModel.UCS2LE_cls), 6, new BitPackage(BitPackage.INDEX_SHIFT_4BITS, BitPackage.SHIFT_MASK_4BITS, BitPackage.BIT_SHIFT_4BITS, BitPackage.UNIT_MASK_4BITS, UCS2LESMModel.UCS2LE_st), UCS2LESMModel.UCS2LECharLenTable, "UTF-16LE")
        {
        }
    }
}
