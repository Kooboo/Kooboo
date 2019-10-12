//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.App.Models
{
    public static class SystemStatus
    {
        public static int Port { get; set; }

        public static string StartUrl
        {
            get
            {
                var uri = new UriBuilder("http", Kooboo.Data.AppSettings.StartHost, Port, "/_Admin/account/login");
                return uri.Uri?.AbsoluteUri;
            }
        }
    }
}