namespace Kooboo.Mail.SecurityControl
{
    public static class Manager
    {


        public static SecurityCheckResult Check(System.Net.IPAddress ip)
        {
            string ip4 = null;
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                ip4 = ip.MapToIPv4().ToString();
            }
            else
            {
                ip4 = ip.ToString();
            }

            if (BlackList.IsBlacklisted(ip4))
            {
                return new SecurityCheckResult() { CanConnect = false, Error = "too many errors, IP temporarily lock" };
            }

            if (!ConnectionSpeed.CanConnect(ip4, ip))
            {
                return new SecurityCheckResult() { CanConnect = false, Error = "Connect too fast, 1 mail per second please" };
            }

            return new SecurityCheckResult() { CanConnect = true };


        }
    }
}
