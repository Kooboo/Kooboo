namespace Kooboo.Web.Authorization
{
    public interface IApiPermissionString : IPermissionControl
    {
        // linked permission tree.
        // developement/view
        string Permission { get; }
    }
}