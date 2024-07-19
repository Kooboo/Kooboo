using Kooboo.Api;
using Kooboo.Sites.ScriptModules.Models;

namespace Kooboo.Web.Api.Implementation.Modules
{

    public class ModuleConfig : TextResourceApi
    {
        public override EnumResourceType ResourceType => EnumResourceType.undefined;

        public override string ModelName => "Config";

        public string Read(string type, ApiCall call)
        {
            var context = this.GetModuleContext(call);

            string FileName = System.IO.Path.Combine(context.RootFolder, "module.config");

            if (System.IO.File.Exists(FileName))
            {
                return System.IO.File.ReadAllText(FileName);
            }

            return DefaultJson();
        }

        public void Write(string type, string content, ApiCall call)
        {
            var context = this.GetModuleContext(call);

            string FileName = System.IO.Path.Combine(context.RootFolder, "module.config");

            System.IO.File.WriteAllText(FileName, content);
        }




        private string DefaultJson()
        {
            string Json = "{customKey: \"value\", RefName: \"unique_name\", Readme: \"Description of your module\"}";
            return Json;
        }

    }


}
