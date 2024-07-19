//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Storage;

namespace Kooboo.Web.ViewModel
{
    public class FileOverViewModel : PagedViewModel<StorageFileModel, StorageFolderModel>
    {
        public string ErrorMessage { get; set; }
    }

    public class FileFolderViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string FullPath { get; set; }

        public int Count { get; set; }
        public string NavigateUrl { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class FileDeleteViewModel
    {
        public List<string> Folders { get; set; } = new List<string>();

        public List<Guid> Images { get; set; } = new List<Guid>();
    }

}
