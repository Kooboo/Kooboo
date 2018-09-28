using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.InlineEditor.Model;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Kooboo.Sites.InlineEditor.Executor
{
  public  class ImageExecutor : IInlineExecutor
    {
        public string EditorType
        {
            get
            {
                return "image";
            }
        }

        public void Execute(RenderContext context, List<IInlineModel> inlineupdates)
        {
            // objeccttype = 'dom' or 'style'
            foreach (var item in inlineupdates.GroupBy(o => o.ObjectType))
            {
                var imageupdates = item.ToList().Cast<ImageModel>().ToList(); 
                if (item.Key == "style")
                { 
                    // not used any more. 
                   // UpdateStyle(context, imageupdates); 
                }
                else if (item.Key=="dom")
                {
                    UpdateDom(context, imageupdates); 
                } 
            }
        } 

        public void UpdateDom(RenderContext context, List<ImageModel> imageupdates)
        {
            var page = context.GetItem<Page>(); 
            var pageimageids = context.WebSite.SiteDb().Images.ListUsedByObjects(page.Id).Select(o => o.Id).ToList(); 
            var pageupdates = imageupdates.Where(o => pageimageids.Contains(o.ImageId)).ToList(); 
            UpdateDomObject(context, ConstObjectType.Page, page.Id, pageupdates); 
            
            var allobjects = context.WebSite.SiteDb().Pages.GetRelatedObject(page.Id, ConstObjectType.View, ConstObjectType.Layout);

            foreach (var item in allobjects)
            { 
                foreach (var objectids in item.Value)
                {
                    var objectimageids = context.WebSite.SiteDb().Images.ListUsedByObjects(objectids).Select(o => o.Id).ToList();
                    var objectupdates = imageupdates.Where(o => objectimageids.Contains(o.ImageId)).ToList();
                    UpdateDomObject(context,item.Key, objectids, objectupdates);
                }
            }
             
        }

        private void UpdateDomObject(RenderContext context, byte ConstType, Guid ObjectId,  List<ImageModel> updates)
        {
            if (updates == null || updates.Count() == 0)
            {
                return; 
            }
            var repo = context.WebSite.SiteDb().GetRepository(ConstType);
            if (repo != null)
            {   
                var siteobject = repo.Get(ObjectId); 
                if (siteobject != null)
                {
                    /// now we only handle dom object, it can be textcontent or htmlblock. 
                    var domobject = siteobject as DomObject;
                    List<InlineSourceUpdate> inlineupdates = new List<InlineSourceUpdate>(); 
                    foreach (var item in updates)
                    {
                        if (!string.IsNullOrEmpty(item.KoobooId))
                        { 
                            var element = Service.DomService.GetElementByKoobooId(domobject.Dom, item.KoobooId); 
                            if (verify(context, element as Kooboo.Dom.Element, item.ImageId))
                            {
                                InlineSourceUpdate sourceupdate = new InlineSourceUpdate();
                                sourceupdate.AttributeName = "src";
                                sourceupdate.KoobooId = item.KoobooId;
                                sourceupdate.Value = item.Value;
                                inlineupdates.Add(sourceupdate);
                            } 
                        }
                    }

                   domobject.Body = UpdateHelper.Update(domobject.Body, inlineupdates);

                    repo.AddOrUpdate(domobject, context.User.Id); 
                }
            } 
        }

        private bool verify(RenderContext context, Kooboo.Dom.Element element, Guid ImageId)
        {
            if (element == null || element.tagName != "img")
            {
                return false; 
            }
            string src = Service.DomUrlService.GetLinkOrSrc(element);
            var route = context.WebSite.SiteDb().Routes.GetByObjectId(ImageId); 

            if (route == null || string.IsNullOrEmpty(route.Name) || string.IsNullOrEmpty(src))
            {
                return false; 
            }

            if (route.Name.ToLower() != src.ToLower())
            {
                return false; 
            }

            return true;  
        }
         
        public void ExecuteObject(RenderContext context, IRepository repo, string NameOrId, List<IInlineModel> updates)
        {
            throw new NotImplementedException();
        }
    }
     
}
