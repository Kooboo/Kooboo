﻿using System;

using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
    /// <summary>
    /// This class represents MIME multipart/related body. Defined in RFC 2387.
    /// </summary>
    /// <remarks>
    ///  The Multipart/Related content-type provides a common mechanism for
    ///  representing objects that are aggregates of related MIME body parts.
    /// </remarks>
    public class MIME_b_MultipartRelated : MIME_b_Multipart
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="contentType">Content type.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>contentType</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public MIME_b_MultipartRelated(MIME_h_ContentType contentType) : base(contentType)
        {
            if (!string.Equals(contentType.TypeWithSubtype, "multipart/related", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/related'.");
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
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b>, <b>defaultContentType</b> or <b>stream</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when any parsing errors.</exception>
        protected static new MIME_b Parse(MIME_Entity owner, MIME_h_ContentType defaultContentType, SmartStream stream)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            if (defaultContentType == null)
            {
                throw new ArgumentNullException("defaultContentType");
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (owner.ContentType == null || owner.ContentType.Param_Boundary == null)
            {
                throw new ParseException("Multipart entity has not required 'boundary' paramter.");
            }

            MIME_b_MultipartRelated retVal = new MIME_b_MultipartRelated(owner.ContentType);
            ParseInternal(owner, owner.ContentType.TypeWithSubtype, stream, retVal);

            return retVal;
        }

        #endregion


        #region Properties implementation

        #endregion
    }
}
