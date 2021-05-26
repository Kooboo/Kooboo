using Kooboo.Api;
using Kooboo.Data.Interface;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kooboo.Web.Api.Implementation
{
    public class CodeSearch : IApi
    {
        public string ModelName => "CodeSearch";

        public bool RequireSite => true;

        public bool RequireUser => false;

        public List<CodeResult> List(ApiCall apiCall)
        {
            var keyword = apiCall.GetValue("keyword");
            return Search(apiCall.Context.WebSite.SiteDb(), keyword);
        }

        private List<CodeResult> Search(SiteDb sitedb, string keyword)
        {
            List<CodeResult> results = new List<CodeResult>();

            List<IRepository> ReposToCheck = new List<IRepository>();


            foreach (var Repo in SiteRepositoryContainer.Repos)
            {
                var currentRepo = sitedb.GetSiteRepository(Repo.Value);
                if (currentRepo != null)
                {
                    if (Lib.Reflection.TypeHelper.HasInterface(currentRepo.ModelType, typeof(ITextObject)))
                    {
                        ReposToCheck.Add(currentRepo);
                    }
                }
            }

            foreach (var repo in ReposToCheck)
            {
                var items = repo.All();
                foreach (var item in items)
                {
                    var TextItem = item as ITextObject;

                    if (TextItem != null)
                    {
                        var text = TextItem.Body;
                        var FindResult = Lib.Helper.StringHelper.FindText(text, keyword);
                        if (FindResult != null && FindResult.Any())
                        {
                            var info = GetDisplayInfo(FindResult, sitedb, item);
                            results.Add(info);
                        }

                    }

                }


            }


            return results;
        }

        public CodeResult GetDisplayInfo(List<FindResult> finds, SiteDb sitedb, ISiteObject item)
        {
            CodeResult model = new CodeResult();
            model.Id = item.Id;
            var info = Kooboo.Sites.Service.ObjectService.GetObjectInfo(sitedb, item);

            if (info != null)
            {
                model.Type = info.ModelType.Name;
                model.Name = info.DisplayName;
                model.Lines =finds;
                // Edit URL.  
                model.Url = Kooboo.Sites.Service.ObjectService.GetEditRoute(item, sitedb);
            }
            else
            {
                model.Name = item.Name;
                model.Lines = finds;
            }

            return model;
        }

        private string GetLineResult(List<FindResult> finds)
        {
            string result = string.Empty;
            foreach (var item in finds)
            {
                result += "line " + item.LineNumber.ToString() + " | " + item.Summary + "<br />";
            }
            return result;
        }
    }


    public class CodeResult
    {

        public Guid Id { get; set; }

        //format url. with {id} in it
        public string Url { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }

        public List<FindResult> Lines { get; set; }

    }
}
