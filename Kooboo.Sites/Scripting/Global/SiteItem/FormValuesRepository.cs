using Kooboo.Data.Context;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Scripting.Global.SiteItem
{
    public class FormValuesRepository : RepositoryBase
    {
        public FormValuesRepository(IRepository repo, RenderContext context) : base(repo, context)
        {
        }
    }
}