//using Kooboo.Data.Models;
//using Kooboo.IndexedDB;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Collections; 
//using System.Text;
//using System.Threading.Tasks;
//using Kooboo.Data.Extensions;
//using Kooboo.Dom;
//using Kooboo.Extensions;
//using Kooboo.Sites.Models;
//using Kooboo.Sites.SiteElements; 

//namespace Kooboo.Sites.Extensions
//{
//    public static class PageElementExtension
//    {
//        public static ObjectStore<Guid, DomElement> PageElements(this Database db)
//        {
//            string storename = StoreNameConvention.PageElement;

//            if (db.hasObjectStore(storename))
//            {
//                return db.getObjectStore<Guid, DomElement>(storename);
//            }
//            else
//            {
//                ObjectStoreParameters paras = new ObjectStoreParameters();
//                paras.EnableLog = false;
//                paras.EnableVersion = false;
//                paras.AddColumn<DomElement>(o => o.KoobooIdHash);
//                paras.AddColumn<DomElement>(o => o.ParentPathHash);
//                //paras.AddColumn<PageElement>(o => o.SubElementHash);
//                paras.AddColumn<DomElement>(o => o.ParentId);
//                paras.AddColumn<DomElement>(o => o.OwnerObjectId);
//                paras.AddColumn<DomElement>(o => o.OwnerObjectType);
//                paras.AddColumn<DomElement>(o => o.InnerHtmlHash);
//                paras.AddColumn<DomElement>(o => o.Depth);
//                paras.AddColumn<DomElement>(o => o.Sibling); 

//                return db.createObjectStore<Guid, DomElement>(storename, paras);
//            }
//        }

//        public static ObjectStore<Guid, DomElement> PageElements(this WebSite website)
//        {
//            return website.CurrentDB().PageElements();
//        }

//        public static DomElement GetByKoobooId(this ObjectStore<Guid, DomElement> store, Guid OwnerObjectId, byte OwnerConstType, string KoobooId)
//        {
//            /// this can only has one record, otherwise it is a an error. 
//            Guid KoobooIdHash = KoobooId.ToHashGuid();
//            return store.Where(o => o.OwnerObjectId == OwnerObjectId && o.KoobooIdHash == KoobooIdHash && o.OwnerObjectType == OwnerConstType).FirstOrDefault();
//        }

//        public static DomElement GetSamePageElement(this ObjectStore<Guid, DomElement> store, DomElement pageelement, Guid DesitinationObjectId, byte ConstType)
//        {
//            var allcandicates =  store.Where(o => o.ParentPathHash == pageelement.ParentPathHash && o.InnerHtmlHash == pageelement.InnerHtmlHash && o.OwnerObjectId == DesitinationObjectId).SelectAll();

//            var currentObjectSiblings = store.Where(o => o.ParentId == pageelement.ParentId && o.OwnerObjectId== pageelement.OwnerObjectId).SelectAll();

//            foreach (var item in allcandicates)
//            {
//                var ysibling = store.Where(o=>o.ParentId == item.ParentId && o.OwnerObjectId == item.OwnerObjectId).SelectAll();

//                if (isSameSibling(pageelement, item, currentObjectSiblings, ysibling))
//                {
//                    return item; 
//                }

//            }

//            return null; 
//        }

//        /// <summary>
//        /// get the same pageelement from allcandidates. 
//        /// </summary>
//        /// <param name="pageelement"></param>
//        /// <param name="allsiblings"></param>
//        /// <param name="allcandicates"></param>
//        /// <returns></returns>
//        //private static PageElement getSameElement(PageElement pageelement, List<PageElement> allsiblings,  List<PageElement> allcandicates)
//        //{
//        //    var currentleft = allsiblings.Where(o => o.Sibling < pageelement.Sibling).ToList();
//        //    var currentright = allsiblings.Where(o => o.Sibling > pageelement.Sibling).ToList();

//        //    foreach (var item in allcandicates)
//        //    {
//        //        var newObjectSibling = allcandicates.Where(o => o.ParentId == item.ParentId).ToList(); 

//        //        var newleft = newObjectSibling.Where(o => o.Sibling < item.Sibling).ToList();
//        //        var newright = newObjectSibling.Where(o => o.Sibling > item.Sibling).ToList();

//        //        if (isSameList(currentleft, newleft) || isSameList(currentright, newright))
//        //        {
//        //            return item;
//        //        }
//        //    }

//        //    return null;
//        //}

//        private static bool isSameSibling(DomElement elementX, DomElement elementY, List<DomElement> XSiblings, List<DomElement> YSiblings)
//        {
//            var xleft = XSiblings.Where(o => o.Sibling < elementX.Sibling).ToList();
//            var xright = XSiblings.Where(o => o.Sibling > elementX.Sibling).ToList();

//            var yleft = YSiblings.Where(o => o.Sibling < elementY.Sibling).ToList();

//            var yright = YSiblings.Where(o => o.Sibling > elementY.Sibling).ToList();

//            return (isSameList(xleft, yleft) || isSameList(xright, yright));
//        }

//        public static DomElement Get(this ObjectStore<Guid, DomElement> store, Guid PageElementId)
//        {
//            return store.get(PageElementId);
//        }

//        /// <summary>
//        /// clean current object and all its subs. 
//        /// </summary>
//        /// <param name="store"></param>
//        /// <param name="PageElementId"></param>
//        public static void Clean(this ObjectStore<Guid, DomElement> store, Guid PageElementId)
//        {
//            store.delete(PageElementId);

//            foreach (var item in store.ListSub(PageElementId))
//            {
//                store.Clean(item.Id);
//            }
//        }

//        public static void CleanObject(this ObjectStore<Guid, DomElement> store, Guid OwnerObjectId, byte ConstType)
//        {
//            var allitems = store.Where(o => o.OwnerObjectId == OwnerObjectId && o.OwnerObjectType == ConstType).SelectAll();
//            foreach (var item in allitems)
//            {
//                store.delete(item.Id);
//            }
//        }

//        public static List<DomElement> ListSub(this ObjectStore<Guid, DomElement> store, Guid ParentId)
//        {
//            return store.Where(o => o.ParentId == ParentId).SelectAll();
//        }

//        private static bool isSamePageElement(DomElement x, DomElement y)
//        {
//            if (x.ParentPathHash != y.ParentPathHash)
//            {
//                return false;
//            }

//            if (x.Name != y.Name)
//            {
//                return false;
//            }

//            if (x.InnerHtmlHash != y.InnerHtmlHash)
//            {
//                return false;
//            }

//            if (x.Depth != y.Depth)
//            {
//                return false;
//            }

//            if (x.NodeAttributeHash!= y.NodeAttributeHash)
//            {
//                return false;
//            }

//            return true;
//        }

//        private static bool isSameList(List<DomElement> listx, List<DomElement> listy)
//        {
//            if (listx.Count != listy.Count)
//            {
//                return false; 
//            }

//            int count = listx.Count();

//            for (int i = 0; i < count; i++)
//            {
//                if (!isSamePageElement(listx[i], listy[i]))
//                {
//                    return false; 
//                }
//            }

//            return true; 
//        }

//        public static void AddOrUpdate(this ObjectStore<Guid, DomElement> store, DomElement element)
//        {

//            var old = store.Get(element.Id);
//            if (old == null)
//            {
//                store.add(element.Id, element);
//            }
//            else
//            {
//                if (!isSamePageElement(old, element))
//                {
//                    store.update(element.Id, element);
//                }
//            }

//        }

//        public static void AddOrUpdateDom(this ObjectStore<Guid, DomElement> store, Document Dom, Guid OwnerObjectId, byte OwnerConstType)
//        {
//            store._AddOrUpdateElement(Dom.body, default(Guid), OwnerObjectId, OwnerConstType);
//        }

//        private static DomElement _AddOrUpdateElement(this ObjectStore<Guid, DomElement> store, Element element, Guid ParentPageElementId, Guid OwnerObjectId, byte OwnerConstType)
//        {
//            List<Guid> newsubs = new List<Guid>();
//            var pagenode = PageElementManager.ConvertToPageElement(element);
//            pagenode.OwnerObjectType = OwnerConstType;
//            pagenode.OwnerObjectId = OwnerObjectId;
//            pagenode.ParentId = ParentPageElementId;
//            store.AddOrUpdate(pagenode);

//            var currentsubs = store.ListSub(pagenode.Id);

//            foreach (var item in element.childNodes.item)
//            {
//                if (item is Element)
//                {
//                    Element e = item as Element;
//                     var back = store._AddOrUpdateElement(e, pagenode.Id, OwnerObjectId, OwnerConstType);
//                    newsubs.Add(back.Id);
//                }
//            }

//            // clean old not used any more pageelement. 

//            foreach (var item in currentsubs)
//            {
//                if (!newsubs.Contains(item.Id))
//                {
//                    store.Clean(item.Id);
//                }
//            }


//            return pagenode;
//        }

//        /// <summary>
//        /// Suggest layout for two objects...
//        /// </summary>
//        /// <param name="store"></param>
//        /// <param name="ObjectX"></param>
//        /// <param name="ObjectY"></param>
//        /// <param name="ConstType"></param>
//        /// <returns></returns>
//        public static List<DomElement> SuggestLayout(this ObjectStore<Guid, DomElement> store, Guid ObjectX, Guid ObjectY, byte ConstType)
//        {
//            var allXelements = store.Where(o => o.OwnerObjectId == ObjectX && o.OwnerObjectType == ConstType).SelectAll();

//            var allYelements = store.Where(o => o.OwnerObjectId == ObjectY && o.OwnerObjectType == ConstType).SelectAll();

//            return LayoutElements.FindPlaceHolders(allXelements, allYelements); 

//          //  return FindPlaceHolders(allXelements, allYelements);
//        }


//        /// <summary>
//        /// find the common part as placeholders. 
//        /// </summary>
//        /// <param name="allXelements"></param>
//        /// <param name="allYelements"></param>
//        /// <returns></returns>
//        private static List<DomElement> FindPlaceHolders(List<DomElement> allXelements, List<DomElement> allYelements)
//        {
//            List<DomElement> placeholders = new List<DomElement>();

//            foreach (var item in allXelements)
//            {
//                if (!Kooboo.Sites.Tag.AllowedTags.IsLayoutTag(item.Name))
//                {
//                    continue;
//                }

//                var xSiblings = allXelements.Where(o => o.ParentId == item.ParentId).ToList();

//                var foundsame = FindSameElement(item, xSiblings, allYelements);

//                if (foundsame != null)
//                {
//                    continue;   
//                }

//                var allycandidates = allYelements.Where(o => o.ParentPathHash == item.ParentPathHash && o.Depth == item.Depth && o.Name ==  item.Name && o.NodeAttributeHash == item.NodeAttributeHash && o.SubElementHash != item.SubElementHash).ToList();

//                foreach (var yitem in allycandidates)
//                {
//                    var ysiblings = allYelements.Where(o => o.ParentId == yitem.ParentId).ToList();

//                    if (isSameSibling(item, yitem, xSiblings, ysiblings))
//                    {
//                        if (LayoutCleaner.CheckIsAllowedForLayout(item, allXelements))
//                        {
//                            placeholders.Add(item);
//                            break;
//                        }
//                    }
//                }
//            }

//            return  placeholders;
//        }


//        private static DomElement FindSameElement(DomElement element, List<DomElement> currentElementSiblings, List<DomElement> allcandidates)
//        {

//            var candicates = allcandidates.Where(o => o.ParentPathHash == element.ParentPathHash && o.Depth == element.Depth && o.Name == element.Name && o.NodeAttributeHash == element.NodeAttributeHash && o.InnerHtmlHash == element.InnerHtmlHash);
            
//            foreach (var sameitem in candicates)
//            {
//                var ysiblings = allcandidates.Where(o => o.ParentId == sameitem.ParentId).ToList();

//                if (isSameSibling(element, sameitem, currentElementSiblings, ysiblings))
//                {
//                    return sameitem; 
//                }
//            }

//            return null; 

//        }


//        /// <summary>
//        /// Layout can only extract out of page. 
//        /// </summary>
//        /// <param name="store"></param>
//        /// <param name="ObjectIds"></param>
//        /// <param name="ConstType"></param>
//        /// <returns></returns>
//        public static List<DomElement> SuggestLayout(this ObjectStore<Guid, DomElement> store, List<Guid> ObjectIds, byte ConstType)
//        {
//            List<DomElement> layouts = new List<DomElement>();

//            List<List<DomElement>> sitePages = getAllPageElements(store, ObjectIds, ConstType);

//            List<DomElement> unionset = GetSamePageElements(sitePages);

//            /// step 2, check to make sure that the element indeed can be used as layout. 
//            List<Guid> PageElementIdToRemove = new List<Guid>();

//            foreach (var item in unionset)
//            {
//                var firstleft = sitePages[0].Where(o => o.ParentId == item.ParentId && o.Sibling < item.Sibling).ToList();
//                var firstright = sitePages[0].Where(o => o.ParentId == item.ParentId && o.Sibling > item.Sibling).ToList();
//            }                       
//            return null;
//        }

//        public static List<List<DomElement>> getAllPageElements(ObjectStore<Guid, DomElement> store, List<Guid> ObjectIds, byte ConstType)
//        {
//            List<List<DomElement>> sitePages = new List<List<DomElement>>();

//            foreach (var item in ObjectIds)
//            {
//                var allitems = store.Where(o => o.OwnerObjectId == item && o.OwnerObjectType == ConstType).SelectAll();
//                if (allitems.Count > 0)
//                {
//                    sitePages.Add(allitems);
//                }
//            }
//            return sitePages;
//        }

//        /// <summary>
//        /// Check whether this selected page elements can be used as layout or not. 
//        /// Layout means the subitem as something different....
//        /// </summary>
//        /// <param name="element"></param>
//        /// <param name="sitePages"></param>
//        /// <returns></returns>
//        public static bool TestAsLayout(DomElement element, List<List<DomElement>> sitePages)
//        {
//            if (sitePages.Count ==1)    { return true;  }

//            /// test to see they all the same left or right sibling. 
//            bool rightok = true;
//            bool leftok = true;

//            List<DomElement> pageone = sitePages[0];

//            var currentleft = pageone.Where(o => o.ParentPathHash==element.ParentPathHash &&  o.Sibling < element.Sibling).ToList();
//            var currentright = pageone.Where(o => o.ParentPathHash == element.ParentPathHash && o.Sibling > element.Sibling).ToList();

//            int count = sitePages.Count;

//            for (int i = 0; i < count-1; i++)
//            {
//                var nextpage = sitePages[i + 1];

//                var newleft = nextpage.Where(o  =>o.ParentPathHash== element.ParentPathHash  &&  o.Sibling < element.Sibling).ToList();
//                var newright = nextpage.Where(o => o.ParentPathHash == element.ParentPathHash && o.Sibling > element.Sibling).ToList();

//                if (leftok)
//                {
//                    if (!isSameList(currentleft, newleft))
//                    {
//                        leftok = false;
//                    }
//                }

//                if (!leftok && !rightok)
//                {
//                    return false; 
//                }

//            }

//            return true; 


//        }


//        /// <summary>
//        /// return all the page elements that has similar parent and with different subs. 
//        /// </summary>
//        /// <param name="sitePages"></param>
//        /// <returns></returns>
//        public static List<DomElement> GetSamePageElements(List<List<DomElement>> sitePages)
//        {
//            //step 1, union those layout elements. 
//            List<DomElement> unionset = new List<DomElement>();
//            unionset = sitePages[0];

//            int counter = sitePages.Count;

//            unionset = sitePages[0];

//            for (int i = 0; i < counter - 1; i++)
//            {
//                List<DomElement> nextelements = sitePages[i + 1];
//                unionset = unionset.Where(o => ContainsAsSameElementIn(o, nextelements)).ToList();   
//            }

//            return unionset; 
//        }

//        private static bool ContainsAsSameElementIn(DomElement element, List<DomElement> targetPage)
//        {

//            var find = targetPage.Find(o => o.ParentPathHash == element.ParentPathHash && o.Depth == element.Depth && o.Name == element.Name && o.NodeAttributeHash == element.NodeAttributeHash);

//            if (find != null)
//            {
//                return true; 
//            }
//            return false; 

//        }




//    }
//}
