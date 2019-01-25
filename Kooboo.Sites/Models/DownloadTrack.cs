//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    /// <summary>
    /// used to temp track the downloads... 
    /// </summary>
   public class DownloadFailTrack : Kooboo.Sites.Models.SiteObject
    {
       public DownloadFailTrack()
       {
           this.HistoryTime = new List<DateTime>(); 
       }

       public override Guid Id { get; set; }
       
       public List<DateTime> HistoryTime { get; set; } 
    }
}
