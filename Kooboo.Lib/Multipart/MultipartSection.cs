//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace Microsoft.AspNet.WebUtilities
{
    public class MultipartSection
    {
        public string ContentType
        {
            get
            {
                string values;
                if (Headers.TryGetValue("Content-Type", out values))
                {
                    return values;
                }
                return null;
            }
        }

        public string ContentDisposition
        {
            get
            {
                string values;
                if (Headers.TryGetValue("Content-Disposition", out values))
                {
                    return values;
                }
                return null;
            }
        }

        public IDictionary<string, string> Headers { get; set; }

        public Stream Body { get; set; }

        /// <summary>
        /// The position where the body starts in the total multipart body.
        /// This may not be available if the total multipart body is not seekable.
        /// </summary>
        public long? BaseStreamOffset { get; set; }
    }
}