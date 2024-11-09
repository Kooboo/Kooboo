//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class CodeApi : SiteObjectApi<Code>
    {
        [Permission(Feature.CODE, Action = Data.Permission.Action.VIEW)]
        public virtual CodeEditViewModel GetEdit(string codetype, ApiCall call)
        {
            call.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            call.Context.Response.Headers.Add("Access-Control-Allow-Headers", "*");

            var sitedb = call.WebSite.SiteDb();

            if (call.ObjectId != default(Guid))
            {
                var code = sitedb.Code.Get(call.ObjectId);
                return ToViewModel(sitedb, code);
            }
            else
            {
                CodeEditViewModel model = new CodeEditViewModel();
                model.ScriptType = ScriptType.Module.ToString();
                model.Config = configSample();

                if (codetype != null && codetype.ToLower() != "all")
                {
                    var enumcodetype = Lib.Helper.EnumHelper.GetEnum<CodeType>(codetype);
                    model.Body = getSample(enumcodetype);
                    model.CodeType = enumcodetype.ToString();
                }

                model.AvailableEventType = Enum.GetNames(typeof(Kooboo.Sites.FrontEvent.enumEventType)).ToList();
                model.AvailableCodeType = Enum.GetNames(typeof(Kooboo.Sites.Models.CodeType)).ToList();
                return model;
            }
        }

        public CodeEditViewModel GetByName(ApiCall call, string name)
        {
            call.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            call.Context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            var siteDb = call.WebSite.SiteDb();
            var code = siteDb.Code.GetByNameOrId(name);

            if (code == default)
            {
                throw new Exception($"Code {name} not found");
            }

            return ToViewModel(siteDb, code);
        }

        private static CodeEditViewModel ToViewModel(SiteDb siteDb, Code code)
        {
            var model = new CodeEditViewModel();
            model.Name = code.Name;
            model.Body = code.Body;
            model.Config = code.Config;
            model.EventType = Enum.GetName(typeof(Kooboo.Sites.FrontEvent.enumEventType), code.EventType);
            model.CodeType = Enum.GetName(typeof(Kooboo.Sites.Models.CodeType), code.CodeType);
            model.Version = code.Version;
            model.Id = code.Id;
            model.ScriptType = code.ScriptType.ToString();
            model.isEmbedded = code.IsEmbedded;
            if (code.IsCodeEncrypted && code.IsDecrypted)
            {
                model.Body = string.Empty;
                model.IsDecrypted = true;
            }

            if (code.CodeType == Sites.Models.CodeType.Api)
            {
                var route = siteDb.Routes.GetByObjectId(model.Id);
                if (route != null)
                {
                    model.Url = route.Name;
                }
            }

            return model;
        }

        private string configSample()
        {
            string text = "//// sample config code..... comment out and modify to write your own config \r\n\r\n";

            text += "//var config=[]; \r\n\r\n ";
            text += "//var item ={};\r\n  //item.name= 'name';\r\n \r\n";

            text += "//// controlType are: TextBox, TextArea, CheckBox, Selection; \r\n\r\n";

            text += "//item.controlType = \"TextBox\";\r\n";
            text += "//item.value = \"optionalDefaultValue\";\r\n";
            text += "//config.push(item);\r\n";

            text += "\r\n";
            text += "////For controltype = Selection, you neet to set selectionValues. \r\n";

            text += "//var selectitem ={};\r\n  //selectitem.name= 'choice';\r\n \r\n";

            text += "//selectitem.selectionvalues = {}; \r\n";
            text += "//selectitem.selectionvalues.keyone = \"valueone\";\r\n";
            text += "//selectitem.selectionvalues.keytwo = \"valuetwo\";\r\n";

            text += "//config.push(selectitem);\r\n\r\n";

            text += "////return them to Kooboo.\r\n";
            text += "//k.export(config);";

            return text;
        }

        private string getSample(CodeType codetype)
        {
            string sample = "";

            if (codetype == Sites.Models.CodeType.Api)
            {
                sample = @"// sample code.. 
//var obj = {}; 
//obj.name = ""myname""; 
//obj.fieldtwo = ""value of field two""; 
//k.response.setHeader(""Access-Control-Allow-Origin"", ""*""); 
//k.response.json(obj); ";

            }
            else if (codetype == Sites.Models.CodeType.PageScript)
            {
                sample = @"// kscript that can be inserted to page position. 
//k.cookie.set(""key"", ""value"");
//k.response.write(""Hello world"");";
            }
            else if (codetype == Sites.Models.CodeType.Job)
            {
                sample = @"//Schedule task";
            }

            else if (codetype == Sites.Models.CodeType.PaymentCallBack)
            {
                sample = @"// kscript that can be inserted to page position. 
//k.cookie.set(""key"", ""value"");
//k.response.write(""Hello world"");";
            }
            else if (codetype == Sites.Models.CodeType.Job)
            {
                sample = @"//Schedule task";
            }

            else if (codetype == Sites.Models.CodeType.CodeBlock)
            {
                sample = @"//Script block
//export function foo(){
//    return 'bar'
//}";
            }

            return sample;
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.EDIT)]
        public virtual Guid Post(CodeEditViewModel model, ApiCall call)
        {
            if (model.Url != null)
            {
                if (model.Url.StartsWith("\\"))
                {
                    model.Url = "/" + model.Url.Substring(1);
                }
                if (!model.Url.StartsWith("/"))
                {
                    model.Url = "/" + model.Url;
                }
            }
            else
            {
                throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("Url is required", call.Context));
            }

            var sitedb = call.WebSite.SiteDb();

            Code code = new Code();
            code.Name = model.Name;
            code.Config = model.Config;
            code.Body = model.Body;
            code.ScriptType = Enum.Parse<ScriptType>(model.ScriptType);

            if (HasScriptTag(code.Body))
            {
                throw new Exception("You do not need script tag in code. You need the tag in the page, view or layout");
            }

            if (!string.IsNullOrEmpty(model.EventType))
            {
                code.EventType = Lib.Helper.EnumHelper.GetEnum<Sites.FrontEvent.enumEventType>(model.EventType);
            }

            if (!string.IsNullOrEmpty(model.CodeType))
            {
                code.CodeType = Lib.Helper.EnumHelper.GetEnum<CodeType>(model.CodeType);
            }

            code.Id = model.Id;

            if (code.CodeType == Sites.Models.CodeType.Api)
            {
                if (!string.IsNullOrEmpty(model.Url) && !sitedb.Routes.Validate(model.Url, model.Id))
                {
                    // one more verify. 
                    var route = sitedb.Routes.Get(model.Url);
                    if (route != null && route.objectId != default(Guid))
                    {
                        var siteobjecttype = Kooboo.ConstTypeContainer.GetModelType(route.DestinationConstType);
                        if (siteobjecttype != null)
                        {
                            var repo = sitedb.GetSiteRepositoryByModelType(siteobjecttype);
                            if (repo != null)
                            {
                                var obj = repo.Get(route.objectId);
                                if (obj != null)
                                {
                                    throw new Exception(Data.Language.Hardcoded.GetValue("Url occupied", call.Context));
                                }
                            }
                        }
                    }

                }
                // check if it only return Json...
                code.IsJson = Lib.Helper.JsonHelper.IsJson(code.Body);
            }

            if (model.Id != default(Guid))
            {
                var oldcode = sitedb.Code.Get(model.Id);
                if (oldcode != null)
                {
                    (model as IDiffChecker).CheckDiff(oldcode);

                    // check if needed to change route. 
                    if (code.CodeType == Sites.Models.CodeType.Api)
                    {
                        var oldroute = sitedb.Routes.GetByObjectId(oldcode.Id);
                        if (oldroute != null && oldroute.Name != model.Url)
                        {
                            sitedb.Routes.ChangeRoute(oldroute.Name, model.Url);
                        }
                    }

                    oldcode.Name = model.Name;
                    oldcode.Body = model.Body;
                    oldcode.Config = model.Config;
                    oldcode.ScriptType = Enum.Parse<ScriptType>(model.ScriptType);
                    oldcode.IsJson = Lib.Helper.JsonHelper.IsJson(oldcode.Body);

                    if (oldcode.IsEmbedded && oldcode.CodeType == Sites.Models.CodeType.PageScript)
                    {
                        sitedb.Code.AddOrUpdate(oldcode, true, true, call.Context.User.Id);
                    }
                    else
                    {
                        sitedb.Code.AddOrUpdate(oldcode, call.Context.User.Id);
                    }


                    code.EventType = oldcode.EventType;
                    code.CodeType = oldcode.CodeType;
                }
            }
            else
            {
                //Api add route.
                if (code.CodeType == Sites.Models.CodeType.Api)
                {
                    // add a new route. 
                    var route = new Kooboo.Sites.Routing.Route();
                    route.Name = model.Url;
                    route.objectId = code.Id;
                    route.DestinationConstType = ConstObjectType.Code;
                    sitedb.Routes.AddOrUpdate(route, call.Context.User.Id);

                }

                sitedb.Code.AddOrUpdate(code, call.Context.User.Id);
            }

            return code.Id;
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.EDIT)]
        public bool IsUniqueName(string name, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var id = Kooboo.Data.IDGenerator.Generate(name, ConstObjectType.Code);
            var code = sitedb.Code.Get(id);

            return code == null;
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.VIEW)]
        public Dictionary<string, string> CodeType(ApiCall call)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var list = Enum.GetNames(typeof(Kooboo.Sites.Models.CodeType));

            foreach (var item in list)
            {
                var value = Lib.Helper.EnumHelper.GetEnum<CodeType>(item);
                var isObsolete = Attribute.IsDefined(value.GetType().GetField(item), typeof(ObsoleteAttribute));
                if (isObsolete) continue;

                string name = item;
                name = Data.Language.Hardcoded.GetValue(name, call.Context);
                // if (value == Sites.Models.CodeType.PageScript)
                // {
                //     name = Data.Language.Hardcoded.GetValue("PageScript", call.Context);
                // }


                result.Add(item, name);
            }
            return result;
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.VIEW)]
        public virtual IEnumerable<CodeListItem> ListByType(string codetype, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            string baseurl = call.WebSite.BaseUrl();

            List<CodeListItem> result = new List<CodeListItem>();

            List<Code> codes = null;

            if (string.IsNullOrEmpty(codetype) || codetype.ToLower() == "all")
            {
                codes = sitedb.Code.All().ToList();
            }
            else
            {
                var enumcodetype = Lib.Helper.EnumHelper.GetEnum<CodeType>(codetype);

                codes = sitedb.Code.ListByCodeType(enumcodetype);
            }

            if (codes == null)
            {
                return new List<CodeListItem>();
            }

            var sortedCodes = SortCodes(codes, ApiHelper.GetIsDevMode(call));
            foreach (var item in sortedCodes)
            {
                CodeListItem model = new CodeListItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    KeyHash = Sites.Service.LogService.GetKeyHash(item.Id),
                    StoreNameHash = Lib.Security.Hash.ComputeInt(sitedb.Code.StoreName),
                    ScriptType = item.ScriptType.ToString(),
                    Version = item.Version
                };

                if (item.IsEmbedded)
                {
                    model.Name = StringHelper.GetSummary(item.Body);
                }

                model.CodeType = Enum.GetName(typeof(CodeType), item.CodeType);
                model.LastModified = item.LastModified;

                if (item.CodeType == Sites.Models.CodeType.Api)
                {
                    var route = sitedb.Routes.GetByObjectId(item.Id);
                    if (route != null)
                    {
                        model.Url = route.Name;
                        model.PreviewUrl = Lib.Helper.UrlHelper.Combine(baseurl, model.Url);
                    }
                }


                if (item.CodeType == Sites.Models.CodeType.PageScript)
                {
                    // calcuate the reference.. 
                    var usedby = sitedb.Code.GetUsedBy(item.Id);

                    model.References = Sites.Helper.RelationHelper.Sum(usedby);

                    model.IsEmbedded = item.IsEmbedded;
                }

                result.Add(model);

            }

            return result;
        }

        private static IEnumerable<Code> SortCodes(IEnumerable<Code> codes, bool isDevMode)
        {
            if (!isDevMode)
            {
                return codes.OrderByDescending(it => it.LastModified);
            }

            return codes
                .OrderBy(it => it.IsEmbedded)
                .ThenBy(it => it.Name?.Trim())
                .ThenByDescending(it => it.ScriptType.ToString())
                .ThenBy(it => it.Body?.Trim());
        }

        public bool HasScriptTag(string body)
        {
            if (body.IndexOf("<script", StringComparison.OrdinalIgnoreCase) > -1)
            {

                var stringValues = Lib.Utilities.JsStringScanner.ScanStringList(body);

                foreach (var item in stringValues)
                {
                    if (item != null && item.Length > 8)
                    {
                        body = body.Replace(item, "");
                    }
                }

                var dom = Kooboo.Dom.DomParser.CreateDom(body);

                var el = dom.getElementsByTagName("script");

                if (el != null && el.length > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual List<IEmbeddableItemListViewModel> EmbeddedScripts(ApiCall apiCall)
        {
            return apiCall.WebSite.SiteDb().Code.GetEmbeddeds().SortByNameOrLastModified(apiCall)
            .Select(o => new IEmbeddableItemListViewModel(apiCall.WebSite.SiteDb(), o)).ToList();
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            return base.List(call);
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.CODE, Action = Data.Permission.Action.EDIT)]
        public override bool IsUniqueName(ApiCall call)
        {
            return base.IsUniqueName(call);
        }
    }
}
