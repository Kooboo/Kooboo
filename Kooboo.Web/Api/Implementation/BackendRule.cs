//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Language;
using Kooboo.Data.Permission;
using Kooboo.Sites.BackendEvent;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.FrontEvent;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class BackendRuleApi : SiteObjectApi<BackendRule>
    {
        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.EDIT)]
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

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.EDIT)]
        public virtual void Post(string eventName, List<IFElseRule> rules, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            string righteventname = getname<Sites.BackendEvent.EventType>(eventName);

            Enum.TryParse(righteventname, out Sites.BackendEvent.EventType eventType);
            var oldRules = sitedb.BackendRules.List().Where(o => o.EventType == eventType).ToList();

            foreach (var item in oldRules)
            {
                if (rules.All(a => a.Id != item.Id))
                {
                    sitedb.BackendRules.Delete(item.Id);
                }
            }

            foreach (var item in rules)
            {
                var rule = new BackendRule
                {
                    EventType = eventType,
                    Rule = item,
                    Id = item.Id
                };
                sitedb.BackendRules.AddOrUpdate(rule);
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

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            return sitedb
                .BackendRules
                .List()
                .GroupBy(o => o.EventType)
                .Select(item =>
                {
                    var name = Enum.GetName(typeof(EventType), item.Key);
                    return new eventcount() { name = name, count = item.Count() };
                })
                .OrderBy(it => it.name)
                .ToList<object>();
        }

        public class eventcount
        {
            public string name { get; set; }
            public int count { get; set; }
        }

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.VIEW)]
        public virtual List<IFElseRule> ListByEvent(string eventname, ApiCall call)
        {
            var enumvalue = Lib.Helper.EnumHelper.GetEnum<EventType>(eventname);
            var sitedb = call.WebSite.SiteDb();
            var list = sitedb.BackendRules.List().Where(o => o.EventType == enumvalue).Select(s => s.Rule).ToList();

            foreach (var item in list)
            {
                SetCodeSource(item, call.Context.WebSite.SiteDb());
            }

            return list;
        }

        void SetCodeSource(IFElseRule rule, SiteDb siteDb)
        {
            if (rule.Do != null)
            {
                foreach (var action in rule.Do)
                {
                    if (action.Code == null && action.CodeId != default)
                    {
                        var code = siteDb.Code.Get(action.CodeId);

                        if (code != null)
                        {
                            action.Code = code.Body;
                        }
                    }
                }
            }

            if (rule.Then != null)
            {
                foreach (var i in rule.Then)
                {
                    SetCodeSource(i, siteDb);
                }
            }

            if (rule.Else != null)
            {
                foreach (var i in rule.Else)
                {
                    SetCodeSource(i, siteDb);
                }
            }
        }

        public List<EventConditionSetting> ConditionOption(string eventname, ApiCall call)
        {
            var eventType = Lib.Helper.EnumHelper.GetEnum<EventType>(eventname);

            var allTypes = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IBackendEvent));

            foreach (var item in allTypes)
            {

                if (Activator.CreateInstance(item) is IBackendEvent instance && instance.EventType == eventType)
                {
                    return instance.GetConditionSetting(call.Context);
                }
            }

            return [];
        }

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.EDIT)]
        public virtual void DeleteRule(Guid id, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            sitedb.BackendRules.Delete(id);
        }

        public object[] EventList(ApiCall call)
        {

            return Enum.GetNames(typeof(EventType)).Select(s =>
            {
                var attribute = Enum.Parse<EventType>(s).GetAttributeOfType<CategoryAttribute>();

                return new
                {
                    Name = s,
                    Display = Hardcoded.GetValue(s, call.Context),
                    Category = Hardcoded.GetValue(attribute.Category, call.Context),
                };
            }).ToArray();
        }

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override bool IsUniqueName(ApiCall call)
        {
            return base.IsUniqueName(call);
        }

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }

        [Permission(Feature.BACKEND_EVENTS, Action = Data.Permission.Action.EDIT)]
        public void DeleteEvents(string[] names, ApiCall call)
        {
            var siteDb = call.Context.WebSite.SiteDb();
            var types = names.Select(Enum.Parse<EventType>).ToArray();
            var oldRules = siteDb.BackendRules.List().Where(o => types.Contains(o.EventType)).ToList();

            foreach (var item in oldRules)
            {
                siteDb.BackendRules.Delete(item.Id);
            }
        }
    }
}
