//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Components
{
    public class FormComponent : IComponent
    {
        public FormComponent()
        {
        }

        public string TagName
        {
            get { return "Form"; }
        }

        public bool IsRegularHtmlTag { get { return true; } }

        public string StoreEngineName { get { return "kform"; } }

        public Dictionary<string, string> Setttings
        {
            get
            {
                return null;
            }
        }

        public Task<string> RenderAsync(RenderContext context, ComponentSetting settings)
        {
            if (settings == null || string.IsNullOrWhiteSpace(settings.NameOrId))
            {
                return null;
            }
            var sitedb = context.WebSite.SiteDb();

            var formid = Data.IDGenerator.GetOrGenerate(settings.NameOrId, ConstObjectType.Form);

            var form = sitedb.Forms.Get(formid);
            if (form == null) { return null; }

            var formsetting = sitedb.FormSetting.GetByFormId(form.Id);

            string body = form.Body;
            if (body.IndexOf("<form", StringComparison.OrdinalIgnoreCase) == -1)
            {
                body = "<form>" + body + "</form>";
            }

            if (formsetting == null || formsetting.Enable == false || string.IsNullOrWhiteSpace(formsetting.FormSubmitter))
            { return Task.FromResult(body); }

            var computeHash = Lib.Security.Hash.ComputeHashGuid(body);
            if (computeHash != this.FormHash)
            {
                this.FormHash = computeHash;
                // reset form...
                var el = GetFormElement(body);
                this.FormAttributes = new Dictionary<string, string>();

                foreach (var item in el.attributes)
                {
                    this.FormAttributes.Add(item.name, item.value);
                }
                this.BodyRenderTask = RenderEvaluator.Evaluate(el.InnerHtml, new EvaluatorOption());
            }

            string submiturl = Kooboo.Sites.HtmlForm.FormManager.GetSubmitUrl(form, formsetting, context);

            Dictionary<string, string> attributes = new Dictionary<string, string>(this.FormAttributes)
            {
                ["action"] = submiturl,
                ["method"] = string.IsNullOrEmpty(formsetting.Method) ? "post" : formsetting.Method
            };


            if (formsetting.AllowAjax)
            {
                attributes["onsubmit"] = "return false;";
            }

            string key = $"kform_{Lib.Security.ShortGuid.GetNewShortId()}";

            // append additional koobooform id.....
            if (form.FormType == Models.FormType.KoobooForm)
            {
                attributes["id"] = key;
            }

            string opentag = Kooboo.Sites.Service.DomService.GenerateOpenTag(attributes, "form");

            if (formsetting.IsSelfRefresh)
            {
                opentag = opentag + "<input type='hidden' name=\"" + Sites.HtmlForm.FormManager.FormUrlName + "\" value=\"" + context.Request.RawRelativeUrl + "\" />";
            }

            // If ajax, append the additional text here...

            //opentag += JsString(key, formsetting.SuccessCallBack, formsetting.FailedCallBack);

            string result = opentag + RenderHelper.Render(GetBodyTask(form.Body), context) + "</form>";

            result += JsString(key, formsetting.SuccessCallBack, formsetting.FailedCallBack);

            return Task.FromResult(result);
        }

        private string JsString(string key, string success, string fail)
        {
            string addtionalJS = $"<script src=\"/_admin/scripts/lib/jquery.min.js\"></script><script src=\"/_admin/scripts/lib/jqBootstrapValidation.js\"></script>";

            string ajax = "<script>$(\"#{key}\").find(\"input, textarea\").not(\"[type=submit]\").jqBootstrapValidation({preventSubmit: false,submitSuccess: function($form, event) {var data = {};$form.find('[name]').each(function(idx, el) {data[$(el).attr('name')] = $(el).val()});$.ajax({url: $form.attr('action'),method: $form.attr('method'),data: data,success: function(res) {{Success}}})},submitError: function($form, event, errors) {{Fail}}})</script>";

            ajax = ajax.Replace("{key}", key).Replace("{Success}", success).Replace("{Fail}", fail);

            return addtionalJS + ajax;
        }

        private Kooboo.Dom.Element GetFormElement(string formbody)
        {
            var doc = Kooboo.Dom.DomParser.CreateDom(formbody);

            var form = doc.getElementsByTagName("form");

            if (form != null && form.length > 0)
            {
                return form.item[0];
            }
            return null;
        }

        private Dictionary<string, string> FormAttributes { get; set; }
        private List<IRenderTask> BodyRenderTask { get; set; }
        private Guid _bodyhash;

        private Guid FormHash { get; set; }
        public byte StoreConstType { get { return ConstObjectType.Form; } }

        private List<IRenderTask> GetBodyTask(string body)
        {
            if (BodyRenderTask == null)
            {
                BodyRenderTask = RenderEvaluator.Evaluate(body, new EvaluatorOption());
                _bodyhash = Lib.Security.Hash.ComputeHashGuid(body);
            }
            else
            {
                var hash = Lib.Security.Hash.ComputeHashGuid(body);
                if (hash != _bodyhash)
                {
                    BodyRenderTask = RenderEvaluator.Evaluate(body, new EvaluatorOption());
                    _bodyhash = hash;
                }
            }
            return BodyRenderTask;
        }

        public List<ComponentInfo> AvaiableObjects(SiteDb sitedb)
        {
            List<ComponentInfo> models = new List<ComponentInfo>();
            var allobjs = sitedb.Forms.All().Where(o => !o.IsEmbedded).ToList();
            foreach (var item in allobjs)
            {
                ComponentInfo comp = new ComponentInfo {Id = item.Id, Name = item.Name};
                models.Add(comp);
            }
            return models;
        }

        public string Preview(SiteDb siteDb, string nameOrId)
        {
            if (string.IsNullOrEmpty(nameOrId))
            {
                return null;
            }
            var item = siteDb.Forms.GetByNameOrId(nameOrId);

            if (item != null && !string.IsNullOrEmpty(item.Body))
            {
                return FormInnerHtml(item.Body);
            }

            return null;
        }

        private string FormInnerHtml(string formbody)
        {
            if (formbody.IndexOf("<form", StringComparison.OrdinalIgnoreCase) > -1)
            {
                string newbody = "<html><body>" + formbody + "</body></html>";
                var dom = Kooboo.Dom.DomParser.CreateDom(newbody);
                if (dom == null || dom.forms.length == 0)
                {
                    return null;
                }
                else
                {
                    var form = dom.forms.item[0];
                    return form.InnerHtml;
                }
            }
            else
            {
                return formbody;
            }
        }

        public string DisplayName(RenderContext context)
        {
            return Kooboo.Data.Language.Hardcoded.GetValue("Form", context);
        }
    }
}