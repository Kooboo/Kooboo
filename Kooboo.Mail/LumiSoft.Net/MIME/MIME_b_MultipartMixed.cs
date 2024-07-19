﻿using System;

using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
    /// <summary>
    /// This class represents MIME multipart/mixed body. Defined in RFC 2046 5.1.3.
    /// </summary>
    /// <remarks>
    /// The "mixed" subtype of "multipart" is intended for use when the body
    /// parts are independent and need to be bundled in a particular order.
    /// </remarks>
    public class MIME_b_MultipartMixed : MIME_b_Multipart
    {
        /// <summary>
        /// Default constructor. The boundary = auto-generated.
        /// </summary>
        public MIME_b_MultipartMixed() : base()
        {
            MIME_h_ContentType contentType_multipartMixed = new MIME_h_ContentType(MIME_MediaTypes.Multipart.mixed);
            contentType_multipartMixed.Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.');
            this.ContentType = contentType_multipartMixed;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="contentType">Content type.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>contentType</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public MIME_b_MultipartMixed(MIME_h_ContentType contentType) : base(contentType)
        {
            if (!string.Equals(contentType.TypeWithSubtype, "multipart/mixed", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/mixed'.");
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

            MIME_b_MultipartMixed retVal = new MIME_b_MultipartMixed(owner.ContentType);
            ParseInternal(owner, owner.ContentType.TypeWithSubtype, stream, retVal);

            return retVal;
        }

        #endregion


        #region Properties implementation

        #endregion
    }
}
