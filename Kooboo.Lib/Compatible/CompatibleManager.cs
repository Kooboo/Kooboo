//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;

namespace Kooboo.Lib.Compatible
{

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
                if (instance == null)
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
            Framework = new NetStandard();
            if (RuntimeSystemHelper.IsWindow())
            {
                System = new WindowSystem();
            }
            else
            {
                System = new LinuxSystem();
            }
        }
    }
}
