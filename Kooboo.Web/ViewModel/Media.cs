//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Sites.Storage;
using Newtonsoft.Json;

namespace Kooboo.Web.ViewModel
{
    public class MediaLibraryViewModel
    {
        public List<ImageFolderViewModel> Folders { get; set; }

        public List<MediaStorageFileModel> Files { get; set; }

        public List<CrumbPath> CrumbPath { get; set; }
    }

    public class MediaPagedViewModel : PagedViewModel<MediaStorageFileModel, StorageFolderModel>
    {
        public string ErrorMessage { get; set; }
    }

    public class ImageFolderViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string FullPath { get; set; }

        public int Count { get; set; }
        public string NavigateUrl { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class MediaFileViewModel
    {
        /// <summary>
        /// The image Id, can be used to retrieve the image and edit it. 
        /// </summary> 
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Thumbnail { get; set; }

        public string Url { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public long Size { get; set; }

        public bool IsImage { get; set; }

        public string Type { get; set; }

        public Dictionary<string, int> References { get; set; }

        public DateTime LastModified { get; set; }

        public string Alt { get; set; }

        public string PreviewUrl { get; set; }

        [JsonIgnore]
        public string MimeTypeOverride { get; set; }


        public string MimeType
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(MimeTypeOverride))
                {
                    return MimeTypeOverride;
                }

                return IOHelper.MimeType(this.Url);
            }
        }
    }

    public class ImageDeleteViewModel
    {
        public List<string> Folders { get; set; } = new List<string>();

        public List<Guid> Images { get; set; } = new List<Guid>();
    }

    public class ImageViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string SiteUrl { get; set; }

        public string Alt { get; set; }

        public string FullUrl { get; set; }
    }

    public class OnlineProvider
    {
        public string Value { get; set; }

        public string Label { get; set; }

        public string Url { get; set; }
    }

    public class ImageDownloadViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Alt { get; set; }
    }
}
