using Kooboo.Sites.ScriptModules.Models;

namespace Kooboo.Web.Api.Implementation.Modules
{

    public class ModuleView : TextResourceApi
    {
        public override string ModelName => "ModuleView";

        public override string objectType => "view";

        public override EnumResourceType ResourceType => EnumResourceType.view;
    }

}
