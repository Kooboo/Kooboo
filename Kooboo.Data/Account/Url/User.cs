using Kooboo.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Account.Url
{
   public static  class User
    {

        //private string ValidateUrl = AccountUrlHelper.User("validate");
        //private string ProfileUrl = AccountUrlHelper.User("profile");
        //private string GetUserUrl = AccountUrlHelper.User("get");

        //private string GetUserByEmailUrl = AccountUrlHelper.User("GetByEmail");

        //private string ChangePasswordUrl = AccountUrlHelper.User("changepassword");
        //private string AddOrUpdateUserUrl = AccountUrlHelper.User("post");
        //private string IsAdminUrl = AccountUrlHelper.User("isadmin");
        //private string ChangeOrgUrl = AccountUrlHelper.User("ChangeOrg");
        //private string OrganizationsUrl = AccountUrlHelper.User("Organizations");
        //private string GetByTokenUrl = AccountUrlHelper.User("GetByToken");
        //private string RegisterUrl = AccountUrlHelper.User("Register");
          
        private static string _validateurl; 
        public static string ValidateUrl
        {
            get
            {
                if (_validateurl == null)
                {
                    _validateurl = AccountUrlHelper.User("validate"); 
                }
                return _validateurl; 
            }
        }

        private static string _profile;
        public static string ProfileUrl
        {
            get
            {
                if (_profile == null)
                {
                    _profile = AccountUrlHelper.User("profile");
                }
                return _profile;
            }
        }

        private static string _getuser; 
        public static string GetUserUrl
        {
            get
            {
                if (_getuser == null)
                {
                    _getuser = AccountUrlHelper.User("get");
                }
                return _getuser;
            }
        }

        private static string _getbyemail;
        public static string GetUserByEmailUrl
        {
            get
            {
                if (_getbyemail == null)
                {
                    _getbyemail = AccountUrlHelper.User("GetByEmail");
                }
                return _getbyemail;
            }
        }
         
        //private string ChangePasswordUrl = AccountUrlHelper.User("changepassword");
        //private string AddOrUpdateUserUrl = AccountUrlHelper.User("post");
        //private string IsAdminUrl = AccountUrlHelper.User("isadmin");
        //private string ChangeOrgUrl = AccountUrlHelper.User("ChangeOrg");
        //private string OrganizationsUrl = AccountUrlHelper.User("Organizations");
        //private string GetByTokenUrl = AccountUrlHelper.User("GetByToken");
        //private string RegisterUrl = AccountUrlHelper.User("Register");


    }
}
