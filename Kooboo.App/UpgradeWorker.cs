//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.App
{
    class UpgradeWorker
    {
        public class UpgradeWorkder : IBackgroundWorker
        {
            public int Interval
            {
                get
                {
                    return  300 * 60;
                }
            }

            public DateTime LastExecute
            {
                get; set;
            }
             
            public async void Execute()
            {
                if (KoobooUpgrade.IsAutoUpgrade)
                {
                    var hours = DateTime.Now.Hour; 
                     // only do it in the midnight of current computer zone. 
                    if (hours == 2 || hours == 3 || hours == 4)
                    {
                       await KoobooUpgrade.Upgrade();
                    }  
                }
            }
        }
    }
}

