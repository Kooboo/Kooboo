using System.Net;

namespace Kooboo.Lib.Extensions
{
    public static class IPAddressExtensions
    {
        public static bool IsInRange(this IPAddress address, IPAddress rangeStart, int subnetMaskLength)
        {
            if (address.AddressFamily != rangeStart.AddressFamily)
            {
                return false; // The address family does not match
            }

            byte[] ipBytes = address.GetAddressBytes();
            byte[] rangeStartBytes = rangeStart.GetAddressBytes();

            // Calculates the byte array of the subnet mask
            byte[] subnetMaskBytes = new byte[ipBytes.Length];
            for (int i = 0; i < subnetMaskLength; i++)
                subnetMaskBytes[i / 8] |= (byte)(1 << (7 - (i % 8)));

            // Evaluates the byte array of the CIDR range
            byte[] cidrRangeBytes = new byte[ipBytes.Length];
            for (int i = 0; i < ipBytes.Length; i++)
                cidrRangeBytes[i] = (byte)(rangeStartBytes[i] & subnetMaskBytes[i]);

            // Check whether the IP address is within the CIDR range
            byte[] ipNetworkBytes = new byte[ipBytes.Length];
            for (int i = 0; i < ipBytes.Length; i++)
                ipNetworkBytes[i] = (byte)(ipBytes[i] & subnetMaskBytes[i]);

            for (int i = 0; i < ipBytes.Length; i++)
                if (ipNetworkBytes[i] != cidrRangeBytes[i])
                    return false;

            return true;
        }
    }
}

