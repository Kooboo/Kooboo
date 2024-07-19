using Kooboo.Sites.ScriptModules.Models;

namespace Kooboo.Web.Api.Implementation.Modules
{
    //For Js resource... 
    public class ModuleJs : TextResourceApi
    {
        public override string ModelName => "ModuleJS";

        public override string objectType => "js";

        public override EnumResourceType ResourceType => EnumResourceType.js;
    }
}
