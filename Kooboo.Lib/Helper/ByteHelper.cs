namespace Kooboo.Lib.Helper
{
    public static class ByteHelper
    {

        public static bool SameValue(byte[] A, byte[] B)
        {
            if (A == null || B == null)
            {
                return false;
            }

            if (A.Length != B.Length)
            {
                return false;
            }

            var AHash = Kooboo.Lib.Security.Hash.ComputeGuid(A);
            var BHash = Kooboo.Lib.Security.Hash.ComputeGuid(B);

            return AHash == BHash;
        }
    }
}
