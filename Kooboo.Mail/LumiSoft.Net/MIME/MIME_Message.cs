using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;

using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
    /// <summary>
    /// Represents a MIME message. Defined in RFC 2045 2.3.
    /// </summary>
    public class MIME_Message : MIME_Entity
    {
        private bool m_IsDisposed = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MIME_Message()
        {
        }


        #region static method ParseFromFile

        /// <summary>
        /// Parses MIME message from the specified file.
        /// </summary>
        /// <param name="file">File name with path from where to parse MIME message.</param>
        /// <returns>Returns parsed MIME message.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>file</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public static MIME_Message ParseFromFile(string file)
        {
            if(file == null){
                throw new ArgumentNullException("file");
            }
            if(file == ""){
                throw new ArgumentException("Argument 'file' value must be specified.");
            }

            using(FileStream fs = File.OpenRead(file)){
                return ParseFromStream(fs,Encoding.UTF8);
            }
        }

        /// <summary>
        /// Parses MIME message from the specified file.
        /// </summary>
        /// <param name="file">File name with path from where to parse MIME message.</param>
        /// <param name="headerEncoding">Header reading encoding. If not sure UTF-8 is recommended.</param>
        /// <returns>Returns parsed MIME message.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>file</b> or <b>headerEncoding</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public static MIME_Message ParseFromFile(string file,Encoding headerEncoding)
        {
            if(file == null){
                throw new ArgumentNullException("file");
            }
            if(file == ""){
                throw new ArgumentException("Argument 'file' value must be specified.");
            }
            if(headerEncoding == null){
                throw new ArgumentNullException("headerEncoding");
            }

            using(FileStream fs = File.OpenRead(file)){
                return ParseFromStream(fs,headerEncoding);
            }
        }

        #endregion

        #region static method ParseFromStream

        /// <summary>
        /// Parses MIME message from the specified stream.
        /// </summary>
        /// <param name="stream">Stream from where to parse MIME message. Parsing starts from current stream position.</param>
        /// <returns>Returns parsed MIME message.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null.</exception>
        public static MIME_Message ParseFromStream(Stream stream)
        {
            if(stream == null){
                throw new ArgumentNullException("stream");
            }

            return ParseFromStream(stream,Encoding.UTF8);
        }

        /// <summary>
        /// Parses MIME message from the specified stream.
        /// </summary>
        /// <param name="stream">Stream from where to parse MIME message. Parsing starts from current stream position.</param>
        /// <param name="headerEncoding">Header reading encoding. If not sure UTF-8 is recommended.</param>
        /// <returns>Returns parsed MIME message.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> or <b>headerEncoding</b> is null.</exception>
        public static MIME_Message ParseFromStream(Stream stream,Encoding headerEncoding)
        {
            if(stream == null){
                throw new ArgumentNullException("stream");
            }
            if(headerEncoding == null){
                throw new ArgumentNullException("headerEncoding");
            }

            MIME_Message retVal = new MIME_Message();
            retVal.Parse(new SmartStream(stream,false),headerEncoding,new MIME_h_ContentType("text/plain"));

            return retVal;
        }

        #endregion


        #region method ToFile

        /// <summary>
        /// Stores message to the specified file.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when <b>file</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public void ToFile(string file)
        {
            ToFile(file,new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B,Encoding.UTF8),Encoding.UTF8);
        }

        #endregion

        #region method ToStream

        /// <summary>
        /// Store message to the specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store MIME entity. Storing starts form stream current position.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null.</exception>
        public void ToStream(Stream stream)
        {
            ToStream(stream,new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B,Encoding.UTF8),Encoding.UTF8);
        }

        #endregion

        #region method ToString

        /// <summary>
        /// Returns message as string.
        /// </summary>
        /// <returns>Returns message as string.</returns>
        public override string ToString()
        {
            return ToString(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B,Encoding.UTF8),Encoding.UTF8);
        }

        #endregion

        #region method ToByte

        /// <summary>
        /// Returns message as byte[].
        /// </summary>
        /// <returns>Returns message as byte[].</returns>
        public byte[] ToByte()
        {
            return ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B,Encoding.UTF8),Encoding.UTF8);
        }

        #endregion

        #region method GetAllEntities

        /// <summary>
        /// Gets all MIME entities as list.
        /// </summary>
        /// <param name="includeEmbbedMessage">If true, then embbed RFC822 message child entities are included.</param>
        /// <returns>Returns all MIME entities as list.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this class is disposed and this method is accessed.</exception>
        public MIME_Entity[] GetAllEntities(bool includeEmbbedMessage)
        {
            if(m_IsDisposed){
                throw new ObjectDisposedException(this.GetType().Name);
            }

            List<MIME_Entity>  retVal       = new List<MIME_Entity>();
            List<MIME_Entity> entitiesQueue = new List<MIME_Entity>();
            entitiesQueue.Add(this);
            
            while(entitiesQueue.Count > 0){
                MIME_Entity currentEntity = entitiesQueue[0];
                entitiesQueue.RemoveAt(0);
        
                retVal.Add(currentEntity);

                // Current entity is multipart entity, add it's body-parts for processing.
                if(this.Body != null && currentEntity.Body.GetType().IsSubclassOf(typeof(MIME_b_Multipart))){
                    MIME_EntityCollection bodyParts = ((MIME_b_Multipart)currentEntity.Body).BodyParts;
                    for(int i=0;i<bodyParts.Count;i++){
                        entitiesQueue.Insert(i,bodyParts[i]);
                    }
                }
                // Add embbed message for processing (Embbed message entities will be included).
                else if(includeEmbbedMessage && this.Body != null && currentEntity.Body is MIME_b_MessageRfc822){
                    entitiesQueue.Add(((MIME_b_MessageRfc822)currentEntity.Body).Message);
                }
            }

            return retVal.ToArray();
        }

        #endregion

        #region method GetEntityByCID

        /// <summary>
        /// Gets MIME entity with the specified Content-ID. Returns null if no such entity.
        /// </summary>
        /// <param name="cid">Content ID.</param>
        /// <returns>Returns MIME entity with the specified Content-ID or null if no such entity.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this class is disposed and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>cid</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public MIME_Entity GetEntityByCID(string cid)
        {
            if(m_IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }
            if(cid == null){
                throw new ArgumentNullException("cid");
            }
            if(cid == ""){
                throw new ArgumentException("Argument 'cid' value must be specified.","cid");
            }

            foreach(MIME_Entity entity in this.AllEntities){
                if(entity.ContentID == cid){
                    return entity;
                }
            }

            return null;
        }

        #endregion

        // TODO:
        //public MIME_Entity GetEntityByPartsSpecifier(string partsSpecifier)

        #region method ConvertToMultipartSigned

        /// <summary>
        /// Converts message to multipart/signed message.
        /// </summary>
        /// <param name="signerCert">>Signer certificate</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>signerCert</b> is null reference.</exception>
        /// <exception cref="InvalidOperationException">Is raised when this method is called for already signed message.</exception>
        public void ConvertToMultipartSigned(X509Certificate2 signerCert)
        {
            if(signerCert == null){
                throw new ArgumentNullException("signerCert");
            }
            if(this.IsSigned){
                throw new InvalidOperationException("Message is already signed.");
            }

            MIME_Entity msgEntity = new MIME_Entity();
            msgEntity.Body = this.Body;
            msgEntity.ContentDisposition = this.ContentDisposition;
            msgEntity.ContentTransferEncoding = this.ContentTransferEncoding;
            this.ContentTransferEncoding = null;

            MIME_b_MultipartSigned multipartSigned = new MIME_b_MultipartSigned();
            this.Body = multipartSigned;
            multipartSigned.SetCertificate(signerCert);
            multipartSigned.BodyParts.Add(msgEntity);
        }

        #endregion

        #region method VerifySignatures

        /// <summary>
        /// Checks SMIME signed enities signtures. NOTE: For not signed messsages this method always return true.
        /// </summary>
        /// <returns>Returns true if all signatures are valid.</returns>
        /// <exception cref="NotSupportedException">Is raised when entity is signed with not supported encryption.</exception>
        public bool VerifySignatures()
        {
            foreach(MIME_Entity entity in this.AllEntities){
                if(string.Equals(entity.ContentType.TypeWithSubtype,MIME_MediaTypes.Application.pkcs7_mime,StringComparison.InvariantCultureIgnoreCase)){
                    if(!((MIME_b_ApplicationPkcs7Mime)entity.Body).VerifySignature()){
                        return false;
                    }
                }
                else if(string.Equals(entity.ContentType.TypeWithSubtype,MIME_MediaTypes.Multipart.signed,StringComparison.InvariantCultureIgnoreCase)){
                    if(!((MIME_b_MultipartSigned)entity.Body).VerifySignature()){
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion


        #region Properties Implementation

        /// <summary>
        /// Gets if message contains signed data.
        /// </summary>
        public bool IsSigned
        {
            get{
                foreach(MIME_Entity entity in this.AllEntities){
                    if(string.Equals(entity.ContentType.TypeWithSubtype,MIME_MediaTypes.Application.pkcs7_mime,StringComparison.InvariantCultureIgnoreCase)){
                        return true;
                    }
                    else if(string.Equals(entity.ContentType.TypeWithSubtype,MIME_MediaTypes.Multipart.signed,StringComparison.InvariantCultureIgnoreCase)){
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets all MIME entities as list.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is disposed and this property is accessed.</exception>
        /// <remarks>The nestetd entities of embbed messages with <b>Content-Type: Message/Rfc822</b> are also included.</remarks>
        public MIME_Entity[] AllEntities
        {
            get{
                if(m_IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                List<MIME_Entity> retVal        = new List<MIME_Entity>();
                List<MIME_Entity> entitiesQueue = new List<MIME_Entity>();
                entitiesQueue.Add(this);
            
                while(entitiesQueue.Count > 0){
                    MIME_Entity currentEntity = entitiesQueue[0];
                    entitiesQueue.RemoveAt(0);
        
                    retVal.Add(currentEntity);

                    // Current entity is multipart entity, add it's body-parts for processing.
                    if(this.Body != null && currentEntity.Body.GetType().IsSubclassOf(typeof(MIME_b_Multipart))){
                        MIME_EntityCollection bodyParts = ((MIME_b_Multipart)currentEntity.Body).BodyParts;
                        for(int i=0;i<bodyParts.Count;i++){
                            entitiesQueue.Insert(i,bodyParts[i]);
                        }
                    }
                    // Add embbed message for processing (Embbed message entities will be included).
                    else if(this.Body != null && currentEntity.Body is MIME_b_MessageRfc822){
                        entitiesQueue.Add(((MIME_b_MessageRfc822)currentEntity.Body).Message);
                    }
                }

                return retVal.ToArray();
            }
        }

        #endregion


        // --- Obsolete stuff --------

        #region static method CreateAttachment

        /// <summary>
        /// Creates attachment entity.
        /// </summary>
        /// <param name="file">File name with optional path.</param>
        /// <returns>Returns created attachment entity.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>file</b> is null reference.</exception>
        [Obsolete("Use MIME_Entity.CreateEntity_Attachment instead.")]
        public static MIME_Entity CreateAttachment(string file)
        {
            if(file == null){
                throw new ArgumentNullException("file");
            }

            MIME_Entity retVal = new MIME_Entity();
            MIME_b_Application body = new MIME_b_Application(MIME_MediaTypes.Application.octet_stream);
            retVal.Body = body;
            body.SetDataFromFile(file,MIME_TransferEncodings.Base64);
            retVal.ContentType.Param_Name = Path.GetFileName(file);

            FileInfo fileInfo = new FileInfo(file);
            MIME_h_ContentDisposition disposition = new MIME_h_ContentDisposition(MIME_DispositionTypes.Attachment);
            disposition.Param_FileName         = Path.GetFileName(file);
            disposition.Param_Size             = fileInfo.Length;
            disposition.Param_CreationDate     = fileInfo.CreationTime;
            disposition.Param_ModificationDate = fileInfo.LastWriteTime;
            disposition.Param_ReadDate         = fileInfo.LastAccessTime;
            retVal.ContentDisposition = disposition;
            
            return retVal;
        }

        /// <summary>
        /// Creates attachment entity.
        /// </summary>
        /// <param name="stream">Attachment data stream. Data is read from stream current position.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>Returns created attachment entity.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> or <b>fileName</b> is null reference.</exception>        
        [Obsolete("Use MIME_Entity.CreateEntity_Attachment instead.")]
        public static MIME_Entity CreateAttachment(Stream stream,string fileName)
        {
            if(stream == null){
                throw new ArgumentNullException("stream");
            }
            if(fileName == null){
                throw new ArgumentNullException("fileName");
            }

            long fileSize = stream.CanSeek ? (stream.Length - stream.Position) : -1;

            MIME_Entity retVal = new MIME_Entity();
            MIME_b_Application body = new MIME_b_Application(MIME_MediaTypes.Application.octet_stream);
            retVal.Body = body;
            body.SetData(stream,MIME_TransferEncodings.Base64);
            retVal.ContentType.Param_Name = Path.GetFileName(fileName);

            MIME_h_ContentDisposition disposition = new MIME_h_ContentDisposition(MIME_DispositionTypes.Attachment);
            disposition.Param_FileName         = Path.GetFileName(fileName);
            disposition.Param_Size             = fileSize;
            //disposition.Param_CreationDate     = fileInfo.CreationTime;
            //disposition.Param_ModificationDate = fileInfo.LastWriteTime;
            //disposition.Param_ReadDate         = fileInfo.LastAccessTime;
            retVal.ContentDisposition = disposition;

            return retVal;
        }

        #endregion

    }
}
