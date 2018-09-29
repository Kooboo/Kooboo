//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Events.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Upgrade
{
    public class UpgradeHandler : Data.Events.IHandler<ApplicationStartUp>
    {
        public void Handle(ApplicationStartUp theEvent, RenderContext context)
        {
            //var version = UpgradeManager.GetVersion();
            //var upgraders = UpgradeManager.GetAppUpgraderList(version);
            //foreach (var item in upgraders.OrderBy(o => o.LowerVersion))
            //{
            //    try
            //    {
            //        item.Do();
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}

            //var siteupgraders = UpgradeManager.GetSiteUpgraderList(version);

            //if (siteupgraders != null && siteupgraders.Count() > 0)
            //{
            //    var allsites = Kooboo.Data.GlobalDb.WebSites.All();

            //    foreach (var item in allsites)
            //    {
            //        foreach (var upgrade in siteupgraders)
            //        {
            //            try
            //            {
            //                upgrade.Do(item);
            //            }
            //            catch (Exception)
            //            {
            //            }
            //        }
            //    }
            //}

           // UpgradeManager.SetVersion();
        }
    }



}
