//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Download
{
    public interface IDownloadExecutor
    {
        DownloadTask DownloadTask { get; set; }

        DownloadManager Manager { get; set; }

        Task Execute();
    }
}