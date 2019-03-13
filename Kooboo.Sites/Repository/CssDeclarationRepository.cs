//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
//using Kooboo.Extensions;
//using Kooboo.IndexedDB;
//using Kooboo.Sites.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
 
//namespace Kooboo.Sites.Repository
//{
//    public class CssDeclarationRepository : SiteRepositoryBase<CmsCssDeclaration>
//    {
//        public override ObjectStoreParameters StoreParameters
//        {
//            get
//            {
//                ObjectStoreParameters paras = new ObjectStoreParameters();
//                paras.AddColumn<CmsCssDeclaration>(o => o.PropertyNameHash);
//                paras.AddColumn<CmsCssDeclaration>(o => o.CmsCssRuleId);
//                paras.AddColumn<CmsCssDeclaration>(o => o.Id);
//                paras.AddColumn<CmsCssDeclaration>(o => o.OwnerObjectId);
//                paras.AddColumn<CmsCssDeclaration>(o => o.OwnerObjectConstType);
//                paras.AddColumn<CmsCssDeclaration>(o => o.ParentStyleId);
//                paras.SetPrimaryKeyField<CmsCssDeclaration>(o => o.Id); 
//                return paras;
//            }
//        }

//        public List<CmsCssDeclaration> GetByRuleId(Guid CssRuleId)
//        {
//            return Query.Where(o => o.CmsCssRuleId == CssRuleId).SelectAll();
//        }
       
//    }
//}



//public class DeclarationUpdate
//{

//    public Guid DeclarationId { get; set; }

//    public Guid CmsCssRuleId { get; set; }

//    public string PropertyName { get; set; }

//    public string Value { get; set; }

//    private bool _IsDelete;
//    public bool IsDelete
//    {
//        get
//        {
//            if (_IsDelete)
//            {
//                return true;
//            }

//            if (string.IsNullOrWhiteSpace(Value))
//            {
//                return true;
//            }
//            return false;
//        }
//        set
//        {
//            _IsDelete = value;
//        }

//    }

//    public bool Important { get; set; }

//}
