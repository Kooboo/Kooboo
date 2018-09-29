using Kooboo.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Account.Url
{
   public static class Org
    {
        private static string _geturl; 
        public static string GetUrl { get {  if (_geturl == null) { _geturl  = AccountUrlHelper.Org("get"); };  return _geturl;  } }

        private static string _addurl;

        public static string Add  {
            get 
            { 
               if (_addurl == null)
                {
                    _addurl = AccountUrlHelper.Org("add");
                }
                return _addurl; 
            }
        }

        private static string _updateurl;
        public static string Update
        {
            get
            {
                if (_updateurl == null)
                {
                    _updateurl = AccountUrlHelper.Org("update");
                }
                return _updateurl;
            }
        }

        private static string _delurl; 
        public static string Delete
        {
            get
            {
                if (_delurl == null)
                {
                    _delurl = AccountUrlHelper.Org("delete");
                }
                return _delurl;
            }
        }

        private static string _users; 
        public static string Users
        {
            get
            {
                if (_users == null)
                {
                    _users = AccountUrlHelper.Org("Users");
                }
                return _users;
            }
        }

        private static string _Addurl;
        public static string AddUser 
        {
            get
            {
                if (_Addurl == null)
                {
                    _Addurl = AccountUrlHelper.Org("adduser");
                }
                return _Addurl;
            }
        }

        private static string _deleteuser; 
        public static string DeleteUser
        {
            get
            {
                if (_deleteuser == null)
                {
                    _deleteuser = AccountUrlHelper.Org("deleteuser");
                }
                return _deleteuser;
            }
        } 

    }
}
