//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
//using Kooboo.Sites.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Kooboo.Sites.Repository;

//namespace Kooboo.Sites.Constraints.Implementation
//{
//    public class PageHeader : IConstraintChecker<Page>
//    {
//        public bool AutoFixOnSave
//        {
//            get
//            {
//                return true; 
//            }
//        }

//        public bool HasCheck
//        {
//            get
//            {
//                return false; 
//            }
//        }

//        public bool HasFix
//        {
//            get
//            {
//                return true; 
//            }
//        }

//        public List<ConstraintResponse> Check(SiteDb SiteDb, Page SiteObject, string Language = null)
//        {
//            throw new NotImplementedException();
//        }

//        public void Fix(SiteDb SiteDb, Page SiteObject, string Language = null)
//        {
//            if (!SiteObject.HasLayout && SiteObject.HasHeaderValue)
//            {
//                string onlyenablecuture = null; 
//                if (SiteDb.WebSite.EnableMultilingual && SiteDb.WebSite.Cultures.Count() ==1)
//                {
//                    onlyenablecuture = SiteDb.WebSite.Cultures.First(); 
//                }
//                if (!SiteDb.WebSite.EnableMultilingual)
//                {
//                    onlyenablecuture = SiteDb.WebSite.DefaultCulture; 
//                }

//                SiteObject.Body = Service.HtmlHeadService.SetHeader(SiteObject.Body, SiteObject.Headers, onlyenablecuture); 
//            }
//        }

//        public DisplayMetaInfo GetMeta()
//        {
//            return new DisplayMetaInfo()
//            {
//                Name = "Page Header Set",
//                Description = "Set the header settings to dom body if applicable",
//                ShortName = "PageHeaderSet"
//            };
//        }
//    }
//}
