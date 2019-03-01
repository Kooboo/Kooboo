//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core {
    using System.Collections.Generic;

    public class ContentResult {
        public string Content { get; set; }
        public string MimeType { get; set; }
        public IEnumerable<string> CacheInvalidationFileList { get; set; }
    }
}
