//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Helper;

namespace Kooboo.Data.Account.Url
{
    public static class Org
    {
        private static string _geturl;
        public static string GetUrl { get { if (_geturl == null) { _geturl = AccountUrlHelper.Org("get"); }; return _geturl; } }

        private static string _addurl;

        public static string Add => _addurl ?? (_addurl = AccountUrlHelper.Org("add"));

        private static string _updateurl;

        public static string Update => _updateurl ?? (_updateurl = AccountUrlHelper.Org("update"));

        private static string _delurl;

        public static string Delete => _delurl ?? (_delurl = AccountUrlHelper.Org("delete"));

        private static string _users;

        public static string Users => _users ?? (_users = AccountUrlHelper.Org("Users"));

        private static string _Addurl;

        public static string AddUser => _Addurl ?? (_Addurl = AccountUrlHelper.Org("adduser"));

        private static string _deleteuser;

        public static string DeleteUser => _deleteuser ?? (_deleteuser = AccountUrlHelper.Org("deleteuser"));

        private static string _changedemandUserBalance;

        public static string ChangeDemandUserBalance =>
            _changedemandUserBalance ??
            (_changedemandUserBalance = AccountUrlHelper.Org("ChangedemandUserBalance"));

        private static string _addProposalUserBalance;

        public static string AddProposalUserBalance =>
            _addProposalUserBalance ??
            (_addProposalUserBalance = AccountUrlHelper.Org("AddProposalUserBalance"));
    }
}