using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
    /// <summary>
    /// This class represents MIME multipart/signed body. Defined in RFC 5751.
    /// </summary>
    public class MIME_b_MultipartSigned : MIME_b_Multipart
    {
        private X509Certificate2 m_pSignerCert = null;

        /// <summary>
        /// Default constructor. The protocol = 'application/x-pkcs7-signature',micalg = 'sha1' and boundary = auto-generated.
        /// </summary>
        public MIME_b_MultipartSigned() : base()
        {
            MIME_h_ContentType contentType_multipartSigned = new MIME_h_ContentType(MIME_MediaTypes.Multipart.signed);
            contentType_multipartSigned.Parameters["protocol"] = "application/x-pkcs7-signature";
            contentType_multipartSigned.Parameters["micalg"]   = "sha1";
            contentType_multipartSigned.Param_Boundary         = Guid.NewGuid().ToString().Replace('-','.');
            this.ContentType = contentType_multipartSigned;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="contentType">Content type.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>contentType</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public MIME_b_MultipartSigned(MIME_h_ContentType contentType) : base(contentType)
        {
            if(!string.Equals(contentType.TypeWithSubtype,"multipart/signed",StringComparison.CurrentCultureIgnoreCase)){
                throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/signed'.");
            }
        }

        #region static method Parse

        /// <summary>
        /// Parses body from the specified stream
        /// </summary>
        /// <param name="owner">Owner MIME entity.</param>
        /// <param name="defaultContentType">Default content-type for this body.</param>
        /// <param name="stream">Stream from where to read body.</param>
        /// <returns>Returns parsed body.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b>, <b>mediaTypedefaultContentTypeb></b> or <b>stream</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when any parsing errors.</exception>
        protected static new MIME_b Parse(MIME_Entity owner,MIME_h_ContentType defaultContentType,SmartStream stream)
        {
            if(owner == null){
                throw new ArgumentNullException("owner");
            }
            if(defaultContentType == null){
                throw new ArgumentNullException("defaultContentType");
            }
            if(stream == null){
                throw new ArgumentNullException("stream");
            }
            if(owner.ContentType == null || owner.ContentType.Param_Boundary == null){
                throw new ParseException("Multipart entity has not required 'boundary' paramter.");
            }
            
            MIME_b_MultipartSigned retVal = new MIME_b_MultipartSigned(owner.ContentType);
            ParseInternal(owner,owner.ContentType.TypeWithSubtype,stream,retVal);

            return retVal;
        }

        #endregion


        #region method SetCertificate

        /// <summary>
        /// Sets signer certificate.
        /// </summary>
        /// <param name="signerCert">Signer certificate.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>signerCert</b> is null reference.</exception>
        public void SetCertificate(X509Certificate2 signerCert)
        {
            if(signerCert == null){
                throw new ArgumentNullException("signerCert");
            }

            m_pSignerCert = signerCert;            
        }

        #endregion

        #region method GetCertificates

        /// <summary>
        /// Gets certificates contained in pkcs 7.
        /// </summary>
        /// <returns>Returns certificates contained in pkcs 7. Returns null if no certificates.</returns>
        public X509Certificate2Collection GetCertificates()
        {
            // multipart/signed must always have only 2 entities, otherwise invalid data.
            if(this.BodyParts.Count != 2){
                return null;
            }
                
            // Get signature. It should be 2 entity.
            MIME_Entity signatureEntity = this.BodyParts[1];

            SignedCms signedCms = new SignedCms();
            signedCms.Decode(((MIME_b_SinglepartBase)signatureEntity.Body).Data);

            return signedCms.Certificates;
        }

        #endregion

        #region method VerifySignature

        /// <summary>
        /// Checks if signature is valid and data not altered.
        /// </summary>
        /// <returns>Returns true if signature is valid, otherwise false.</returns>
        public bool VerifySignature()
        {
            // Message is signed when it's saved out.
            if(m_pSignerCert != null){
                return true;
            }
            // multipart/signed must always have only 2 entities, otherwise invalid data.
            if(this.BodyParts.Count != 2){
                return false;
            }
                
            // Get signature. It should be 2 entity.
            MIME_Entity signatureEntity = this.BodyParts[1];
                       
            // Store entity to tmp stream.              
            MemoryStream tmpDataEntityStream = new MemoryStream();
            this.BodyParts[0].ToStream(tmpDataEntityStream,null,null,false);

            try{
                SignedCms signedCms = new SignedCms(new ContentInfo(tmpDataEntityStream.ToArray()),true);
                signedCms.Decode(((MIME_b_SinglepartBase)signatureEntity.Body).Data);
                signedCms.CheckSignature(true);

                return true;
            }
            catch{
                return false;
            }
        }

        #endregion


        #region method ToStream

        /// <summary>
        /// Stores MIME entity body to the specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store body data.</param>
        /// <param name="headerWordEncoder">Header 8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="headerParmetersCharset">Charset to use to encode 8-bit header parameters. Value null means parameters not encoded.</param>
        /// <param name="headerReencode">If true always specified encoding is used for header. If false and header field value not modified, 
        /// original encoding is kept.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        internal protected override void ToStream(Stream stream,MIME_Encoding_EncodedWord headerWordEncoder,Encoding headerParmetersCharset,bool headerReencode)
        {
            // We have signer certificate, sign this entity.
            if(this.BodyParts.Count > 0 && m_pSignerCert != null){
                // Remove old signature if there is any.
                if(this.BodyParts.Count > 1){
                    this.BodyParts.Remove(1);
                }

                // Store entity to tmp stream.
                MemoryStream tmpDataEntityStream = new MemoryStream();
                this.BodyParts[0].ToStream(tmpDataEntityStream,null,null,false);
        
                // Compute PKCS #7 message.
                SignedCms signedCms = new SignedCms(new ContentInfo(tmpDataEntityStream.ToArray()),true);
                signedCms.ComputeSignature(new CmsSigner(m_pSignerCert));
                byte[] pkcs7 = signedCms.Encode();
   
                // Create PKCS 7 entity.
                MIME_Entity entity_application_pkcs7 = new MIME_Entity();
                MIME_b_Application application_pkcs7 = new MIME_b_Application(MIME_MediaTypes.Application.x_pkcs7_signature);
                entity_application_pkcs7.Body = application_pkcs7;
                application_pkcs7.SetData(new MemoryStream(pkcs7),MIME_TransferEncodings.Base64);
                entity_application_pkcs7.ContentType.Param_Name = "smime.p7s";
                entity_application_pkcs7.ContentDescription = "S/MIME Cryptographic Signature";
                this.BodyParts.Add(entity_application_pkcs7);

                signedCms.Decode(application_pkcs7.Data);
                signedCms.CheckSignature(true);
            }

            base.ToStream(stream,headerWordEncoder,headerParmetersCharset,headerReencode);
        }

        #endregion


        #region Properties implementation

        #endregion
    }
}
