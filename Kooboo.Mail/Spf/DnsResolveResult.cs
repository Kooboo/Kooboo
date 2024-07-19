namespace Kooboo.Mail.Spf
{
    public class DnsResolveResult<T>
    {
        public DnsResolveResult(ReturnCode returnCode, T records)
        {
            ReturnCode = returnCode;
            Records = records;
        }

        public ReturnCode ReturnCode { get; }

        public T Records { get; }
    }
}

