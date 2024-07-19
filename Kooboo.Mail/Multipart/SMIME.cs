using System.Security.Cryptography.X509Certificates;
using MimeKit;

namespace Kooboo.Mail.Multipart
{
    public class SMIME
    {

        public void MultipartSign(MimeMessage message)
        {
            // digitally sign our message body using our custom S/MIME cryptography context
            //using (var ctx = new SecureMimeContext())
            //{
            //	// Note: this assumes that the Sender address has an S/MIME signing certificate
            //	// and private key with an X.509 Subject Email identifier that matches the
            //	// sender's email address.
            //	var sender = message.From.Mailboxes.FirstOrDefault(); 
            //	message.Body = MultipartSigned.Create(ctx, sender, DigestAlgorithm.Sha1, message.Body);
            //}
        }

        public void MultipartSign(MimeMessage message, X509Certificate2 certificate)
        {
            // digitally sign our message body using our custom S/MIME cryptography context
            //using (var ctx = new MySecureMimeContext())
            //{
            //	var signer = new CmsSigner(certificate)
            //	{
            //		DigestAlgorithm = DigestAlgorithm.Sha1
            //	};

            //	message.Body = MultipartSigned.Create(ctx, signer, message.Body);
            //}
        }

    }
}