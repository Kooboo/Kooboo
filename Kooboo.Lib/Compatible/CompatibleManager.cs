//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Lib.Helper;
using Kooboo.Lib.Compatible;

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

        private static CompatibleManager instance;

        public static CompatibleManager Instance
        {
            get
            {
                if(instance==null)
                {
                    lock (_lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new CompatibleManager();
                        }
                        
                    }
                    
                }
                return instance;
            }
        }
        public CompatibleManager()
        {
#if NET461
            System = new WindowSystem();
            Framework = new NET461();
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
