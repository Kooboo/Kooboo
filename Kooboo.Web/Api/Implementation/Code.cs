//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Linq;
using Kooboo.Web.ViewModel;
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using System.Collections.Generic;
using Kooboo.Lib.Helper;

namespace Kooboo.Web.Api.Implementation
{
    public class CodeApi : SiteObjectApi<Code>
    {
        public virtual CodeEditViewModel GetEdit(string codetype, ApiCall call)
        {
            call.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            call.Context.Response.Headers.Add("Access-Control-Allow-Headers", "*");

            var sitedb = call.WebSite.SiteDb();
            
            if (call.ObjectId != default(Guid))
            {
                var code = sitedb.Code.Get(call.ObjectId);
                CodeEditViewModel model = new CodeEditViewModel();

                if (code != null)
                {
                    model.Name = code.Name;
                    model.Body = code.Body;
                    model.Config = code.Config;
                    model.EventType = Enum.GetName(typeof(Kooboo.Sites.FrontEvent.enumEventType), code.EventType);
                    model.CodeType = Enum.GetName(typeof(Kooboo.Sites.Models.CodeType), code.CodeType);

                    model.Id = code.Id;
                    if (code.CodeType == Sites.Models.CodeType.Api)
                    {
                        var route = sitedb.Routes.GetByObjectId(model.Id);
                        if (route != null)
                        {
                            model.Url = route.Name;
                        }
                    }
                }


                return model;
            }
            else
            {
                CodeEditViewModel model = new CodeEditViewModel();

                model.Config = configSample();

                if (codetype != null && codetype.ToLower() != "all")
                {
                    var enumcodetype = Lib.Helper.EnumHelper.GetEnum<CodeType>(codetype);
                    model.Body = getSample(enumcodetype);
                }

                model.AvailableEventType = Enum.GetNames(typeof(Kooboo.Sites.FrontEvent.enumEventType)).ToList();
                model.AvailableCodeType = Enum.GetNames(typeof(Kooboo.Sites.Models.CodeType)).ToList();
                return model;
            }
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
            else if (codetype == Sites.Models.CodeType.Datasource)
            {
                sample = @"//sample code, use the k.export to return datasource.
//var list = []; 
//var obj = {name: ""myname"", fieldtwo: ""field two value""};
//list.push(obj);  
//var obj2 = {name: ""myname2"", fieldtwo: ""field two value2""};
//list.push(obj2); 
//k.export(list); ";

            }
            else if (codetype == Sites.Models.CodeType.Diagnosis)
            {
                sample = @"// sample code.
//var allpages = k.siteDb.pages.all();   
//var page = allpages[0]; 
//if (page.body.length> 200)
//{
//    var error = ""Page too long: "" + page.name;  
//    k.diagnosis.error(error);
//}";

            }
            else if (codetype == Sites.Models.CodeType.Event)
            {
                sample = @"//common event varilables: k.event.url, k.event.userAgent, k.event.culture; 
//variables per event. k.event.page, k.event.view, k.event.route; 
// Finding=before object found. Found = object founded. 
//example, url redirect. only valid on RouteFinding event. 
//if (k.event.url.indexOf(""pagetwo"")>-1)
//{
//     k.event.url = ""/pageone"";
//}"; 
            }
            else if (codetype == Sites.Models.CodeType.PageScript)
            {
                sample = @"// kscript that can be inserted to page position. 
//k.cookie.set(""key"", ""value"");
//k.response.write(""Hello world"");"; 
            }
            else  if (codetype == Sites.Models.CodeType.Job)
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


            return sample;
        }

        public virtual Guid Post(CodeEditViewModel model, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            Code code = new Code();
            code.Name = model.Name;
            code.Config = model.Config;
            code.Body = model.Body;

            if (HasScriptTag(code.Body))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("You do not need script tag in code. Only in the page, view or layout, you need the script tag"));
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
                    if (route !=null && route.objectId != default(Guid))
                    {
                        var siteobjecttype = Kooboo.ConstTypeContainer.GetModelType(route.DestinationConstType); 
                        if (siteobjecttype !=null)
                        {
                            var repo = sitedb.GetSiteRepositoryByModelType(siteobjecttype); 
                            if (repo !=null)
                            {
                                var obj = repo.Get(route.objectId); 
                                if (obj !=null)
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
                    oldcode.Name = model.Name;
                    oldcode.Body = model.Body;
                    oldcode.Config = model.Config;
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
                sitedb.Code.AddOrUpdate(code);
            }

            if (code.CodeType == Sites.Models.CodeType.Api)
            {
                string url = model.Url;
                if (!string.IsNullOrEmpty(url))
                {
                    var route = new Kooboo.Sites.Routing.Route();
                    route.Name = url;
                    route.objectId = code.Id;
                    route.DestinationConstType = ConstObjectType.Code;
                    sitedb.Routes.AddOrUpdate(route);
                }
                else
                {
                    // delete the route. 
                    var route = sitedb.Routes.GetByObjectId(code.Id);
                    if (route != null)
                    {
                        sitedb.Routes.Delete(route.Id);
                    }
                }
            }

            return code.Id;
        }

        public bool IsUniqueName(string name, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var code = sitedb.Code.Get(name);

            return code == null;
        }

        public Dictionary<string, string> CodeType(ApiCall call)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
             
            var list = Enum.GetNames(typeof(Kooboo.Sites.Models.CodeType));

            foreach (var item in list)
            {
                var value = Lib.Helper.EnumHelper.GetEnum<CodeType>(item);
                string name = item;
                if (value == Sites.Models.CodeType.Diagnosis)
                {
                    name = Data.Language.Hardcoded.GetValue("Diagnosis", call.Context);
                }
                else if (value == Sites.Models.CodeType.Event)
                {
                    name = Data.Language.Hardcoded.GetValue("Event", call.Context);
                }
                else if (value == Sites.Models.CodeType.Job)
                {
                    name = Data.Language.Hardcoded.GetValue("Job", call.Context);
                }
                else if (value == Sites.Models.CodeType.PageScript)
                {
                    name = Data.Language.Hardcoded.GetValue("PageScript", call.Context); 
                }
                else if (value == Sites.Models.CodeType.Datasource)
                {
                    name = Data.Language.Hardcoded.GetValue("DataSource", call.Context); 
                }

                
                result.Add(item, name);
            }
            return result;
        }

        public virtual List<CodeListItem> ListByType(string codetype, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
            string baseurl = call.WebSite.BaseUrl();

            List<CodeListItem> result = new List<CodeListItem>();

            List<Code> codes = null;

            if (string.IsNullOrEmpty(codetype) || codetype.ToLower() == "all")
            {
                codes = sitedb.Code.All().OrderBy(o => o.Name).ToList();
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

            foreach (var item in codes)
            {
                CodeListItem model = new CodeListItem();
                model.Id = item.Id;
                model.Name = item.Name;       

                if (item.IsEmbedded)
                {
                    model.Name = StringHelper.GetSummary(item.Body);
                }

                model.CodeType = Enum.GetName(typeof(CodeType), item.CodeType);

                if (item.CodeType == Sites.Models.CodeType.Event)
                {
                    model.EventType = Enum.GetName(typeof(Kooboo.Sites.FrontEvent.enumEventType), item.EventType);
                }
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
                                                           
        public bool HasScriptTag(string body)
        {
            if (body.IndexOf("<script", StringComparison.OrdinalIgnoreCase) > -1)
            {
                var dom = Kooboo.Dom.DomParser.CreateDom(body);

                var el = dom.getElementsByTagName("script");

                if (el != null)
                {
                    return true;
                }
            }
            return false;
        }
              
        public virtual List<IEmbeddableItemListViewModel> EmbeddedScripts(ApiCall apiCall)
        {
            return apiCall.WebSite.SiteDb().Code.GetEmbeddeds()
            .Select(o => new IEmbeddableItemListViewModel(apiCall.WebSite.SiteDb(), o)).ToList();
        }            

    }
}
