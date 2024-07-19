using Kooboo.Sites.ScriptModules.Models;

namespace Kooboo.Web.Api.Implementation.Modules
{
    public class ModuleCss : TextResourceApi
    {
        public override string ModelName => "ModuleCss";

        public override string objectType => "css";

        public override EnumResourceType ResourceType => EnumResourceType.css;
    }
}
