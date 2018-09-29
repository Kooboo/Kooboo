//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.FrontEvent;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
  
namespace Kooboo.Sites.Events.FrontFlow
{
  public static class FrontEventHelper
    {
        public static List<EventConditionSetting> GetPageSetting(SiteDb siteDb)
        {
            List<EventConditionSetting> result = new List<EventConditionSetting>(); 

            result.Add(new EventConditionSetting() { Name = "Page.Name" });
            result.Add(new EventConditionSetting() { Name = "Page.DefaultStart", ControlType = Data.ControlType.CheckBox, DataType = typeof(bool) });
            result.Add(new EventConditionSetting() { Name = "Page.Body" });

            Dictionary<string, string> pagenames = new Dictionary<string, string>();
            foreach (var item in siteDb.Pages.All())
            {
                var url = Sites.Service.ObjectService.GetObjectRelativeUrl(siteDb, item);
                pagenames.Add(item.Id.ToString(), url);
            }

            result.Add(new EventConditionSetting() { Name = "Page.Id", ControlType = Data.ControlType.Selection, DataType = typeof(Guid), SelectionValues = pagenames });

            return result; 
        }

        public static List<EventConditionSetting> GetViewSetting(SiteDb siteDb)
        {
            List<EventConditionSetting> result = new List<EventConditionSetting>();

            Dictionary<string, string> viewnames = new Dictionary<string, string>();
            foreach (var item in siteDb.Views.All())
            {
                viewnames.Add(item.Id.ToString(), item.Name);
            }

            result.Add(new EventConditionSetting() { Name = "View.Id", ControlType = Data.ControlType.Selection, DataType = typeof(Guid), SelectionValues = viewnames });

            result.Add(new EventConditionSetting() { Name = "View.Name" });

            return result;
        }
    }
}
