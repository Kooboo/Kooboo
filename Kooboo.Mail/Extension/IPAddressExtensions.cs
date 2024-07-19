using System;
using System.Net;
using System.Net.Sockets;

namespace Kooboo.Mail.Extension
{
    public static class IPAddressExtensions
    {
        /// <summary>
        ///   Gets the network address for a specified IPAddress and netmask
        /// </summary>
        /// <param name="ipAddress"> IPAddress, for that the network address should be returned </param>
        /// <param name="netmask"> Netmask in CIDR format </param>
        /// <returns> New instance of IPAddress with the network address assigend </returns>
        public static IPAddress GetNetworkAddress(this IPAddress ipAddress, int netmask)
        {
            _ = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));

            if ((ipAddress.AddressFamily == AddressFamily.InterNetwork) && ((netmask < 0) || (netmask > 32)))
                throw new ArgumentException("Netmask have to be in range of 0 to 32 on IPv4 addresses", nameof(netmask));

            if ((ipAddress.AddressFamily == AddressFamily.InterNetworkV6) && ((netmask < 0) || (netmask > 128)))
                throw new ArgumentException("Netmask have to be in range of 0 to 128 on IPv6 addresses", nameof(netmask));

            byte[] ipAddressBytes = ipAddress.GetAddressBytes();

            for (int i = 0; i < ipAddressBytes.Length; i++)
            {
                if (netmask >= 8)
                {
                    netmask -= 8;
                }
                else
                {
                    if (BitConverter.IsLittleEndian)
                    {
                        ipAddressBytes[i] &= ReverseBitOrder((byte)~(255 << netmask));
                    }

                    netmask = 0;
                }
            }

            return new IPAddress(ipAddressBytes);
        }

        private static byte ReverseBitOrder(byte value)
        {
            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                result |= (byte)((((1 << i) & value) >> i) << (7 - i));
            }

            return result;
        }
    }
}