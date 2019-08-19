//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.App.Upgrade
{
    class Program
    {
        //current only support linux  app upgrade
        static void Main(string[] args)
        {
            KoobooUpgrade.Log("Closing Kooboo");
            KoobooUpgrade.CloseKooboo();
            KoobooUpgrade.Log("Kooboo Closed");

            KoobooUpgrade.Log("Upgrade in progress");
            KoobooUpgrade.Upgrade();
            KoobooUpgrade.Log("Upgrade finished");

            KoobooUpgrade.Log("Opening Kooboo");
            KoobooUpgrade.OpenKoobooApp();
        }
    }
}
