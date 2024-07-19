//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.

namespace Kooboo.Web.ViewModel
{
    public class BulkDownloadViewModel
    {
        public string Root { get; set; }

        public List<Guid> Files { get; set; }

        public List<string> Folders { get; set; }
    }
}
