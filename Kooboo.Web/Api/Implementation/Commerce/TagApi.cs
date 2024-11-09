using System.Linq;
using Kooboo.Api;
using Kooboo.Sites.Commerce;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class CommerceTagApi : CommerceApi
    {
        public override string ModelName => "CommerceTag";

        public string[] List(ApiCall apiCall, string type)
        {
            var commerce = GetSiteCommerce(apiCall);
            var tagType = Enum.Parse<TagType>(type);
            var list = commerce.Tag.Entities.Where(w => w.Type == tagType);
            return list.Select(s => s.Name).ToArray();
        }

        public void Delete(ApiCall apiCall, string type, string name)
        {
            var commerce = GetSiteCommerce(apiCall);
            var tagType = Enum.Parse<TagType>(type);
            var list = commerce.Tag.Entities.Where(w => w.Type == tagType && w.Name == name);
            commerce.Tag.Delete(list.Select(s => s.Id).ToArray());
        }
    }
}
