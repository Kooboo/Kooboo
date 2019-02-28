//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.SiteTransfer.Download
{
  public  interface IDownloadExecutor
    {
        DownloadTask DownloadTask { get; set;  }

        DownloadManager Manager { get; set; }
         
        Task  Execute(); 
    }
}
