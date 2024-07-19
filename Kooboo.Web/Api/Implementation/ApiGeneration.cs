//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class ApiGeneration : IApi
    {
        public string ModelName => "ApiGeneration";

        public bool RequireSite => true;

        public bool RequireUser => true;

        public List<ApiGenerationViewModel> Objects(ApiCall call)
        {
            List<ApiGenerationViewModel> result = new List<ApiGenerationViewModel>();

            var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);
            var list = db.GetTables();

            list.RemoveAll(o => o.StartsWith("_sys_"));

            list.RemoveAll(o => o.StartsWith("_koobootemp"));

            var actions = getActions();

            foreach (var item in list)
            {
                string type = "Database";
                string typeDisplayName = Kooboo.Data.Language.Hardcoded.GetValue("Database", call.Context);
                ApiGenerationViewModel model = new ApiGenerationViewModel();
                model.Type = type;
                model.TypeDisplayName = typeDisplayName;
                model.Name = item;
                model.DisplayName = item;
                model.Actions = actions;
                result.Add(model);
            }
            var sitedb = call.Context.WebSite.SiteDb();
            var folders = sitedb.ContentFolders.All();

            foreach (var item in folders)
            {
                string type = "TextContent";
                string TypeDisplayName = Data.Language.Hardcoded.GetValue("TextContent", call.Context);
                ApiGenerationViewModel model = new ApiGenerationViewModel();
                model.Type = type;
                model.TypeDisplayName = TypeDisplayName;
                model.Name = item.Name;
                model.DisplayName = item.DisplayName;
                model.Actions = actions;
                result.Add(model);
            }
            return result;
        }

        private List<string> getActions()
        {
            List<string> result = new List<string>();
            result.Add("add");
            result.Add("update");
            result.Add("delete");
            result.Add("get");
            result.Add("list");
            return result;
        }

        public bool Generate(List<ApiGenerationViewModel> updatemodel, ApiCall call)
        {
            var website = call.WebSite;

            foreach (var item in updatemodel.GroupBy(o => o.Type))
            {
                var key = item.Key.ToLower();
                var list = item.ToList();

                if (key == "database")
                {
                    foreach (var name in list)
                    {
                        Kooboo.Web.JQL.CodeGeneration.GenerateDatabase(website, name.Name, name.Actions);
                    }
                }
                else if (key == "textcontent")
                {
                    foreach (var name in list)
                    {
                        Kooboo.Web.JQL.CodeGeneration.GenerateTextContent(website, name.Name, name.Actions);
                    }
                }
            }
            return true;
        }

    }
}
