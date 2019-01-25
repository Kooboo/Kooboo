//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.FrontEvent;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class BusinessRuleApi : SiteObjectApi<BusinessRule>
    {

        public virtual Dictionary<Guid, string> GetAvailableCodes(string eventname, ApiCall call)
        {
            Enum.TryParse(eventname, out Kooboo.Sites.FrontEvent.enumEventType enumeventyptye);

            var sitedb = call.WebSite.SiteDb();

            var allcode = sitedb.Code.GetByEvent(enumeventyptye);

            Dictionary<Guid, string> result = new Dictionary<Guid, string>();

            foreach (var item in allcode)
            {
                result.Add(item.Id, item.Name);
            }
            return result;
        }

        public List<Data.Models.SimpleSetting> GetSetting(Guid id, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            var code = sitedb.Code.Get(id);

            if (code == null)
            {
                return null;
            }
            return Kooboo.Sites.Scripting.Manager.GetSetting(call.Context.WebSite, code);
        }

        public virtual void Post(string eventName, List<IFElseRule> rules, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            string righteventname = getname<Sites.FrontEvent.enumEventType>(eventName);

            Enum.TryParse(righteventname, out Sites.FrontEvent.enumEventType eventtype);

            foreach (var item in rules)
            {
                BusinessRule rule = new BusinessRule();
                rule.EventType = eventtype;
                rule.Rule = item;
                rule.Id = item.Id;
                sitedb.Rules.AddOrUpdate(rule);
            }
        }

        private string getname<TEnum>(string value)
        {
            var names = Enum.GetNames(typeof(TEnum));

            string lower = value.ToLower();

            if (names == null)
            {
                return null;
            }

            foreach (var item in names)
            {
                if (item.ToLower() == lower)
                {
                    return item;
                }
            }
            return null;
        }

        public override List<object> List(ApiCall call)
        {
            List<eventcount> counts = new List<eventcount>();
            var sitedb = call.WebSite.SiteDb();
            var groupby = sitedb.Rules.List().GroupBy(o => o.EventType);

            foreach (var item in groupby)
            {
                var name = Enum.GetName(typeof(Kooboo.Sites.FrontEvent.enumEventType), item.Key);
                counts.Add(new eventcount() { name = name, count = item.Count() });
            }
            return counts.ToList<object>();
        }

        public class eventcount
        {
            public string name { get; set; }
            public int count { get; set; }
        }

        public virtual List<IFElseRule> ListByEvent(string eventname, ApiCall call)
        {
            var enumvalue = Lib.Helper.EnumHelper.GetEnum<Kooboo.Sites.FrontEvent.enumEventType>(eventname);

            var sitedb = call.WebSite.SiteDb();

            return convert(sitedb.Rules.List().Where(o => o.EventType == enumvalue).ToList());
        }

        List<IFElseRule> convert(List<BusinessRule> rules)
        {
            if (rules == null)
            {
                return null;
            }
            List<IFElseRule> converted = new List<IFElseRule>();
            foreach (var item in rules)
            {
                converted.Add(item.Rule);
            }
            return converted;
        }


        public List<EventConditionSetting> ConditionOption(string eventname, ApiCall call)
        {
            var eventtype = Lib.Helper.EnumHelper.GetEnum<Kooboo.Sites.FrontEvent.enumEventType>(eventname);

            return Kooboo.Sites.FrontEvent.Manager.GetConditionSetting(eventtype, call.Context);

        }

        public virtual void DeleteRule(Guid id, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            sitedb.Rules.Delete(id);
        }

    }
}
