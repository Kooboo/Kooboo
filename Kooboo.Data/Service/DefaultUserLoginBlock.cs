using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Service
{
    public class DefaultUserLoginBlock : ILoginBlock
    {
        public int MaxFailure { get; set; } = 5;

        public Dictionary<string, List<DateTime>> IPFailure { get; set; } = new Dictionary<string, List<DateTime>>();

        public Dictionary<Guid, List<DateTime>> UserNameFailure { get; set; } = new Dictionary<Guid, List<DateTime>>();

        public virtual void AddIpFailure(string ip)
        {
            if (!IPFailure.ContainsKey(ip))
            {
                var list = new List<DateTime> { DateTime.Now };

                IPFailure[ip] = list;
            }
            else
            {
                var list = IPFailure[ip];
                list.Add(DateTime.Now);
            }
        }

        public void AddLoginOk(string username, string ip)
        {
            if (ip != null)
            {
                if (IPFailure.ContainsKey(ip))
                {
                    IPFailure.Remove(ip);
                }
            }

            if (username != null)
            {
                var userid = Lib.Security.Hash.ComputeGuidIgnoreCase(username);
                UserNameFailure.Remove(userid);
            }
        }

        public void AddUserNameFailure(string userName)
        {
            if (userName == null)
            {
                return;
            }
            var userid = Lib.Security.Hash.ComputeGuidIgnoreCase(userName);

            if (!UserNameFailure.ContainsKey(userid))
            {
                var list = new List<DateTime> { DateTime.Now };

                UserNameFailure[userid] = list;
            }
            else
            {
                var list = UserNameFailure[userid];
                list.Add(DateTime.Now);
            }
        }

        public virtual bool IsIpBlocked(string ip)
        {
            if (IPFailure.ContainsKey(ip))
            {
                var items = IPFailure[ip];

                items.RemoveAll(o => o < DateTime.Now.AddHours(-4));

                if (items.Count > MaxFailure)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsUserNameBlocked(string userName)
        {
            if (userName == null)
            {
                return false;
            }
            var userid = Lib.Security.Hash.ComputeGuidIgnoreCase(userName);

            if (UserNameFailure.ContainsKey(userid))
            {
                var items = UserNameFailure[userid];

                items.RemoveAll(o => o < DateTime.Now.AddHours(-4));

                if (items.Count > MaxFailure)
                {
                    return true;
                }
            }
            return false;
        }
    }
}