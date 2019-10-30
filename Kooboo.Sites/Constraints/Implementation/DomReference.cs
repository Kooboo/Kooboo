//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using System.Collections.Generic;

namespace Kooboo.Sites.Constraints
{
    public class DomReference : IConstraintChecker<DomObject>
    {
        public bool HasFix
        {
            get
            {
                return true;
            }
        }

        public bool HasCheck
        { get { return true; } }

        public bool AutoFixOnSave
        {
            get
            {
                return true;
            }
        }

        private List<SourceUpdate> GetChanges(SiteDb siteDb, DomObject domObject, string language = null)
        {
            List<SourceUpdate> updates = new List<SourceUpdate>();
            if (string.IsNullOrEmpty(domObject.Body))
            {
                return updates;
            }

            string objectRelativeUrl = GetObjectUrl(siteDb, domObject);

            objectRelativeUrl = objectRelativeUrl.Replace("\\", "/");

            if (!objectRelativeUrl.StartsWith("/"))
            {
                objectRelativeUrl = "/" + objectRelativeUrl;
            }

            var dom = domObject.Dom;

            foreach (var item in dom.Links.item)
            {
                string itemsrc = Service.DomUrlService.GetLinkOrSrc(item);

                if (!string.IsNullOrEmpty(itemsrc))
                {
                    CheckAndAddChange(updates, item, itemsrc, objectRelativeUrl);
                }
            }

            var imgurls = Kooboo.Sites.Service.DomUrlService.GetImageSrcs(dom);

            foreach (var item in imgurls)
            {
                if (!string.IsNullOrEmpty(item.Value) && !Kooboo.Lib.Utilities.DataUriService.isDataUri(item.Value))
                {
                    CheckAndAddChange(updates, item.Key, item.Value, objectRelativeUrl);
                }
            }

            HTMLCollection scripts = dom.getElementsByTagName("script");

            foreach (var item in scripts.item)
            {
                if (item.hasAttribute("src"))
                {
                    string srcurl = Service.DomUrlService.GetLinkOrSrc(item);

                    if (!string.IsNullOrEmpty(srcurl))
                    {
                        CheckAndAddChange(updates, item, srcurl, objectRelativeUrl);
                    }
                }
            }

            HTMLCollection embedElement = dom.getElementsByTagName("embed");

            foreach (var item in embedElement.item)
            {
                string fileurl = Kooboo.Sites.Service.DomUrlService.GetLinkOrSrc(item);

                if (!string.IsNullOrEmpty(fileurl))
                {
                    CheckAndAddChange(updates, item, fileurl, objectRelativeUrl);
                }
            }

            HTMLCollection styletags = dom.getElementsByTagName("link");

            foreach (var item in styletags.item)
            {
                if (item.hasAttribute("rel") && item.getAttribute("rel").ToLower().Contains("stylesheet"))
                {
                    string itemurl = Kooboo.Sites.Service.DomUrlService.GetLinkOrSrc(item);

                    if (!string.IsNullOrEmpty(itemurl))
                    {
                        CheckAndAddChange(updates, item, itemurl, objectRelativeUrl);
                    }
                }
            }

            return updates;
        }

        private void CheckAndAddChange(List<SourceUpdate> updates, Dom.Element item, string itemsrc, string objectRelativeUrl)
        {
            if (string.IsNullOrEmpty(itemsrc))
            {
                return;
            }

            if (DomUrlService.IsExternalLink(itemsrc))
            {
                return;
            }

            string relativeUrl = Kooboo.Lib.Helper.UrlHelper.Combine(objectRelativeUrl, itemsrc);

            if (relativeUrl != null)
            {
                relativeUrl = System.Net.WebUtility.UrlDecode(relativeUrl);
            }

            if (itemsrc != relativeUrl)
            {
                string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                string newstring = oldstring.Replace(itemsrc, relativeUrl);

                updates.Add(new SourceUpdate()
                {
                    StartIndex = item.location.openTokenStartIndex,
                    EndIndex = item.location.openTokenEndIndex,
                    NewValue = newstring
                });
            }
        }

        private string GetObjectUrl(SiteDb siteDb, DomObject domObject)
        {
            string url;
            if (Attributes.AttributeHelper.IsRoutable(domObject))
            {
                url = siteDb.Routes.GetObjectPrimaryRelativeUrl(domObject.Id);
            }
            else
            {
                if (string.IsNullOrEmpty(domObject.Name))
                {
                    url = "/";
                }
                else
                {
                    url = "/" + domObject.Name;
                }
            }
            if (string.IsNullOrEmpty(url))
            {
                url = "/";
            }
            return url;
        }

        public void Fix(SiteDb siteDb, DomObject domobject, string language = null)
        {
            var changes = GetChanges(siteDb, domobject);

            if (changes.Count > 0)
            {
                domobject.Body = Kooboo.Sites.Service.DomService.UpdateSource(domobject.Body, changes);
            }
        }

        public List<ConstraintResponse> Check(SiteDb siteDb, DomObject domobject, string language = null)
        {
            List<ConstraintResponse> responseresult = new List<ConstraintResponse>();

            var changes = GetChanges(siteDb, domobject);

            if (changes.Count > 0)
            {
                foreach (var item in changes)
                {
                    string oldvalue = domobject.Body.Substring(item.StartIndex, item.EndIndex - item.StartIndex + 1);

                    ConstraintResponse response = new ConstraintResponse {AffectedPart = oldvalue};
                    var linecol = domobject.Body.GetPosition(item.StartIndex);
                    response.LineNumber = linecol.Line;
                    response.ColumnNumber = linecol.Column;

                    response.ContraintName = this.GetMeta().Name;

                    response.Message = "Should be: " + item.NewValue;

                    responseresult.Add(response);
                }
            }

            return responseresult;
        }

        public DisplayMetaInfo GetMeta()
        {
            DisplayMetaInfo meta = new DisplayMetaInfo
            {
                Name = "DomReference",
                Description = "Check all the image, style, script, links, all the resources must start with / root."
            };
            return meta;
        }
    }
}