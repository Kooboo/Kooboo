//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Helper;

namespace Kooboo.Lib.Compatible
{
    //    public static class CompatibleManager
    //    {
    //        public static ISystem System;

    //        public static IFramework Framework;

    //        static CompatibleManager()
    //        {
    //#if NET45
    //            System=new WindowSystem();
    //            Framework=new NET45();
    //#else
    //            Framework = new NetStandard();
    //            if (RuntimeSystemHelper.IsWindow())
    //            {
    //                System = new WindowSystem();
    //            }
    //            else
    //            {
    //                System = new LinuxSystem();
    //            }
    //#endif
    //        }
    //    }

    //for best test
    public class CompatibleManager
    {
        public ISystem System;

        public IFramework Framework;

        private static object _lockObj = new object();

        private static CompatibleManager _instance;

        public static CompatibleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new CompatibleManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public CompatibleManager()
        {
#if NET45|| NET461
            System = new WindowSystem();
            Framework = new NET45();
#else
            Framework = new NetStandard();
            if (RuntimeSystemHelper.IsWindow())
            {
                System = new WindowSystem();
            }
            else
            {
                System = new LinuxSystem();
            }
#endif
        }
    }
}