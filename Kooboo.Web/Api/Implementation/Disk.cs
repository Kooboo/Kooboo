//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;

namespace Kooboo.Web.Api.Implementation
{
    public class DiskApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "Disk";
            }
        }

        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        public DiskSize List(ApiCall call)
        {
            return call.WebSite.SiteDb().GetSize();
        }


        public DiskSize Size(ApiCall call)
        {
            return call.WebSite.SiteDb().GetSize();
        }


        public void CleanLog(string storename, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            List<string> names = new List<string>();

            names.Add(storename);

            sitedb.ClearLog(names.ToArray());
        }

        public storeSize GetSize(string storename, ApiCall call)
        {
            storeSize storesize = new storeSize() { Name = storename };

            var sitedb = call.WebSite.SiteDb();

            var repo = sitedb.GetRepository(storename);
            storesize.ItemCount = repo.Store.Count();

            storesize.Disk = Lib.Helper.IOHelper.GetDirectorySize(repo.Store.ObjectFolder);

            var logs = sitedb.Log.GetByStoreName(storename);

            storesize.LogCount = logs.Count;

            Dictionary<Guid, long> keyblockPosition = new Dictionary<Guid, long>();

            HashSet<long> DeletedBlock = new HashSet<long>();

            foreach (var item in logs)
            {
                long blockposition = 0;

                if (item.EditType == IndexedDB.EditType.Delete)
                {
                    blockposition = item.OldBlockPosition;
                    DeletedBlock.Add(blockposition);

                    storesize.CanClean = true; 
                }
                else
                {
                    blockposition = item.NewBlockPosition;
                    if (item.EditType == IndexedDB.EditType.Update && item.OldBlockPosition >0)
                    {
                        storesize.CanClean = true; 
                    }
                }

                if (keyblockPosition.ContainsKey(item.KeyHash))
                {
                    var currentposition = keyblockPosition[item.KeyHash];
                    if (blockposition > currentposition)
                    {
                        keyblockPosition[item.KeyHash] = blockposition;
                    }
                }
                else
                {
                    keyblockPosition[item.KeyHash] = blockposition;
                }
            }

            foreach (var item in keyblockPosition)
            {
                if (item.Value > 0)
                {
                    if (!DeletedBlock.Contains(item.Value))
                    {
                        var itemsize = repo.Store.getLength(item.Value);
                        storesize.ItemLength += itemsize;
                    }
                }
            }

            return storesize;
        }

    }



    public class storeSize
    {
        public string Name { get; set; }

        internal long ItemLength { get; set; }

        internal long Disk { get; set; }

        public int ItemCount { get; set; }

        public int LogCount { get; set; }

        public bool CanClean
        {
            get;set;
        }

        public string DiskSize
        {
            get
            {
                return Lib.Utilities.CalculateUtility.GetSizeString(this.Disk);
            }
        }

        public string ItemSize
        {
            get
            {
                return Lib.Utilities.CalculateUtility.GetSizeString(this.ItemLength);
            }
        }
    }

}
