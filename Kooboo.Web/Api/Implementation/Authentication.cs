using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Interface;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;

namespace Kooboo.Web.Api.Implementation
{
    public class Authentication : SiteObjectApi<Sites.Models.Authentication>
    {
        public object GetTypes()
        {
            return new
            {
                Matcher = GetEnumDescription(typeof(MatcherType)),
                Action = GetEnumDescription(typeof(AuthenticationAction)),
                FailedAction = GetEnumDescription(typeof(FailedAction)),
            };
        }

        private object GetEnumDescription(Type type)
        {
            return Enum.GetNames(type).Select(s => new
            {
                Name = s,
                Display = s
            });
        }

        [Permission(Feature.AUTHENTICATION, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.AUTHENTICATION, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.AUTHENTICATION, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.AUTHENTICATION, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.AUTHENTICATION, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            var result = base.List(call);

            foreach (var item in result)
            {
                if (item is not Sites.Models.Authentication auth) continue;
                if (string.IsNullOrWhiteSpace(auth.CustomCodeName)) continue;
                if (auth.CustomCode != null) continue;
                auth.CustomCode = call.Context.WebSite.SiteDb().Code.GetByNameOrId(auth.CustomCodeName)?.Body;
            }

            return result.OrderByDescending(it => ((ISiteObject)it).LastModified).ToList<object>();
        }

        [Permission(Feature.AUTHENTICATION, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.AUTHENTICATION, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }
    }
}
