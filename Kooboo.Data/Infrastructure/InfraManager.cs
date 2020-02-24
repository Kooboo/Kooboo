//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Infrastructure
{
    public static class InfraManager
    {

        private static IInfrastructure _instance;

        private static object _locker = new object();

        public static IInfrastructure instance
        { 
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    { 
                        if (_instance == null)
                        {
                            _instance = Lib.IOC.Service.GetSingleTon<IInfrastructure>(true);
                        }

                        if (_instance == null)
                        {
                            _instance = new LocalInfra(); 
                        }
                    }
                }
                return _instance; 
            }
            set
            {
                _instance = value;
            } 
        }
                                                
        public static bool Test(Guid OrganizationId, InfraType InfraType, long amount)
        {
            return instance.Test(OrganizationId, InfraType, amount); 
        }

        public static void Add(Guid OrganizationId, InfraType InfraType, long amount, string message)
        {
            instance.Add(OrganizationId, InfraType, amount, message); 
        }
                  
    }
}
