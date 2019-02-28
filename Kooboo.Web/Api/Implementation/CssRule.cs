//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;

namespace Kooboo.Web.Api.Implementation
{
    public class CssRule : SiteObjectApi<CmsCssRule>
    {
        public override string ModelName
        {
            get
            {
                return "CssRule";
            }
            set
            {

            }
        }
         
        public List<InlineItemViewModel> InlineList(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb(); 

            List<InlineItemViewModel> result = new List<InlineItemViewModel>();

            foreach (var item in sitedb.CssRules.GetInLineRules())
            {
                var info = ObjectService.GetObjectInfo(sitedb, item);

                InlineItemViewModel newitem = new InlineItemViewModel();
                newitem.Id = item.Id;
                newitem.Name = string.IsNullOrEmpty(item.KoobooOpenTag) ? item.RuleText : item.KoobooOpenTag;
                newitem.OwnerName = info.DisplayName;
                newitem.OwnerType = info.ModelType.Name;
                newitem.LastModified = item.LastModified;
                newitem.Source = item.RuleText;
                result.Add(newitem);
            }

            return result;
        }

        public List<RelationItemViewModel> Relation(ApiCall call)
        {
            CmsCssRule cssrule = call.WebSite.SiteDb().CssRules.Get(call.ObjectId);

            List<RelationItemViewModel> relations = new List<RelationItemViewModel>();

            var usedby = call.WebSite.SiteDb().CssRules.ShowRelations(cssrule);

            foreach (var item in usedby)
            {
                relations.Add(new RelationItemViewModel()
                {
                    DisplayName = item.Name,
                    ElementName = item.Remark,
                    Url = item.Url
                });
            }

            return relations;
        }

        [Kooboo.Attributes.RequireParameters("id")]
        public InlineStyleViewModel GetInline(ApiCall call)
        {
            var item = call.WebSite.SiteDb().CssRules.Get(call.ObjectId);
            if (item == null)
            {
                return null;
            }
            InlineStyleViewModel model = new InlineStyleViewModel();
            model.Id = call.ObjectId;

            model.Declarations = CssService.ParseDeclarationBlock(item.CssText, call.ObjectId);

            return model;
        } 
        
        [Kooboo.Attributes.RequireParameters("id", "RuleText")]
        public void UpdateInline(ApiCall call)
        {
            string ruletext = call.GetValue("RuleText"); 
            call.WebSite.SiteDb().CssRules.UpdateInlineCss(call.ObjectId, ruletext);  
        }

      
    }
}
