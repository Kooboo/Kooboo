using System.Linq;
using System.Text.RegularExpressions;
using Kooboo.Api;
using Kooboo.Data.Interface;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;

namespace Kooboo.Web.Api.Implementation
{
    public class CodeSearch : IApi
    {
        public string ModelName => "CodeSearch";

        public bool RequireSite => true;

        public bool RequireUser => false;

        [Permission(Feature.CODE, Action = Data.Permission.Action.VIEW)]
        public List<CodeResult> List(ApiCall apiCall)
        {
            var keyword = apiCall.GetValue("keyword");
            return SearchForNormal(apiCall.Context.WebSite.SiteDb(), keyword);
        }

        public record SearchResult(string Name, string Type, Guid Id, List<FindResult> matched, Dictionary<string, object> Params);
        public static string[] includes = new[] { "Page", "Script", "Style", "Menu", "Layout", "View", "Form", "Code" };
        [Permission(Feature.CODE, Action = Data.Permission.Action.VIEW)]
        public List<SearchResult> Search(ApiCall apiCall)
        {
            var keyword = apiCall.GetValue("keyword");
            var isRegex = apiCall.GetBoolValue("isRegex");
            var ignoreCase = apiCall.GetBoolValue("ignoreCase");
            var sitedb = apiCall.Context.WebSite.SiteDb();
            var result = new List<SearchResult>();
            if (string.IsNullOrWhiteSpace(keyword)) return result;

            foreach (var repo in SiteRepositoryContainer.Repos)
            {
                if (!includes.Contains(repo.Key)) continue;
                var currentRepo = sitedb.GetSiteRepository(repo.Value);
                if (currentRepo == null) continue;
                if (Lib.Reflection.TypeHelper.HasInterface(currentRepo.ModelType, typeof(ITextObject)))
                {
                    var items = currentRepo.All();
                    foreach (var item in items)
                    {
                        bool nameMatched = false;

                        if (isRegex)
                        {
                            nameMatched = Regex.IsMatch(
                                item.Name, keyword,
                                ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None
                            );
                        }
                        else
                        {
                            var stringComparison = ignoreCase ?
                                    StringComparison.CurrentCultureIgnoreCase :
                                    StringComparison.CurrentCulture;

                            nameMatched = item.Name.Contains(keyword, stringComparison);
                        }

                        Dictionary<string, object> @params = new Dictionary<string, object>();
                        if (item is ITextObject textItem)
                        {
                            if (item is Page page)
                            {
                                @params.Add("type", page.Type);
                                @params.Add("layout", page.LayoutName);
                            }

                            if (item is Code code)
                            {
                                if (code.CodeType == CodeType.PageScript && code.IsEmbedded) continue;
                                if (code.CodeType == CodeType.Job) continue;
                                @params.Add("codeType", code.CodeType.ToString());
                            }

                            if (item is Script script)
                            {
                                if (script.IsEmbedded) continue;
                                @params.Add("ownerObjectId", script.OwnerObjectId);
                            }

                            if (item is Style style)
                            {
                                if (style.IsEmbedded) continue;
                                @params.Add("ownerObjectId", style.OwnerObjectId);
                            }

                            if (item is Form form)
                            {
                                if (form.IsEmbedded) continue;
                            }

                            List<FindResult> findResult = null;

                            if (!string.IsNullOrWhiteSpace(textItem.Body))
                            {
                                findResult = StringHelper.FindText(textItem.Body, keyword, isRegex, ignoreCase);
                            }

                            if (nameMatched || (findResult != null && findResult.Count > 0))
                            {
                                var name = string.IsNullOrWhiteSpace(item.Name)
                                    ? StringHelper.GetSummary(textItem.Body?.TrimStart())
                                    : item.Name;

                                result.Add(new SearchResult(name, repo.Key, item.Id, findResult, @params));
                            }
                        }
                    }
                }
            }

            return result;
        }

        private List<CodeResult> SearchForNormal(SiteDb sitedb, string keyword)
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
                        List<FindResult> FindResult = null;

                        if (!string.IsNullOrWhiteSpace(TextItem.Body))
                        {
                            FindResult = StringHelper.FindText(TextItem.Body, keyword);
                        }

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
                model.Lines = finds;
                // Edit URL.  
                model.Url = Kooboo.Sites.Service.ObjectService.GetEditRoute(item, sitedb);

                if (item is Page page)
                {
                    model.layoutId = Implementation.PageApi.GetLayoutId(page);
                    model.PageType = page.Type.ToString();
                }
                else if (item is TextContent content)
                {
                    model.folderId = content.FolderId;
                }
            }
            else
            {
                model.Name = item.Name;
                model.Lines = finds;
            }

            return model;
        }
    }


    public class CodeResult
    {

        public Guid Id { get; set; }

        //format url. with {id} in it
        public string Url { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public Guid layoutId { get; set; }
        public string PageType { get; set; }
        public Guid folderId { get; set; }
        public List<FindResult> Lines { get; set; }

    }
}
