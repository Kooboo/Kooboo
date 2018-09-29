//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Repository;
using Kooboo.Extensions;

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

        public List<ConstraintResponse> Check(SiteDb SiteDb, Style SiteObject, string Language = null)
        {
            List<ConstraintResponse> responseresult = new List<ConstraintResponse>();

            if (SiteObject.Extension != null && (SiteObject.Extension.ToLower() == "css" || SiteObject.Extension.ToLower() == ".css"))
            {
                var changes = GetChanges(SiteObject.Body, Language);

                if (changes.Count > 0)
                {
                    foreach (var item in changes)
                    {
                        string oldvalue = SiteObject.Body.Substring(item.StartIndex, item.EndIndex - item.StartIndex);

                        ConstraintResponse response = new ConstraintResponse();
                        response.AffectedPart = oldvalue;
                        var linecol = SiteObject.Body.GetPosition(item.StartIndex);
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

        public void Fix(SiteDb SiteDb, Style SiteObject, string Language = null)
        {
            if (!SiteObject.IsEmbedded)
            {
                if (SiteObject.Extension != null && (SiteObject.Extension.ToLower() == "css" || SiteObject.Extension.ToLower() == ".css"))
                {

                    string relativecssrule = Service.ObjectService.GetObjectRelativeUrl(SiteDb, SiteObject);
                    if (string.IsNullOrEmpty(relativecssrule))
                    {
                        relativecssrule = "/";
                    }
                    var changes = GetChanges(SiteObject.Body, relativecssrule);

                    if (changes.Count > 0)
                    {
                        SiteObject.Body = Kooboo.Sites.Service.DomService.UpdateSource(SiteObject.Body, changes);
                    }
                }
            }
        }

        private List<SourceUpdate> GetChanges(string cssText, string CssFileRelativeUrl)
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
                    string NewRelativeUrl = Kooboo.Lib.Helper.UrlHelper.Combine(CssFileRelativeUrl, righturl);


                    if (item.PureUrl != NewRelativeUrl)
                    {
                        string newvalue;
                        if (item.isUrlToken)
                        {
                            newvalue = "url(\"" + NewRelativeUrl + "\")";
                        }
                        else
                        {
                            newvalue = "\"" + NewRelativeUrl + "\"";
                        }

                        updates.Add(new SourceUpdate { StartIndex = item.StartIndex, EndIndex = item.EndIndex, NewValue = newvalue });
                    }

                }
            }

            return updates;
        }

    }
}
