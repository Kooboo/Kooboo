using Kooboo.Sites.ScriptModules.Models;

namespace Kooboo.Web.Api.Implementation.Modules
{

    public class ModuleCode : TextResourceApi
    {
        public override string ModelName => "ModuleCode";

        public override string objectType => "code";

        public override EnumResourceType ResourceType => EnumResourceType.Api;
    }

}
