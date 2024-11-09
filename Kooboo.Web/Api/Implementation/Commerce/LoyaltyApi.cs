using System.Linq;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce.Condition;
using Kooboo.Sites.Commerce.DataStorage;
using Kooboo.Sites.Commerce.Entities;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Sites.Commerce.ViewModels;
using Kooboo.Sites.Commerce.ViewModels.Loyalty;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Commerce;

public class LoyaltyApi : CommerceApi
{
    public override string ModelName => "Loyalty";

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.VIEW)]
    [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.VIEW)]
    public Membership[] Memberships(ApiCall apiCall)
    {
        var commerce = GetSiteCommerce(apiCall);
        var memberships = commerce.Membership.Entities.OrderBy(o => o.Priority);
        return [.. memberships];
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.EDIT)]
    public void CreateMembership(ApiCall apiCall)
    {
        var siteDb = apiCall.WebSite.SiteDb();
        var body = apiCall.Context.Request.Body;
        var model = JsonSerializer.Deserialize<MembershipCreate>(body, Defaults.JsonSerializerOptions);
        var shipping = model.ToMembership();
        siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
        {
            Id = Lib.Security.Hash.ComputeHashGuid(shipping.Id),
            Name = $"Membership:{model.Name}",
            Body = JsonSerializer.Serialize(shipping, Defaults.JsonSerializerOptions),
            Type = Sites.Models.CommerceDataType.Membership
        }, apiCall.Context.User.Id);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.DELETE)]
    public void DeleteMemberships(string[] ids, ApiCall apiCall)
    {
        var siteDb = apiCall.WebSite.SiteDb();
        var commerce = GetSiteCommerce(apiCall);
        foreach (var id in ids)
        {
            var guid = Lib.Security.Hash.ComputeHashGuid(id);
            var exist = siteDb.CommerceData.Get(guid);
            if (exist == default)
            {
                commerce.Membership.Delete(d => d.Id == id);
            }
            else
            {
                siteDb.CommerceData.Delete(guid, apiCall.Context.User.Id);
            }
        }
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.EDIT)]
    public void EditMembership(ApiCall apiCall)
    {
        var siteDb = apiCall.WebSite.SiteDb();
        var commerce = GetSiteCommerce(apiCall);
        var body = apiCall.Context.Request.Body;
        var model = JsonSerializer.Deserialize<MembershipEdit>(body, Defaults.JsonSerializerOptions);
        var entity = commerce.Membership.Entities.FirstOrDefault(g => g.Id == model.Id);
        model.UpdateMembership(entity);
        siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
        {
            Id = Lib.Security.Hash.ComputeHashGuid(entity.Id),
            Name = $"Membership:{model.Name}",
            Body = JsonSerializer.Serialize(entity, Defaults.JsonSerializerOptions),
            Type = Sites.Models.CommerceDataType.Membership
        }, apiCall.Context.User.Id);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    public MembershipEdit GetMembershipEdit(string id, ApiCall apiCall)
    {
        var commerce = GetSiteCommerce(apiCall);
        var entity = commerce.Membership.Entities.FirstOrDefault(g => g.Id == id);
        return new MembershipEdit(entity);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.VIEW)]
    [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.VIEW)]
    public Membership GetMembership(string id, ApiCall apiCall)
    {
        var commerce = GetSiteCommerce(apiCall);
        var body = apiCall.Context.Request.Body;
        var entity = commerce.Membership.Entities.FirstOrDefault(c => c.Id == id);
        return entity;
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    public object MembershipSchemas(ApiCall call)
    {
        return MembershipMatcher.Instance.GetOptionDetails(call.Context);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    public object Members(ApiCall apiCall)
    {
        var commerce = GetSiteCommerce(apiCall);
        var body = apiCall.Context.Request.Body;
        var model = JsonSerializer.Deserialize<MemberQuery>(body, Defaults.JsonSerializerOptions);
        var service = new LoyaltyService(commerce);
        return service.Members(model);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    public object Member(string id, ApiCall apiCall)
    {
        var commerce = GetSiteCommerce(apiCall);
        var service = new LoyaltyService(commerce);
        return service.Member(id);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.EDIT)]
    public void ChangeMembership(string id, string membershipId, ApiCall call)
    {
        var commerce = GetSiteCommerce(call);
        var service = new MembershipService(commerce);
        service.Change(id, membershipId);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.EDIT)]
    public void RenewMembership(string id, ApiCall call)
    {
        var commerce = GetSiteCommerce(call);
        var service = new MembershipService(commerce);
        service.Renew(id);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    public PagingResult CustomerMemberships(ApiCall call)
    {
        var commerce = GetSiteCommerce(call);
        var body = call.Context.Request.Body;
        var model = JsonSerializer.Deserialize<CustomerMembershipQuery>(body, Defaults.JsonSerializerOptions);
        var service = new MembershipService(commerce);
        return service.CustomerMemberships(model);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    public PagingResult PointList(ApiCall call)
    {
        var commerce = GetSiteCommerce(call);
        var body = call.Context.Request.Body;
        var model = JsonSerializer.Deserialize<RewardPointQuery>(body, Defaults.JsonSerializerOptions);
        var service = new RewardPointService(commerce);
        return service.List(model);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.EDIT)]
    public void EarnPoints(string id, int points, ApiCall call)
    {
        var commerce = GetSiteCommerce(call);
        var rewardPointService = new RewardPointService(commerce);
        var description = call.GetValue("description");
        rewardPointService.Change(new CustomerPointModel
        {
            CustomerId = id,
            Type = CustomerPointModel.ChangeType.ManualEarn,
            Points = points,
            Description = description
        });
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.EDIT)]
    public void RedeemPoints(string id, int points, ApiCall call)
    {
        var commerce = GetSiteCommerce(call);
        var rewardPointService = new RewardPointService(commerce);
        var description = call.GetValue("description");
        rewardPointService.Change(new CustomerPointModel
        {
            CustomerId = id,
            Type = CustomerPointModel.ChangeType.ManualRedeem,
            Points = -points,
            Description = description
        });
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    public object OrderEarnSchemas(ApiCall call)
    {
        return OrderEarnPointsMatcher.Instance.GetOptionDetails(call.Context);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    public object OrderRedeemSchemas(ApiCall call)
    {
        return OrderRedeemPointsMatcher.Instance.GetOptionDetails(call.Context);
    }

    [Permission(Feature.COMMERCE_LOYALTY, Action = Data.Permission.Action.VIEW)]
    public object LoginEarnSchemas(ApiCall call)
    {
        return LoginEarnPointsMatcher.Instance.GetOptionDetails(call.Context);
    }
}