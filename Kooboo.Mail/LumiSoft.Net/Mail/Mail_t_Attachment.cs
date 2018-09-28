using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;

using LumiSoft.Net.IO;

namespace LumiSoft.Net.Mail
{
    /// <summary>
    /// This class represents mail message attachment info.
    /// </summary>
    public class Mail_t_Attachment
    {
        private string m_Name        = null;
        private string m_FileName    = null;
        private bool   m_ZipCompress = false;
        private bool   m_CloseStream = true;
        private Stream m_pStream     = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="fileName">File name with path.</param>
        /// <exception cref="ArgumentNullException">>Is raised when <b>fileName</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public Mail_t_Attachment(string fileName) : this(null,fileName)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="fileName">File name with path.</param>
        /// <param name="zipCompress">If true attachment is zip compressed.</param>
        /// <exception cref="ArgumentNullException">>Is raised when <b>fileName</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public Mail_t_Attachment(string fileName,bool zipCompress) : this(null,fileName,zipCompress)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="attachmentName">Attachment file name what appears in mail message. If null then 'fileName' filename without path is used.</param>
        /// <param name="fileName">File name with path.</param>
        /// <exception cref="ArgumentNullException">>Is raised when <b>fileName</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public Mail_t_Attachment(string attachmentName,string fileName) : this(null,fileName,false)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="attachmentName">Attachment file name what appears in mail message. If null then 'fileName' filename without path is used.</param>
        /// <param name="fileName">File name with path.</param>
        /// <param name="zipCompress">If true attachment is zip compressed.</param>
        /// <exception cref="ArgumentNullException">>Is raised when <b>fileName</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public Mail_t_Attachment(string attachmentName,string fileName,bool zipCompress)
        {
            if(fileName == null){
                throw new ArgumentNullException("fileName");
            }
            if(string.IsNullOrEmpty(fileName)){
                throw new ArgumentException("Argument 'fileName' value must be specified.","fileName");
            }

            if(string.IsNullOrEmpty(attachmentName)){
                m_Name = Path.GetFileName(fileName);
            }
            else{
                m_Name = attachmentName;
            }

            m_FileName    = fileName;
            m_ZipCompress = zipCompress;
            m_CloseStream = true;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="attachmentName">Attachment file name what appears in mail message.</param>
        /// <param name="data">Attachment data.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>attachmentName</b> or <b>data</b> is null reference.</exception>
        public Mail_t_Attachment(string attachmentName,byte[] data) : this(attachmentName,data,false)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="attachmentName">Attachment file name what appears in mail message.</param>
        /// <param name="data">Attachment data.</param>
        /// <param name="zipCompress">If true attachment is zip compressed.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>attachmentName</b> or <b>data</b> is null reference.</exception>
        public Mail_t_Attachment(string attachmentName,byte[] data,bool zipCompress)
        {
            if(attachmentName == null){
                throw new ArgumentNullException("attachmentName");
            }
            if(data == null){
                throw new ArgumentNullException("data");
            }

            m_Name    = attachmentName;
            m_pStream = new MemoryStream(data);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="attachmentName">Attachment file name what appears in mail message.</param>
        /// <param name="stream">Stream containing attachment data.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>attachmentName</b> or <b>stream</b> is null reference.</exception>
        public Mail_t_Attachment(string attachmentName,Stream stream) : this(attachmentName,stream,false)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="attachmentName">Attachment file name what appears in mail message.</param>
        /// <param name="stream">Stream containing attachment data.</param>
        /// <param name="zipCompress">If true attachment is zip compressed.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>attachmentName</b> or <b>stream</b> is null reference.</exception>
        public Mail_t_Attachment(string attachmentName,Stream stream,bool zipCompress)
        {
            if(attachmentName == null){
                throw new ArgumentNullException("attachmentName");
            }
            if(stream == null){
                throw new ArgumentNullException("stream");
            }

            m_Name    = attachmentName;
            m_pStream = stream;
        }


        #region method GetStream

        /// <summary>
        /// Gets attachment stream.
        /// </summary>
        /// <returns>Returns attachment stream.</returns>
        internal Stream GetStream()
        {
            if(m_pStream == null){
                m_pStream = File.OpenRead(m_FileName);
            }

            if(m_ZipCompress){
                #if NET20 || NET35|| NET40
                    throw new InvalidOperationException("Not supported lower framework version than 4.5.");
                #else
                    MemoryStreamEx retVal = new MemoryStreamEx();

                    using(ZipArchive archive = new ZipArchive(retVal,ZipArchiveMode.Create)){
                        ZipArchiveEntry entry = archive.CreateEntry(m_Name,CompressionLevel.Optimal);
                        using(Stream zipStream = entry.Open()){
                            Net_Utils.StreamCopy(m_pStream,zipStream,64000);
                        }
                    }
                    retVal.Position = 0;
                    CloseStream();

                    return retVal;
                #endif
            }

            return m_pStream;
        }

        #endregion

        #region method CloseStream

        /// <summary>
        /// Closes stream.
        /// </summary>
        internal void CloseStream()
        {
            if(m_CloseStream){
                m_pStream.Dispose();
            }
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets attachment name.
        /// </summary>
        public string Name
        {
            get{
                if(m_ZipCompress){
                    return Path.GetFileNameWithoutExtension(m_Name) + ".zip";
                }
                else{
                    return m_Name;
                }
            }
        }

        #endregion
    }
}
