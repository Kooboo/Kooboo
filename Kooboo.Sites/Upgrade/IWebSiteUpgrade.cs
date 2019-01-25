//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Upgrade
{
    public interface IWebSiteUpgrade
    {
        System.Version UpVersion { get; }

        System.Version LowerVersion { get; }

        void Do(WebSite site);
    }
}
