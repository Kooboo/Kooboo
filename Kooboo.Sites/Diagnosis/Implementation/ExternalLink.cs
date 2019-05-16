//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Diagnosis.Implementation
{
    public class ExternalLink : IDiagnosis
    {
        public DiagnosisSession session { get; set; }

        public string Group(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Normal", context);
        }

        public string Name(RenderContext context)
        {
            return Hardcoded.GetValue("Check availability of external links", context);
        }

        public void Check()
        {
            string checking = Hardcoded.GetValue("Checking", this.session.context);
            string name = Hardcoded.GetValue("external links", this.session.context); 
            session.Headline = checking + " " + name;

            var sitedb = this.session.context.WebSite.SiteDb();

            string errorheader = Hardcoded.GetValue("Invalid link", session.context);

            var allresource = sitedb.ExternalResource.All();

            var tasks = new List<Task>();

            foreach (var resource in allresource)
            {
                if (ShouldCheck(resource.FullUrl))
                {
                    tasks.Add(Task.Run(() => DoCheck(session.context, resource)));
                }
            }

            Task.WaitAll(tasks.ToArray()); 
        }


        private void DoCheck(RenderContext context, ExternalResource resource)
        {
            var fullUrl = resource.FullUrl;

            string header = Hardcoded.GetValue("Missing link", context);
            var message = String.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", fullUrl);

            try
            {
                HttpWebResponse response = DownloadHelper.RequestHeader(new Uri(fullUrl));

                if (!IsEnableStatusCode(response.StatusCode))
                {
                    session.AddMessage(header, message, MessageType.Critical);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    HttpWebResponse response = DownloadHelper.RequestGet(fullUrl);
                    if (!IsEnableStatusCode(response.StatusCode))
                    {
                        session.AddMessage(header, message, MessageType.Critical);
                    }

                }
                catch (Exception)
                {
                    session.AddMessage(header, message, MessageType.Critical);
                }
            }
        }
         

        private bool IsEnableStatusCode(HttpStatusCode statusCode)
        {
            string statusStr = ((int)statusCode).ToString();
            return statusStr.StartsWith("2") || statusStr.StartsWith("3");
        }

        private bool ShouldCheck(string fullurl)
        {
            if (string.IsNullOrEmpty(fullurl))
            {
                return false;
            }
            fullurl = fullurl.ToLower().Trim();
            if (fullurl.StartsWith("mailto:"))
            {
                return false;
            }
            if (fullurl.StartsWith("javascript:"))
            {
                return false; 
            }
            if (fullurl.StartsWith("#"))
            {
                return false;
            }

            if (fullurl.StartsWith("http://") || fullurl.StartsWith("https://"))
            {
                return true;
            }

            return false;
        }

    }
}
