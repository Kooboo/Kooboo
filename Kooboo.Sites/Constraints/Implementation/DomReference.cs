//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using System;
using System.Collections.Generic; 
using System.Text;
using System.Threading.Tasks;
using Kooboo.Extensions; 

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

        private List<SourceUpdate> GetChanges(SiteDb SiteDb, DomObject DomObject, string Language = null)
        {
            List<SourceUpdate> updates = new List<SourceUpdate>();
            if (string.IsNullOrEmpty(DomObject.Body))
            {
                return updates;
            } 
            
            string   ObjectRelativeUrl = GetObjectUrl(SiteDb, DomObject);
          

            ObjectRelativeUrl = ObjectRelativeUrl.Replace("\\", "/");

            if (!ObjectRelativeUrl.StartsWith("/"))
            {
                ObjectRelativeUrl = "/" + ObjectRelativeUrl;
            }

            var dom = DomObject.Dom;

            foreach (var item in dom.Links.item)
            {
                string itemsrc = Service.DomUrlService.GetLinkOrSrc(item);

                if (!string.IsNullOrEmpty(itemsrc))
                {
                    CheckAndAddChange(updates, item, itemsrc, ObjectRelativeUrl);
                }
            }

            foreach (var item in dom.images.item)
            {
                string itemsrc = DomUrlService.GetLinkOrSrc(item);

                if (!string.IsNullOrEmpty(itemsrc) && !Kooboo.Lib.Utilities.DataUriService.isDataUri(itemsrc))
                {

                    CheckAndAddChange(updates, item, itemsrc, ObjectRelativeUrl);

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
                        CheckAndAddChange(updates, item, srcurl, ObjectRelativeUrl);
                    }
                }
            }

            HTMLCollection embedElement = dom.getElementsByTagName("embed");

            foreach (var item in embedElement.item)
            {
                string fileurl = Kooboo.Sites.Service.DomUrlService.GetLinkOrSrc(item);

                if (!string.IsNullOrEmpty(fileurl))
                {
                    CheckAndAddChange(updates, item, fileurl, ObjectRelativeUrl);
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
                        CheckAndAddChange(updates, item, itemurl, ObjectRelativeUrl);
                    }

                }
            }

            return updates;
        }

        private void CheckAndAddChange(List<SourceUpdate> updates, Dom.Element item, string itemsrc, string ObjectRelativeUrl)
        {
            if (string.IsNullOrEmpty(itemsrc))
            {
                return; 
            }

            if (DomUrlService.IsExternalLink(itemsrc))
            {
                return; 
            }

            string RelativeUrl = Kooboo.Lib.Helper.UrlHelper.Combine(ObjectRelativeUrl, itemsrc);

            if (RelativeUrl !=null)
            {
                RelativeUrl = System.Net.WebUtility.UrlDecode(RelativeUrl); 
            }

            if (itemsrc != RelativeUrl)
            {
                string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                string newstring = oldstring.Replace(itemsrc, RelativeUrl);

                updates.Add(new SourceUpdate()
                {
                    StartIndex = item.location.openTokenStartIndex,
                    EndIndex = item.location.openTokenEndIndex,
                    NewValue = newstring

                });
            }
        }

        private string GetObjectUrl(SiteDb SiteDb, DomObject DomObject)
        {
            string url; 
            if (Attributes.AttributeHelper.IsRoutable(DomObject))
            {
                url=  SiteDb.Routes.GetObjectPrimaryRelativeUrl(DomObject.Id);
            }
            else
            {
                if (string.IsNullOrEmpty(DomObject.Name))
                {
                    url =  "/";
                }
                else
                {
                    url =  "/" + DomObject.Name;
                }
            }
            if (string.IsNullOrEmpty(url))
            {
                url = "/";
            }
            return url;
        }

        public void Fix(SiteDb SiteDb, DomObject domobject, string Language = null)
        {
            var changes = GetChanges(SiteDb, domobject);

            if (changes.Count > 0)
            {
                domobject.Body = Kooboo.Sites.Service.DomService.UpdateSource(domobject.Body, changes);
            }
        }

        public List<ConstraintResponse> Check(SiteDb SiteDb, DomObject domobject, string Language = null)
        {
            List<ConstraintResponse> responseresult = new List<ConstraintResponse>();

            var changes = GetChanges(SiteDb, domobject);

            if (changes.Count > 0)
            {
                foreach (var item in changes)
                {
                    string oldvalue = domobject.Body.Substring(item.StartIndex, item.EndIndex - item.StartIndex +1);

                    ConstraintResponse response = new ConstraintResponse();
                    response.AffectedPart = oldvalue;
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
            DisplayMetaInfo meta = new DisplayMetaInfo();
            meta.Name = "DomReference";
            meta.Description = "Check all the image, style, script, links, all the resources must start with / root.";
            return meta;
        }
    }
}
