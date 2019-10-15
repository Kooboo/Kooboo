//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Helper;

namespace Kooboo.Data.Account.Url
{
    public static class Cluster
    {
        private static string _getavailable;

        public static string GetDataCenter { get { if (_getavailable == null) { _getavailable = AccountUrlHelper.Cluster("GetDataCenter"); }; return _getavailable; } }

        private static string _savesetting;

        public static string SaveSetting
        {
            get { if (_savesetting == null) { _savesetting = AccountUrlHelper.Cluster("SaveSetting"); }; return _savesetting; }
        }
    }
}