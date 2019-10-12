//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.App.Upgrade
{
    internal class Program
    {
        //current only support linux  app upgrade
        private static void Main(string[] args)
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