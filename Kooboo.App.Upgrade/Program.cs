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
            KoobooUpgrade.Log("start close kooboo");
            KoobooUpgrade.CloseKooboo();
            KoobooUpgrade.Log("close kooboo");

            KoobooUpgrade.Log("start upgrade");
            KoobooUpgrade.Upgrade();
            KoobooUpgrade.Log("end upgrade");

            KoobooUpgrade.Log("open kooboo");
            KoobooUpgrade.OpenKoobooApp();

        }
    }
}
