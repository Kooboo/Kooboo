//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Render
{
    public class FormRenderTask : IRenderTask
    {
        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public Guid OwnerObjectId { get; set; }

        public string KoobooId { get; set; }

        private Dictionary<string, string> _formattribute;

        public Dictionary<string, string> FormAttributes
        {
            get
            {
                if (_formattribute == null)
                {
                    _formattribute = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return _formattribute;
            }
            set
            {
                _formattribute = value;
            }
        }

        public string OriginalInnerHtml { get; set; }

        public EvaluatorOption options { get; set; }

        private Guid FormId { get; set; }

        public FormRenderTask(Element element, Guid OwnerObjectId, Guid FormId, EvaluatorOption evalutionOption)
        {
            this.options = options;

            foreach (var item in element.attributes)
            {
                this.FormAttributes.Add(item.name, item.value);
            }
            this.OwnerObjectId = OwnerObjectId;

            this.OriginalInnerHtml = element.InnerHtml;

            this.FormId = FormId;

            InitOrgTask();
        }

        private void InitOrgTask()
        {
            if (this.BodyHash == default(Guid) || this.BodyHash != this.OrginalHash)
            {
                this.BodyTask = GetBodyTask(this.OriginalInnerHtml, this.options);
                this.BodyHash = Lib.Security.Hash.ComputeHashGuid(this.OriginalInnerHtml);
                this.OrginalHash = Lib.Security.Hash.ComputeHashGuid(this.OriginalInnerHtml);
            }
        }

        private void InitFormBodyTask(string formbody)
        {
            var hash = Lib.Security.Hash.ComputeHashGuid(formbody);
            if (this.BodyHash != hash)
            {
                this.BodyTask = GetBodyTask(formbody, this.options);
                this.BodyHash = hash;
            }
        }

        private List<IRenderTask> BodyTask { get; set; }
        private Guid BodyHash { get; set; }
        private Guid OrginalHash { get; set; }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }

        public string Render(RenderContext context)
        {
            if (this.OwnerObjectId == default(Guid))
            {
                // this should not happen... just in case... 
                InitOrgTask();
                string open = Kooboo.Sites.Service.DomService.GenerateOpenTag(this.FormAttributes, "form");
                return open + RenderHelper.Render(this.BodyTask, context) + "</form>";
            }

            var sitedb = context.WebSite.SiteDb();

            var form = sitedb.Forms.Get(this.FormId);

            FormSetting formsetting = null;
            if (form != null)
            {
                formsetting = sitedb.FormSetting.GetByFormId(form.Id);
            }

            if (form == null || formsetting == null || formsetting.Enable == false || string.IsNullOrEmpty(formsetting.FormSubmitter))
            {
                InitOrgTask();
                string open = Kooboo.Sites.Service.DomService.GenerateOpenTag(this.FormAttributes, "form");
                return open + RenderHelper.Render(this.BodyTask, context) + "</form>";
            }
            // or else result 
                                
            string submiturl = Kooboo.Sites.HtmlForm.FormManager.GetSubmitUrl(form, formsetting, context);  




            Dictionary<string, string> attributes = new Dictionary<string, string>(FormAttributes);

            attributes["action"] = submiturl;
            attributes["method"] = string.IsNullOrEmpty(formsetting.Method) ? "post" : formsetting.Method;

            string key = "kform_" + Lib.Security.ShortGuid.GetNewShortId();

            /// append additional koobooform id.....
            //if (form.FormType == FormType.KoobooForm || formsetting.AllowAjax)
            //{
            attributes["id"] = key;
            //}

            string opentag = Kooboo.Sites.Service.DomService.GenerateOpenTag(attributes, "form");

            if (formsetting.IsSelfRefresh)
            {
                opentag = opentag + "<input type='hidden' name=\"" + Sites.HtmlForm.FormManager.FormUrlName + "\" value=\"" + context.Request.RawRelativeUrl + "\" />";
            }


            //if (formsetting.AllowAjax)
            //{
            string addtionalJS = "<script src=\"/_admin/scripts/lib/jquery.min.js\"></script><script src=\"/_admin/scripts/lib/jqBootstrapValidation.js\"></script>";

            opentag += addtionalJS;

            opentag += JsString(key, formsetting.SuccessCallBack, formsetting.FailedCallBack);
            // }


            if (!string.IsNullOrEmpty(form.Body))
            {
                InitFormBodyTask(form.Body);
                return opentag + RenderHelper.Render(this.BodyTask, context) + "</form>";
            }
            else
            {
                InitOrgTask();
                return opentag + RenderHelper.Render(this.BodyTask, context) + "</form>";
            }

        }

        private string JsString(string key, string success, string fail)
        {

            string ajax = "<script>$(\"#{key}\").find(\"input, textarea\").not(\"[type=submit]\").jqBootstrapValidation({preventSubmit: false,submitSuccess: function($form, event) {var data = {};$form.find('[name]').each(function(idx, el) {data[$(el).attr('name')] = $(el).val()});$.ajax({url: $form.attr('action'),method: $form.attr('method'),data: data,success: function(res) {{Success}}})},submitError: function($form, event, errors) {{Fail}}})</script>";

            return ajax.Replace("{key}", key).Replace("{Success}", success).Replace("{Fail}", fail);
        }


        private List<IRenderTask> GetBodyTask(string body, EvaluatorOption option)
        {
            return RenderEvaluator.Evaluate(body, option);
        }
    }
}
