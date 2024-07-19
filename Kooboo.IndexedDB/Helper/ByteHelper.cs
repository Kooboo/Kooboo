using System;

namespace Kooboo.IndexedDB.Helper
{
    public static class ByteHelper
    {
        public static byte SetBit(byte oneByte, int BitPosition, bool value)
        {
            if (value)
            {
                return SetBit(oneByte, BitPosition);
            }
            else
            {
                return UnsetBit(oneByte, BitPosition);
            }
        }

        public static bool GetBit(byte oneByte, int BitPosition)
        {
            return (oneByte & (1 << BitPosition)) != 0;
        }

        private static byte SetBit(byte b, int pos)
        {
            return (byte)(b | (1 << pos));
        }

        private static byte UnsetBit(byte b, int pos)
        {
            return (byte)(b & ~(1 << pos));
        }

        public static byte[] IntToThreeBytes(int value)
        {
            byte[] result = new byte[3];

            if (value <= Int16.MaxValue)
            {
                var bytes = BitConverter.GetBytes((Int16)value);
                result[0] = bytes[0];
                result[1] = bytes[1];
                result[2] = 0;
                return result;
            }
            else if (value < Int16.MaxValue * 99)
            {
                var strNumber = value.ToString();
                var FirstPart = strNumber.Substring(0, strNumber.Length - 2);
                var LastTwo = strNumber.Substring(strNumber.Length - 2);

                if (LastTwo.StartsWith("0"))
                {
                    LastTwo = "1" + LastTwo;
                }

                byte LastByte = byte.Parse(LastTwo);
                Int16 NextValue = Int16.Parse(FirstPart);

                var FirstTwoBytes = BitConverter.GetBytes(NextValue);

                result[0] = FirstTwoBytes[0];
                result[1] = FirstTwoBytes[1];
                result[2] = LastByte;
                return result;
            }
            else
            {
                //this is too big... can not handle. 
                return null;
            }
        }

        public static int ThreeBytesToInt(byte[] ThreeBytes)
        {
            var FirstValue = BitConverter.ToInt16(ThreeBytes, 0);

            var TailingByte = ThreeBytes[2];

            if (TailingByte == 0)
            {
                return FirstValue;
            }
            else
            {
                string tailingValue = TailingByte.ToString();

                if (tailingValue.Length > 2)
                {
                    tailingValue = tailingValue.Substring(1);
                }

                var strResult = FirstValue.ToString() + tailingValue;

                return int.Parse(strResult);
            }
        }

        public static int ThreeBytesToInt(byte[] ThreeBytes, int startPos)
        {
            var FirstValue = BitConverter.ToInt16(ThreeBytes, startPos);

            var TailingByte = ThreeBytes[startPos + 2];

            if (TailingByte == 0)
            {
                return FirstValue;
            }
            else
            {
                string tailingValue = TailingByte.ToString();

                if (tailingValue.Length > 2)
                {
                    tailingValue = tailingValue.Substring(1);
                }

                var strResult = FirstValue.ToString() + tailingValue;

                return int.Parse(strResult);
            }
        }


    }
}
