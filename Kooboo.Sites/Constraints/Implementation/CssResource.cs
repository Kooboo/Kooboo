//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System.Collections.Generic;

namespace Kooboo.Sites.Constraints
{
    public class CssUrl : IConstraintChecker<Style>
    {
        public bool AutoFixOnSave
        {
            get
            {
                return true;
            }
        }

        public bool HasFix
        {
            get
            {
                return true;
            }
        }

        public bool HasCheck
        {
            get
            { return true; }
        }

        public DisplayMetaInfo GetMeta()
        {
            return new DisplayMetaInfo()
            {
                Name = "CssResource",
                Description = "Fix the resource like image, font reference in css file to start with / "
            };
        }

        public List<ConstraintResponse> Check(SiteDb siteDb, Style siteObject, string language = null)
        {
            List<ConstraintResponse> responseresult = new List<ConstraintResponse>();

            if (siteObject.Extension != null && (siteObject.Extension.ToLower() == "css" || siteObject.Extension.ToLower() == ".css"))
            {
                var changes = GetChanges(siteObject.Body, language);

                if (changes.Count > 0)
                {
                    foreach (var item in changes)
                    {
                        string oldvalue = siteObject.Body.Substring(item.StartIndex, item.EndIndex - item.StartIndex);

                        ConstraintResponse response = new ConstraintResponse {AffectedPart = oldvalue};
                        var linecol = siteObject.Body.GetPosition(item.StartIndex);
                        response.LineNumber = linecol.Line;
                        response.ColumnNumber = linecol.Column;

                        response.ContraintName = this.GetMeta().Name;

                        response.Message = "Should be: " + item.NewValue;

                        responseresult.Add(response);
                    }
                }
            }
            return responseresult;
        }

        public void Fix(SiteDb siteDb, Style siteObject, string language = null)
        {
            if (!siteObject.IsEmbedded)
            {
                if (siteObject.Extension != null && (siteObject.Extension.ToLower() == "css" || siteObject.Extension.ToLower() == ".css"))
                {
                    string relativecssrule = Service.ObjectService.GetObjectRelativeUrl(siteDb, siteObject);
                    if (string.IsNullOrEmpty(relativecssrule))
                    {
                        relativecssrule = "/";
                    }
                    var changes = GetChanges(siteObject.Body, relativecssrule);

                    if (changes.Count > 0)
                    {
                        siteObject.Body = Kooboo.Sites.Service.DomService.UpdateSource(siteObject.Body, changes);
                    }
                }
            }
        }

        private List<SourceUpdate> GetChanges(string cssText, string cssFileRelativeUrl)
        {
            List<SourceUpdate> updates = new List<SourceUpdate>();

            if (string.IsNullOrEmpty(cssText))
            {
                return updates;
            }
            var urlInfos = Service.CssService.GetUrlInfos(cssText);

            foreach (var item in urlInfos)
            {
                if (!Kooboo.Lib.Utilities.DataUriService.isDataUri(item.PureUrl))
                {
                    string righturl = Kooboo.Lib.Helper.UrlHelper.ReplaceBackSlash(item.PureUrl);
                    string newRelativeUrl = Kooboo.Lib.Helper.UrlHelper.Combine(cssFileRelativeUrl, righturl);

                    if (item.PureUrl != newRelativeUrl)
                    {
                        string newvalue;
                        if (item.isUrlToken)
                        {
                            newvalue = "url(\"" + newRelativeUrl + "\")";
                        }
                        else
                        {
                            newvalue = "\"" + newRelativeUrl + "\"";
                        }

                        updates.Add(new SourceUpdate { StartIndex = item.StartIndex, EndIndex = item.EndIndex, NewValue = newvalue });
                    }
                }
            }

            return updates;
        }
    }
}