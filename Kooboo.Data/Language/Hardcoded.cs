//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System.Collections.Generic; 

namespace Kooboo.Data.Language
{
  public static class Hardcoded
    {
        private static object _locker = new object(); 

        private static HashSet<string> _langkeys; 

        // make sure all dynamic lang values are defined here.. otherwise, it might not be translated. 
        // this is only for keys that are defined during runtime. 
        public static HashSet<string> HardCodeLangKeys
        {
            get
            {
                if (_langkeys == null)
                {
                   lock(_locker)
                    {
                        _langkeys = new HashSet<string>();
                        // Data center names... 
                       // _langkeys.Add("HK(China)");
                         
                        // kooboo app.  
                        //_langkeys.Add("Confirm");
                        //_langkeys.Add("is in use");
                        //_langkeys.Add("Kooboo at port");
                        //_langkeys.Add("View in browser");
                        //_langkeys.Add("Invalid website configuration");
                        //_langkeys.Add("Open containing folder");
                        //_langkeys.Add("Website path not found");
                        //_langkeys.Add("Delete server");
                        //_langkeys.Add("Upgrade");
                        //_langkeys.Add("Upgrade in progress, please stand by");
                        //_langkeys.Add("Software up to date");
                        //_langkeys.Add("New version ready to be updated"); 
                        /// menu items.  
                        //_langkeys.Add("Domains");
                        //_langkeys.Add("Sites");
                        //_langkeys.Add("Emails");
                        //_langkeys.Add("Extensions");
                        //_langkeys.Add("Feature");
                        //_langkeys.Add("Advance");
                        //_langkeys.Add("Inbox");
                        //_langkeys.Add("Sent");
                        //_langkeys.Add("Draft");
                        //_langkeys.Add("Trash");
                        //_langkeys.Add("Spam");
                        //_langkeys.Add("Address");
                        //_langkeys.Add("SiteBindings");
                        //_langkeys.Add("Assembly");
                        //_langkeys.Add("DataSource");
                        //_langkeys.Add("Media Library");
                        //_langkeys.Add("Pages");
                        //_langkeys.Add("Diagnosis");
                        //_langkeys.Add("Publishing");
                        //_langkeys.Add("System");
                        //_langkeys.Add("Settings");
                        //_langkeys.Add("Domains");
                        //_langkeys.Add("SiteLogs");
                        //_langkeys.Add("VisitorLogs");
                    }
                } 
                return _langkeys; 
            }
        }

        public static string GetValue(string key, RenderContext context)
        {
            return LanguageProvider.GetValue(key, context); 
        }

        public static string GetValue(string key)
        {
            return LanguageProvider.GetValue(key); 
        }  
    }
}
