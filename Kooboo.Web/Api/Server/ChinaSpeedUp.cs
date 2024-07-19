using Kooboo.Api;

namespace Kooboo.Web.Api.Server
{
    public class ChinaSpeedUp : IApi
    {
        public string ModelName => "";

        public bool RequireSite => false;

        public bool RequireUser => throw new System.NotImplementedException();
    }
}
