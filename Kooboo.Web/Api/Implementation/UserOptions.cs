namespace Kooboo.Web.Api.Implementation;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites;
using Kooboo.Sites.Extensions;
using Kooboo.Web.ViewModel;

public class UserOptionsApi : IApi
{
    public string ModelName => "UserOptions";

    public bool RequireSite => true;

    public bool RequireUser => true;

    [Permission(Feature.USER_OPTIONS, Action = Action.VIEW)]
    public object All(ApiCall call)
    {
        var siteDb = call.WebSite.SiteDb();
        return siteDb.UserOptions.All();
    }

    [Permission(Feature.USER_OPTIONS, Action = Action.VIEW)]
    public object Get(Guid id, ApiCall call)
    {
        var siteDb = call.WebSite.SiteDb();
        return siteDb.UserOptions.Get(id);
    }

    [Permission(Feature.USER_OPTIONS, Action = "setting")]
    public void AddOrUpdate(UserOptionsViewModel model, ApiCall call)
    {
        var siteDb = call.WebSite.SiteDb();
        var entity = siteDb.UserOptions.GetByNameOrId(model.Name);
        if (entity == default)
        {
            entity = new Sites.Models.UserOptions
            {
                Name = model.Name,
            };
        }

        entity.Display = model.Display;
        entity.Schema = model.Schema;
        UserOptionsService.AddOrUpdate(call.Context, entity);
    }

    [Permission(Feature.USER_OPTIONS, Action = Action.EDIT)]
    public void UpdateOptions(Guid id, string setting, ApiCall call)
    {
        var siteDb = call.WebSite.SiteDb();
        var entity = siteDb.UserOptions.Get(id);
        entity.Data = setting;
        UserOptionsService.AddOrUpdate(call.Context, entity);
    }

    [Permission(Feature.USER_OPTIONS, Action = Action.DELETE)]
    public void Deletes(ApiCall call, Guid[] ids)
    {
        var siteDb = call.WebSite.SiteDb();
        foreach (var item in ids)
        {
            siteDb.UserOptions.Delete(item);
            UserOptionsService.Remove(call.WebSite.Id, item);
        }
    }

    [Permission(Feature.USER_OPTIONS, Action = Data.Permission.Action.VIEW)]
    public bool IsUniqueName(string name, ApiCall call)
    {
        var sitedb = call.WebSite.SiteDb();

        var id = Kooboo.Data.IDGenerator.Generate(name, ConstObjectType.UserOptions);
        var model = sitedb.UserOptions.Get(id);

        return model == null;
    }
}