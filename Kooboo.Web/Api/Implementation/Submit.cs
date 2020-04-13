//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Extensions;
using Kooboo.Api.ApiResponse;
using System;
using Kooboo.Api;

namespace Kooboo.Web.Api.Implementation
{
    public class Submit : IApi
    {
        public string ModelName
        {
            get
            {
                return "Submit";
            }
        }

        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;
            }
        }

        public IResponse Form(ApiCall call)
        {
            // in the format of /_api/submit/form/xxxyyssss; 
            var sitedb = call.WebSite.SiteDb(); 
            Guid FormId = Lib.Security.ShortGuid.Decode(call.Command.Value);

            var formsetting = sitedb.FormSetting.GetByFormId(FormId);

            if (formsetting != null && !string.IsNullOrEmpty(formsetting.FormSubmitter))
            {
                var submitter = Sites.HtmlForm.FormManager.GetSubmitter(formsetting.FormSubmitter);
                submitter.Submit(call.Context, formsetting.FormId, formsetting.Setting);

                if (call.Context.Response.End || call.Context.Response.StatusCode >= 300)
                {
                    return new NoResponse(); 
                }

                MetaResponse response = new MetaResponse();
                response.Success = true;

                if (!formsetting.AllowAjax && !string.IsNullOrEmpty(formsetting.RedirectUrl))
                {
                    string url = formsetting.RedirectUrl;
                    if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                    {
                        if (url.ToLower() == Kooboo.Sites.SiteConstants.SelfRefreshUrl)
                        {
                            string formurlvalue = call.GetValue(Sites.HtmlForm.FormManager.FormUrlName);
                            if (!string.IsNullOrEmpty(formurlvalue))
                            {
                                url = formurlvalue;
                                if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                                {
                                    string baseurl = call.Context.WebSite.BaseUrl();
                                    url = Lib.Helper.UrlHelper.Combine(baseurl, url);
                                }
                            }
                            else
                            {
                                var form = sitedb.Forms.Get(FormId); 
                                url = Kooboo.Sites.Service.ObjectService.GetObjectFullUrl(call.Context.WebSite, form.OwnerObjectId);
                            }
                        }
                        else
                        {
                            string baseurl = call.Context.WebSite.BaseUrl();
                            url = Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, url);
                        }
                        response.Redirect(url);
                        return response;
                    }
                    else
                    {
                        response.Redirect(url);
                        return response;
                    }

                }

                return response;
            }

            return null;

        }
    }
}