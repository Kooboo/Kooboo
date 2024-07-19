using Kooboo.Api;
using Kooboo.Sites.Storage;

namespace Kooboo.Web.Api.Implementation;

public abstract class StorageBase
{
    protected IStorageProvider GetStorageProvider(ApiCall call)
    {
        var provider = call.GetValue("provider");
        return StorageProviderFactory.GetProvider(provider, call.Context);
    }
}
