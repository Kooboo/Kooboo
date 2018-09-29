//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.TaskQueue.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Repository;

namespace Kooboo.Sites.TaskQueue.TaskExecutor
{
    class DeleteDiskExecutor : ITaskExecutor<DeleteDisk>
    {
        public bool Execute(SiteDb SiteDb, string JsonModel)
        {
            var item = Lib.Helper.JsonHelper.Deserialize<DeleteDisk>(JsonModel);

            if (item == null)
            { return true;  }
 
            if (item.IsFolder)
            {
                if (System.IO.Directory.Exists(item.FullPath))
                {
                    try
                    {
                        System.IO.Directory.Delete(item.FullPath, true);
                        return true; 
                    }
                    catch (Exception)
                    {
                        return false; 
                    }
                }
                else
                {
                    return true; 
                }
               
            }
            else
            {
                if (System.IO.File.Exists(item.FullPath))
                {
                    try
                    {
                        System.IO.File.Delete(item.FullPath);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    } 
                }
                else
                {
                    return true; 
                } 
            }
            
        }
    }
}
