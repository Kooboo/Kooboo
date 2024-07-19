using Kooboo.Data.Context;

namespace Kooboo.Api
{
    public interface ISecureApi : IApi
    {
        bool AccessCheck(ApiCommand command, RenderContext call);
    }
}
