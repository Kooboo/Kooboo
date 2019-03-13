//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using Kooboo.Extensions;
using Kooboo.Sites.SiteElements;
using Kooboo.Dom;

namespace Kooboo.Sites.Repository
{
   public class DomElementRepository : SiteRepositoryBase<DomElement>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters paras = new ObjectStoreParameters(); 
                paras.AddColumn<DomElement>(o => o.KoobooIdHash);
                paras.AddColumn<DomElement>(o => o.ParentPathHash);
                //paras.AddColumn<PageElement>(o => o.SubElementHash);
                paras.AddColumn<DomElement>(o => o.ParentId);
                paras.AddColumn<DomElement>(o => o.OwnerObjectId);
                paras.AddColumn<DomElement>(o => o.OwnerObjectType);
                paras.AddColumn<DomElement>(o => o.InnerHtmlHash);
                paras.AddColumn<DomElement>(o => o.Depth);
                paras.AddColumn<DomElement>(o => o.Sibling);
                paras.SetPrimaryKeyField<DomElement>(o => o.Id);             
                return paras; 
            }
        }
        
        public DomElement GetByKoobooId(Guid OwnerObjectId, byte OwnerConstType, string KoobooId)
        {
            /// this can only has one record, otherwise it is a an error. 
            Guid KoobooIdHash = KoobooId.ToHashGuid();
            return this.Query.Where(o => o.OwnerObjectId == OwnerObjectId && o.KoobooIdHash == KoobooIdHash && o.OwnerObjectType == OwnerConstType).FirstOrDefault();
        }

        public DomElement GetSamePageElement(DomElement pageelement, Guid DesitinationObjectId, byte ConstType)
        {
            var allcandicates = Query.Where(o => o.ParentPathHash == pageelement.ParentPathHash && o.InnerHtmlHash == pageelement.InnerHtmlHash && o.OwnerObjectId == DesitinationObjectId).SelectAll();

            var currentObjectSiblings = Query.Where(o => o.ParentId == pageelement.ParentId && o.OwnerObjectId == pageelement.OwnerObjectId).SelectAll();

            foreach (var item in allcandicates)
            {
                var ysibling = Query.Where(o => o.ParentId == item.ParentId && o.OwnerObjectId == item.OwnerObjectId).SelectAll();

                if (isSameSibling(pageelement, item, currentObjectSiblings, ysibling))
                {
                    return item;
                }

            }

            return null;
        }
        
        private  bool isSameSibling(DomElement elementX, DomElement elementY, List<DomElement> XSiblings, List<DomElement> YSiblings)
        {
            var xleft = XSiblings.Where(o => o.Sibling < elementX.Sibling).ToList();
            var xright = XSiblings.Where(o => o.Sibling > elementX.Sibling).ToList();

            var yleft = YSiblings.Where(o => o.Sibling < elementY.Sibling).ToList();

            var yright = YSiblings.Where(o => o.Sibling > elementY.Sibling).ToList();

            return (isSameList(xleft, yleft) || isSameList(xright, yright));
        }
        
        /// <summary>
        /// clean current object and all its subs. 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="PageElementId"></param>
        public   void CleanSub(Guid PageElementId, List<DomElement> AllOwnerElements)
        {
           this.Delete(PageElementId);

            var subs = AllOwnerElements.Where(o => o.ParentId == PageElementId); 

            if (subs !=null && subs.Count()>0)
            {
                foreach (var item in subs)
                {
                    CleanSub(item.Id, AllOwnerElements);
                }
            } 
        }

        public  void CleanObject( Guid OwnerObjectId, byte ConstType)
        {
            var allitems = Query.Where(o => o.OwnerObjectId == OwnerObjectId && o.OwnerObjectType == ConstType).SelectAll();
            foreach (var item in allitems)
            {
                Delete(item.Id);
            }
        }

        public  List<DomElement> ListSub(Guid ParentId)
        {
            return Query.Where(o => o.ParentId == ParentId).SelectAll();
        }

        private  bool isSamePageElement(DomElement x, DomElement y)
        {
            if (x.ParentPathHash != y.ParentPathHash)
            {
                return false;
            }

            if (x.Name != y.Name)
            {
                return false;
            }

            if (x.InnerHtmlHash != y.InnerHtmlHash)
            {
                return false;
            }

            if (x.Depth != y.Depth)
            {
                return false;
            }

            if (x.NodeAttributeHash != y.NodeAttributeHash)
            {
                return false;
            }

            return true;
        }

        private  bool isSameList(List<DomElement> listx, List<DomElement> listy)
        {
            if (listx.Count != listy.Count)
            {
                return false;
            }

            int count = listx.Count();

            for (int i = 0; i < count; i++)
            {
                if (!isSamePageElement(listx[i], listy[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public  override bool AddOrUpdate(DomElement element, Guid UserId = default(Guid))
        {
            var old = Get(element.Id);
            if (old == null)
            {
               this.Store.add(element.Id, element);
                return true; 
            }
            else
            {
                if (!isSamePageElement(old, element))
                {
                    this.Store.update(element.Id, element);
                    return true; 
                }
            }
            return false; 
        }

        public  void AddOrUpdateDom(Document Dom, Guid OwnerObjectId, byte OwnerConstType, bool NewThread = true)
        {
            if (Dom == null)
            { return;  }

            var OwnerElements = this.Query.Where(o => o.OwnerObjectId == OwnerObjectId).SelectAll(); 

            //if (NewThread)
            //{
            //    System.Threading.Tasks.Task.Run(() => { _AddOrUpdateElement(Dom.body, default(Guid), OwnerObjectId, OwnerConstType); });
            //}
            //else
            //{
                _AddOrUpdateElement(Dom.body, default(Guid), OwnerObjectId, OwnerConstType,   OwnerElements);
            //}
        }
        
        private  DomElement _AddOrUpdateElement(Element element, Guid ParentPageElementId, Guid OwnerObjectId, byte OwnerConstType,   List<DomElement> AllOwnerElements)
        {
            List<Guid> newsubs = new List<Guid>();
            var pagenode = PageElementManager.ConvertToPageElement(element);
            pagenode.OwnerObjectType = OwnerConstType;
            pagenode.OwnerObjectId = OwnerObjectId;
            pagenode.ParentId = ParentPageElementId;
            AddOrUpdate(pagenode);

            var currentsubs = AllOwnerElements.Where(o => o.ParentId == pagenode.Id);  

            foreach (var item in element.childNodes.item)
            {
                if (item is Element)
                {
                    Element e = item as Element;
                    var back = _AddOrUpdateElement(e, pagenode.Id, OwnerObjectId, OwnerConstType,   AllOwnerElements);
                    newsubs.Add(back.Id);
                }
            }

            //clean old not used any more pageelement.
            foreach (var item in currentsubs)
            {
                if (!newsubs.Contains(item.Id))
                {
                    CleanSub(item.Id, AllOwnerElements);
                }
            }
            return pagenode;
        }

        /// <summary>
        /// Suggest layout for two objects...
        /// </summary>
        /// <param name="store"></param>
        /// <param name="ObjectX"></param>
        /// <param name="ObjectY"></param>
        /// <param name="ConstType"></param>
        /// <returns></returns>
        public  List<DomElement> SuggestLayout(Guid ObjectX, Guid ObjectY, byte ConstType)
        {
            var allXelements = Query.Where(o => o.OwnerObjectId == ObjectX && o.OwnerObjectType == ConstType).SelectAll();

            var allYelements = Query.Where(o => o.OwnerObjectId == ObjectY && o.OwnerObjectType == ConstType).SelectAll();

            return LayoutElements.FindPlaceHolders(allXelements, allYelements);
            //  return FindPlaceHolders(allXelements, allYelements);
        }


        /// <summary>
        /// find the common part as placeholders. 
        /// </summary>
        /// <param name="allXelements"></param>
        /// <param name="allYelements"></param>
        /// <returns></returns>
        private  List<DomElement> FindPlaceHolders(List<DomElement> allXelements, List<DomElement> allYelements)
        {
            List<DomElement> placeholders = new List<DomElement>();

            foreach (var item in allXelements)
            {
                if (!Kooboo.Sites.Tag.AllowedTags.IsLayoutTag(item.Name))
                {
                    continue;
                }

                var xSiblings = allXelements.Where(o => o.ParentId == item.ParentId).ToList();

                var foundsame = FindSameElement(item, xSiblings, allYelements);

                if (foundsame != null)
                {
                    continue;
                }

                var allycandidates = allYelements.Where(o => o.ParentPathHash == item.ParentPathHash && o.Depth == item.Depth && o.Name == item.Name && o.NodeAttributeHash == item.NodeAttributeHash && o.SubElementHash != item.SubElementHash).ToList();

                foreach (var yitem in allycandidates)
                {
                    var ysiblings = allYelements.Where(o => o.ParentId == yitem.ParentId).ToList();

                    if (isSameSibling(item, yitem, xSiblings, ysiblings))
                    {
                        if (LayoutCleaner.CheckIsAllowedForLayout(item, allXelements))
                        {
                            placeholders.Add(item);
                            break;
                        }
                    }
                }
            }

            return placeholders;
        }


        private  DomElement FindSameElement(DomElement element, List<DomElement> currentElementSiblings, List<DomElement> allcandidates)
        {
            var candicates = allcandidates.Where(o => o.ParentPathHash == element.ParentPathHash && o.Depth == element.Depth && o.Name == element.Name && o.NodeAttributeHash == element.NodeAttributeHash && o.InnerHtmlHash == element.InnerHtmlHash);

            foreach (var sameitem in candicates)
            {
                var ysiblings = allcandidates.Where(o => o.ParentId == sameitem.ParentId).ToList();

                if (isSameSibling(element, sameitem, currentElementSiblings, ysiblings))
                {
                    return sameitem;
                }
            }

            return null;

        }


        /// <summary>
        /// Layout can only extract out of page. 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="ObjectIds"></param>
        /// <param name="ConstType"></param>
        /// <returns></returns>
        public  List<DomElement> SuggestLayout(List<Guid> ObjectIds, byte ConstType)
        {
            List<DomElement> layouts = new List<DomElement>();

            List<List<DomElement>> sitePages =  getAllPageElements(ObjectIds, ConstType);

            List<DomElement> unionset = GetSamePageElements(sitePages);

            /// step 2, check to make sure that the element indeed can be used as layout. 
            List<Guid> PageElementIdToRemove = new List<Guid>();

            foreach (var item in unionset)
            {
                var firstleft = sitePages[0].Where(o => o.ParentId == item.ParentId && o.Sibling < item.Sibling).ToList();
                var firstright = sitePages[0].Where(o => o.ParentId == item.ParentId && o.Sibling > item.Sibling).ToList();
            }
            return null;
        }

        public  List<List<DomElement>> getAllPageElements(List<Guid> ObjectIds, byte ConstType)
        {
            List<List<DomElement>> sitePages = new List<List<DomElement>>();

            foreach (var item in ObjectIds)
            {
                var allitems = Query.Where(o => o.OwnerObjectId == item && o.OwnerObjectType == ConstType).SelectAll();
                if (allitems.Count > 0)
                {
                    sitePages.Add(allitems);
                }
            }
            return sitePages;
        }

        /// <summary>
        /// Check whether this selected page elements can be used as layout or not. 
        /// Layout means the subitem as something different....
        /// </summary>
        /// <param name="element"></param>
        /// <param name="sitePages"></param>
        /// <returns></returns>
        public  bool TestAsLayout(DomElement element, List<List<DomElement>> sitePages)
        {
            if (sitePages.Count == 1) { return true; }

            /// test to see they all the same left or right sibling. 
            bool rightok = true;
            bool leftok = true;

            List<DomElement> pageone = sitePages[0];

            var currentleft = pageone.Where(o => o.ParentPathHash == element.ParentPathHash && o.Sibling < element.Sibling).ToList();
            var currentright = pageone.Where(o => o.ParentPathHash == element.ParentPathHash && o.Sibling > element.Sibling).ToList();

            int count = sitePages.Count;

            for (int i = 0; i < count - 1; i++)
            {
                var nextpage = sitePages[i + 1];

                var newleft = nextpage.Where(o => o.ParentPathHash == element.ParentPathHash && o.Sibling < element.Sibling).ToList();
                var newright = nextpage.Where(o => o.ParentPathHash == element.ParentPathHash && o.Sibling > element.Sibling).ToList();

                if (leftok)
                {
                    if (!isSameList(currentleft, newleft))
                    {
                        leftok = false;
                    }
                }

                if (!leftok && !rightok)
                {
                    return false;
                }

            }

            return true;


        }
         
        /// <summary>
        /// return all the page elements that has similar parent and with different subs. 
        /// </summary>
        /// <param name="sitePages"></param>
        /// <returns></returns>
        public static List<DomElement> GetSamePageElements(List<List<DomElement>> sitePages)
        {
            //step 1, union those layout elements. 
            List<DomElement> unionset = new List<DomElement>();
            unionset = sitePages[0];

            int counter = sitePages.Count;

            unionset = sitePages[0];

            for (int i = 0; i < counter - 1; i++)
            {
                List<DomElement> nextelements = sitePages[i + 1];
                unionset = unionset.Where(o => ContainsAsSameElementIn(o, nextelements)).ToList();
            }

            return unionset;
        }

        private static bool ContainsAsSameElementIn(DomElement element, List<DomElement> targetPage)
        {
            var find = targetPage.Find(o => o.ParentPathHash == element.ParentPathHash && o.Depth == element.Depth && o.Name == element.Name && o.NodeAttributeHash == element.NodeAttributeHash);

            if (find != null)
            {
                return true;
            }
            return false;
        }
    }
}
