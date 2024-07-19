using System.ComponentModel;
using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.FrontEvent;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Microsoft.OpenApi.Extensions;

namespace Kooboo.Web.Api.Implementation
{


    public class CMSEvent : SiteObjectApi<BusinessRule>
    {
        public override string ModelName
        {
            get
            {
                return "CMSEvent";
            }
            set
            {

            }

        }


        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.EDIT)]
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

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.EDIT)]
        public virtual void Post(string eventName, List<IFElseRule> rules, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            string righteventname = getname<Sites.FrontEvent.enumEventType>(eventName);

            Enum.TryParse(righteventname, out Sites.FrontEvent.enumEventType eventtype);
            var oldRules = sitedb.Rules.List().Where(o => o.EventType == eventtype).ToList();

            foreach (var item in oldRules)
            {
                if (rules.All(a => a.Id != item.Id))
                {
                    sitedb.Rules.Delete(item.Id);
                }
            }

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

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            return sitedb
                .Rules
                .List()
                .GroupBy(o => o.EventType)
                .Select(item =>
                {
                    var name = Enum.GetName(typeof(enumEventType), item.Key);
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

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.VIEW)]
        public virtual List<IFElseRule> ListByEvent(string eventname, ApiCall call)
        {
            var enumvalue = Lib.Helper.EnumHelper.GetEnum<enumEventType>(eventname);
            var sitedb = call.WebSite.SiteDb();
            var list = sitedb.Rules.List().Where(o => o.EventType == enumvalue).Select(s => s.Rule).ToList();

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
            var eventtype = Lib.Helper.EnumHelper.GetEnum<Kooboo.Sites.FrontEvent.enumEventType>(eventname);

            return Kooboo.Sites.FrontEvent.Manager.GetConditionSetting(eventtype, call.Context);

        }

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.EDIT)]
        public virtual void DeleteRule(Guid id, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            sitedb.Rules.Delete(id);
        }

        public object[] EventList()
        {

            return Enum.GetNames(typeof(enumEventType)).Select(s =>
            {
                var attribute = Enum.Parse<enumEventType>(s).GetAttributeOfType<CategoryAttribute>();

                return new
                {
                    Name = s,
                    attribute.Category
                };
            }).ToArray();
        }

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override bool IsUniqueName(ApiCall call)
        {
            return base.IsUniqueName(call);
        }

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.FRONT_EVENTS, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }
    }

}
