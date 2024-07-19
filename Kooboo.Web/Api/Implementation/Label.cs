//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Permission;
using Kooboo.Sites.Automation.SPA;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class LabelApi : SiteObjectApi<Label>
    {
        [Permission(Feature.LABEL, Action = Data.Permission.Action.EDIT)]
        public void Update(Guid id, Dictionary<string, string> values, ApiCall call)
        {
            var label = call.WebSite.SiteDb().Labels.Get(call.ObjectId);
            if (label != null)
            {
                foreach (var item in values)
                {
                    label.SetValue(item.Key, item.Value);
                }
                call.WebSite.SiteDb().Labels.AddOrUpdate(label, call.Context.User.Id);
            }
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.EDIT)]
        public void Create(string key, Dictionary<string, string> values, ApiCall call)
        {

            Label label = new Label();

            if (System.Guid.TryParse(key, out var id))
            {
                label.Name = Lib.Helper.StringHelper.ToValidFileName(values.First().Value);
            }
            else
            {
                label.Name = key;
            }

            foreach (var item in values)
            {
                label.SetValue(item.Key, item.Value);
            }
            call.WebSite.SiteDb().Labels.AddOrUpdate(label, call.Context.User.Id);
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            List<LabelItemViewModel> result = new List<LabelItemViewModel>();

            var sitedb = call.WebSite.SiteDb();

            int storeNameHash = Lib.Security.Hash.ComputeInt(sitedb.Labels.StoreName);

            foreach (var item in sitedb.Labels.All())
            {
                LabelItemViewModel model = new LabelItemViewModel();
                model.Id = item.Id;
                model.Name = item.Name;
                model.KeyHash = Sites.Service.LogService.GetKeyHash(item.Id);
                model.StoreNameHash = storeNameHash;
                model.LastModified = item.LastModified;
                model.Values = item.Values;
                var relations = sitedb.Labels.GetUsedBy(item.Id);
                model.Relations = Sites.Helper.RelationHelper.Sum(relations);
                model.RelationDetails = relations.Select(s =>
                new
                {
                    Name = s.Name,
                    Id = s.ObjectId,
                    type = s.ModelType.Name
                });
                result.Add(model);
            }
            return result.ToList<object>();
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.VIEW)]
        public List<string> Keys(ApiCall call)
        {
            List<string> keys = new List<string>();
            foreach (var item in call.WebSite.SiteDb().Labels.All())
            {
                keys.Add(item.Name);
            }
            return keys;
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.EDIT)]
        public List<LangKeyResult> Scan(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb(); 

            return new DomKeys().ScanSite(sitedb);
        }


        [Permission(Feature.LABEL, Action = Data.Permission.Action.EDIT)]
        public void ConfirmAdd(List<LangKeyResult> model, ApiCall call)
        {
            if (model == null || model.Count == 0)
            {
                throw new Exception("invalid data");
            } 
            new DomKeys().UpdateSite(call.Context.WebSite.SiteDb(), model);
        }

        [Permission(Feature.LABEL, Action = Data.Permission.Action.EDIT)]
        public List<LangKeyResult> ScanI18N(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var JsKeys = new JsKeys();

            return JsKeys.ScanSite(sitedb);
        }


        [Permission(Feature.LABEL, Action = Data.Permission.Action.EDIT)]
        public void ConfirmI18NAdd(List<LangKeyResult> model, ApiCall call)
        {
            if (model == null || model.Count == 0)
            {
                throw new Exception("invalid data");
            }  
            new JsKeys().UpdateSite(call.Context.WebSite.SiteDb(), model);
        }



        //public override List<object> List(ApiCall apiCall)
        //{
        //    return apiCall.Context.WebSite.SiteDb().Labels.All(); 
        //}


        public Dictionary<string, string> LangList(ApiCall call)
        {
            var userCulture = call.Context.Culture;
            return ListByLang(userCulture, call);
        }

        public Dictionary<string, string> ListByLang(string lang, ApiCall call)
        {
            Dictionary<string, string> UserCultureValue = new Dictionary<string, string>();

            var all = call.Context.WebSite.SiteDb().Labels.All();

            foreach (var item in all)
            {
                var value = item.GetValue(lang);

                if (value != null)
                {
                    UserCultureValue.Add(item.Name, value.ToString());
                }
            }
            return UserCultureValue;
        }


        public PlainResponse Json(ApiCall call)
        {
            Dictionary<string, string> values = LangList(call);

            string json = System.Text.Json.JsonSerializer.Serialize(values);
            PlainResponse response = new PlainResponse();
            response.Content = json;
            response.ContentType = "application/json";
            response.statusCode = 200;
            return response;
        }


        public BinaryResponse Export(ApiCall call)
        {

            var website = call.Context.WebSite;
            string json = null;

            var AllLabels = call.Context.WebSite.SiteDb().Labels.All();

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };

            if (website.EnableMultilingual)
            {
                Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();

                foreach (var item in website.Culture)
                {

                    Dictionary<string, string> langKeys = new Dictionary<string, string>();

                    foreach (var la in AllLabels)
                    {
                        var value = la.GetValue(item.Key);

                        if (value == null)
                        {
                            value = "";
                        }

                        langKeys[la.Name] = value.ToString();
                    }

                    result.Add(item.Key, langKeys);
                }

                json = System.Text.Json.JsonSerializer.Serialize(result, options);
            }
            else
            {

                Dictionary<string, string> langResult = new Dictionary<string, string>();

                var culture = website.DefaultCulture;

                foreach (var item in AllLabels)
                {
                    var value = item.GetValue(culture);

                    string textValue;
                    if (value == null)
                    {
                        textValue = "";
                    }
                    else
                    {
                        textValue = value.ToString();
                    }

                    langResult.Add(item.Name, textValue);

                }

                json = System.Text.Json.JsonSerializer.Serialize(langResult, options);

            }


            BinaryResponse response = new BinaryResponse();
            response.ContentType = "application/octet-stream";
            response.Headers.Add("Content-Disposition", $"attachment;filename=label.json");
            response.BinaryBytes = System.Text.Encoding.UTF8.GetBytes(json);

            return response;
        }

        public void Import(ApiCall call)
        {
            // Import Json. 
            var files = call.Context.Request.Files;

            if (files == null || files.Count() == 0 || !files[0].FileName.ToLower().EndsWith(".json"))
            {
                throw new Exception("Parameter not valid");

            }

            var json = System.Text.Encoding.UTF8.GetString(files[0].Bytes);


            // import may have change the lang setting. 
            var sitedb = call.Context.WebSite.SiteDb();

            try
            {
                var langList = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);

                if (langList != null && langList.First().Value.Count() > 0)
                {

                    var firstLang = langList.First();
                    var culture = firstLang.Key;

                    foreach (var item in firstLang.Value)
                    {

                        var label = sitedb.Labels.Get(item.Key);
                        if (label == null)
                        {
                            label = new Label() { Name = item.Key };
                        }

                        label.SetValue(culture, item.Value);

                        foreach (var lang in langList)
                        {
                            if (lang.Key != culture)
                            {
                                if (lang.Value.TryGetValue(item.Key, out string langvalue))
                                {
                                    label.SetValue(lang.Key, langvalue);
                                }
                            }
                        }

                        sitedb.Labels.AddOrUpdate(label);
                    }

                    return;   // has multilingual content. 

                }

            }
            catch (Exception)
            {

            }



            var defaultCulture = call.Context.WebSite.DefaultCulture;

            try
            {
                var DefaultCultureList = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (DefaultCultureList != null)
                {
                    foreach (var item in DefaultCultureList)
                    {
                        var label = sitedb.Labels.Get(item.Key);
                        if (label == null)
                        {
                            label = new Label() { Name = item.Key };
                        }
                        label.SetValue(defaultCulture, item.Value);

                        sitedb.Labels.AddOrUpdate(label);
                    }
                }
            }
            catch (Exception)
            {

            }


        }



    }
}
