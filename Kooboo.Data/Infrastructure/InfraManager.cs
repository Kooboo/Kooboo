//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Data.Infrastructure
{
    public static class InfraManager
    {
        private static IInfrastructure _instance;

        private static object _locker = new object();

        public static IInfrastructure Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new LocalInfra();
                        }
                    }
                }
                return _instance;
            }
            set => _instance = value;
        }

        public static bool Test(Guid organizationId, InfraType infraType, long amount)
        {
            return Instance.Test(organizationId, infraType, amount);
        }

        public static void Add(Guid organizationId, InfraType infraType, long amount, string message)
        {
            Instance.Add(organizationId, infraType, amount, message);
        }
    }
}