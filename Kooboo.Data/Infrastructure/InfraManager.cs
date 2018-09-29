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


        public static bool Test(Guid OrganizationId, byte InfraType, int amount)
        {
            return instance.Test(OrganizationId, InfraType, amount); 
        }

        public static void Add(Guid OrganizationId, byte InfraType, int amount, string message)
        {
            instance.Add(OrganizationId, InfraType, amount, message); 
        }
                  
    }
}
