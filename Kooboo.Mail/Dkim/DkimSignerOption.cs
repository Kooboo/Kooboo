using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MimeKit;
using MimeKit.Cryptography;

namespace Kooboo.Mail.Dkim
{
    public class DkimSignerOption
    {
        private static object _syncLock = new object();

        private DkimSignerOption() { }

        public DkimSignerOption(string dkimSelector, string dkimDomain)
        {
            DKIMSelector = dkimSelector;
            DKIMDomain = dkimDomain;
        }

        public string DKIMSelector { get; set; } = null!;

        public string DKIMDomain { get; set; } = null!;

        public readonly static List<HeaderId> DefaultHeaderIds = new() { HeaderId.From, HeaderId.Subject, HeaderId.Date };

        public string QueryMethod { get; set; } = null!;

        public DkimCanonicalizationAlgorithm BodyCanonicalizationAlgorithm { get; set; }

        public DkimCanonicalizationAlgorithm HeaderCanonicalizationAlgorithm { get; set; }

        public DkimSignatureAlgorithm SignatureAlgorithm { get; set; }

        public EncodingConstraint MimeMessagePrepare { get; set; }

        public List<HeaderId> HeadIds { get; set; } = null!;

        public DkimSigner GetDkimSigner(DkimKeyConfiguration dkimKeyConfiguration)
        {
            if (dkimKeyConfiguration is null || dkimKeyConfiguration.Keys is null)
            {
                throw new ArgumentNullException("Dkim key is required", nameof(dkimKeyConfiguration.Keys));
            }

            lock (_syncLock)
            {
                var rsaKey = dkimKeyConfiguration.Keys()[DKIMSelector];
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(rsaKey.PrivateKey));
                var signer = new DkimSigner(stream, DKIMDomain, DKIMSelector)
                {
                    BodyCanonicalizationAlgorithm = BodyCanonicalizationAlgorithm,
                    HeaderCanonicalizationAlgorithm = HeaderCanonicalizationAlgorithm,
                    SignatureAlgorithm = SignatureAlgorithm,
                    QueryMethod = QueryMethod,
                };
                return signer;
            }
        }
        public static DkimSignerOption GetDefaultDkimSignerOption()
        {
            var dkimSignerOption = new DkimSignerOption()
            {
                BodyCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Relaxed,
                HeaderCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Relaxed,
                SignatureAlgorithm = DkimSignatureAlgorithm.RsaSha256,
                QueryMethod = "dns/txt",
                MimeMessagePrepare = EncodingConstraint.EightBit,
                HeadIds = DefaultHeaderIds
            };
            return dkimSignerOption;
        }
    }
}

