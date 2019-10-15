//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Helper;

namespace Kooboo.Data.Account.Url
{
    public static class User
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

        public static string ValidateUrl => _validateurl ?? (_validateurl = AccountUrlHelper.User("validate"));

        private static string _profile;

        public static string ProfileUrl => _profile ?? (_profile = AccountUrlHelper.User("profile"));

        private static string _getuser;

        public static string GetUserUrl => _getuser ?? (_getuser = AccountUrlHelper.User("get"));

        private static string _getbyemail;

        public static string GetUserByEmailUrl => _getbyemail ?? (_getbyemail = AccountUrlHelper.User("GetByEmail"));

        //private string ChangePasswordUrl = AccountUrlHelper.User("changepassword");
        //private string AddOrUpdateUserUrl = AccountUrlHelper.User("post");
        //private string IsAdminUrl = AccountUrlHelper.User("isadmin");
        //private string ChangeOrgUrl = AccountUrlHelper.User("ChangeOrg");
        //private string OrganizationsUrl = AccountUrlHelper.User("Organizations");
        //private string GetByTokenUrl = AccountUrlHelper.User("GetByToken");
        //private string RegisterUrl = AccountUrlHelper.User("Register");
    }
}