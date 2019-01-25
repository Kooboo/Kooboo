//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.ViewModel
{
  public  class DiskSize
    {
        public long Total{ get; set; } 

        public string TotalSize { get { return Lib.Utilities.CalculateUtility.GetSizeString(this.Total);  } }

        public List<RepoSize> RepositorySize { get; set; } = new List<RepoSize>(); 

        public long VisitorLog { get; set; }

        public List<string> VisitorLogWeeks { get; set; } = new List<string>(); 
           
        public long RepositoryEditCount { get; set; } 
         
    }

    public class RepoSize
    {
        public string Name { get; set; }

        public long Length { get; set; }

        public int ItemCount { get; set; }

        public string Size
        {
            get
            {
                return Lib.Utilities.CalculateUtility.GetSizeString(this.Length); 
            }
        }
    }
}
