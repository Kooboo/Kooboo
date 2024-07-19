using System;
using System.Collections.Generic;
using Kooboo.Mail.DnsQuery;
using MimeKit;
using MimeKit.Cryptography;

namespace Kooboo.Mail.Dkim
{
    public class KDkimVerifier
    {
        private readonly IDnsRequest _dnsRequest = new DnsRequest();
        List<string> DnsLookupText(string domain, string selector)
        {
            var FullDomain = selector + "._domainkey." + domain;
            return _dnsRequest.GetTxtRecords(FullDomain);
        }

        private string GetPublicKey(List<string> textRecords)
        {
            /*
 *  two._domainkey.abc.com subdomain  
 v=DKIM1;t=s;p=MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCWE3zy2aS/rFl4S+E1vBqj6YMqaohp7wpyxP4RzpuZ4epNp9ZOO7AncdllGFTRDCeQhenZh7EzW3/T/fqwYsAYg61tWdlQbEVqKVX1E0rkctlg5wTcGEkxeQguvsH9K/35WT6DgXlQE/AzteePk9BhANZHj2yW2ELxWriWkUH0zwIDAQAB
 */
            foreach (var item in textRecords)
            {
                if (item != null && item.Contains("DKIM", StringComparison.InvariantCultureIgnoreCase))
                {
                    var parts = item.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        var PartWithoutSpace = part.Replace(" ", "");
                        if (PartWithoutSpace.StartsWith("p=") || PartWithoutSpace.StartsWith("P="))
                        {
                            var key = PartWithoutSpace.Substring(2);
                            if (key != null && key.Length > 10)
                            {
                                return key;
                            }
                        }
                    }

                }

            }

            return null;
        }

        public DkimValidateResult Verify(string messagebody)
        {
            var message = Utility.MailKitUtility.LoadMessage(messagebody);
            return Verify(message);
        }

        public DkimValidateResult Verify(MimeMessage message)
        {
            var verifier = new DkimVerifier(new KeyLocator());
            var index = message.Headers.IndexOf(HeaderId.DkimSignature);
            if (index == -1)
            {
                return DkimValidateResult.NoResult;
            }
            var dkim = message.Headers[index];
            if (verifier.Verify(message, dkim))
            {
                return DkimValidateResult.Pass;
            }
            else
            {
                return DkimValidateResult.Failed;
            }
        }
    }

}

