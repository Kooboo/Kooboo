using System.Linq;
using System.Text;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Mail;
using Kooboo.Sites.Extensions;


namespace Kooboo.Web.Api.Implementation
{
    public class SpaMultilingual : IApi
    {
        public string ModelName => "SpaMultilingual";

        public bool RequireSite => true;

        public bool RequireUser => true;

        [Permission(Feature.SPA_LANG, Action = Data.Permission.Action.EDIT)]
        public void Import(ApiCall apiCall)
        {
            var file = apiCall.Context.Request.Files.FirstOrDefault();
            var importType = apiCall.Context.Request.GetValue("importType");
            if (file == default) throw new Exception("file not found");
            var json = Encoding.UTF8.GetString(file.Bytes);
            var root = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
            var invalidItem = root.FirstOrDefault(f => f.Key.Length > 50);
            if (invalidItem.Key != default) throw new Exception($"Key: '{invalidItem.Key}' length should less or equal to 50");
            var spaMultilingualRepository = apiCall.Context.WebSite.SiteDb().SpaMultilingual;
            var old = spaMultilingualRepository.All();
            var langs = old.SelectMany(s => s.Value.Select(m => m.Key)).Distinct();

            foreach (var item in root)
            {
                var model = spaMultilingualRepository.GetByNameOrId(item.Key);

                model ??= new Sites.Models.SpaMultilingual
                {
                    Name = item.Key,
                    DefaultLang = item.Value.FirstOrDefault().Key?.ToLower(),
                    Value = new Dictionary<string, string>()
                };

                foreach (var kv in item.Value)
                {
                    if (string.IsNullOrEmpty(kv.Value)) continue;

                    if (importType != "replace")
                    {
                        model.Value.TryGetValue(kv.Key?.ToLower(), out var value);
                        if (!string.IsNullOrEmpty(value)) continue;
                    }

                    model.Value.SetValue(kv.Key?.ToLower(), kv.Value);
                }

                foreach (var lang in langs)
                {
                    if (model.Value.ContainsKey(lang)) continue;
                    model.Value.Add(lang, null);
                }

                spaMultilingualRepository.AddOrUpdate(model);
            }
        }

        [Permission(Feature.SPA_LANG, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<Sites.Models.SpaMultilingual> List(ApiCall apiCall)
        {
            var spaMultilingualRepository = apiCall.Context.WebSite.SiteDb().SpaMultilingual;
            return spaMultilingualRepository.All().OrderBy(it => it.Name);
        }

        [Permission(Feature.SPA_LANG, Action = Data.Permission.Action.EDIT)]
        public void Post(Sites.Models.SpaMultilingual model, ApiCall apiCall)
        {
            var spaMultilingualRepository = apiCall.Context.WebSite.SiteDb().SpaMultilingual;
            spaMultilingualRepository.AddOrUpdate(model);
        }

        [Permission(Feature.SPA_LANG, Action = Data.Permission.Action.EDIT)]
        public void SetLang(string[] model, ApiCall apiCall)
        {
            var spaMultilingualRepository = apiCall.Context.WebSite.SiteDb().SpaMultilingual;
            var list = spaMultilingualRepository.All();

            foreach (var item in list)
            {
                var removedKeys = item.Value.Keys.Except(model);

                foreach (var key in removedKeys)
                {
                    item.Value.Remove(key);
                }

                foreach (var key in model)
                {
                    if (!item.Value.ContainsKey(key))
                    {
                        item.Value.Add(key, null);
                    }
                }

                spaMultilingualRepository.AddOrUpdate(item);
            }
        }

        [Permission(Feature.SPA_LANG, Action = Data.Permission.Action.DELETE)]
        public bool Deletes(ApiCall call)
        {
            string json = call.GetValue("ids");

            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }

            var ids = JsonHelper.Deserialize<Guid[]>(json);

            var spaMultilingualRepository = call.Context.WebSite.SiteDb().SpaMultilingual;

            foreach (var id in ids)
            {
                spaMultilingualRepository.Delete(id);
            }

            return true;
        }

    }
}
