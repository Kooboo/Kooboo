using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Sites.InlineEditor.Model;
using Kooboo.Sites.Extensions;
using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Dom;
using System;

namespace Kooboo.Sites.InlineEditor.Executor
{
    public class DomExecutor : IInlineExecutor
    {
        public string EditorType
        {
            get
            {
                return "dom";
            }
        }

        public void Execute(RenderContext context, List<IInlineModel> inlineupdates)
        {
            foreach (var item in inlineupdates.GroupBy(o => o.ObjectType))
            {
                var repo = context.WebSite.SiteDb().GetRepository(item.Key);
                if (repo != null)
                {
                    var objecttypelist = item.ToList();
                    foreach (var objectitems in objecttypelist.GroupBy(o => o.NameOrId))
                    {
                        string nameorid = objectitems.Key;
                        ExecuteObject(context, repo, objectitems.Key, objectitems.ToList());
                    }
                }
            }
        }

        public void ExecuteObject(RenderContext context, IRepository repo, string NameOrId, List<IInlineModel> inlineupdates)
        {
            var updates = inlineupdates.Cast<DomModel>();
            var koobooobject = repo.GetByNameOrId(NameOrId);

            if (koobooobject != null)
            {
                List<InlineSourceUpdate> sourceupdates = new List<InlineSourceUpdate>();

                foreach (var item in updates)
                {
                    if (!string.IsNullOrEmpty(item.KoobooId))
                    {
                        InlineSourceUpdate oneupdate = new InlineSourceUpdate();
                        oneupdate.KoobooId = item.KoobooId;
                        oneupdate.AttributeName = item.AttributeName;
                        oneupdate.Value = item.Value;

                        if (item.Action !=null && item.Action.ToString().ToLower() == "delete")
                        {
                            oneupdate.IsDelete = true; 
                        }
                        sourceupdates.Add(oneupdate);
                    }
                }

                if (koobooobject is IDomObject && sourceupdates.Count() > 0)
                {
                    var domobject = koobooobject as IDomObject;

                    Page savepage = null;
                    if (repo.ModelType == typeof(Page))
                    {
                        savepage = repo.Store.get(domobject.Id) as Page;
                    }

                    domobject.Body = UpdateHelper.Update(domobject.Body, sourceupdates);
                    repo.AddOrUpdate(domobject, context.User.Id);
                    if (savepage != null)
                    {
                        var updateothers = new UpdateSamePage();
                        updateothers.sitedb = context.WebSite.SiteDb();
                        updateothers.updates = sourceupdates;
                        updateothers.CurrentPage = savepage;
                        updateothers.UserId = context.User.Id; 
                        System.Threading.Tasks.Task.Factory.StartNew(updateothers.Execute);
                    }
                }
            }
        }  
    }

    public class UpdateSamePage
    {
        public SiteDb sitedb { get; set; }
        public List<InlineSourceUpdate> updates { get; set; }
        public Page CurrentPage { get; set; }

        public Guid UserId { get; set; } = default(Guid); 

        public void Execute()
        {
            var allotherpages = sitedb.Pages.All().Where(o => o.Id != CurrentPage.Id && o.HasLayout == false).ToList();

            foreach (var item in allotherpages)
            {
                List<InlineSourceUpdate> itemupdates = new List<InlineSourceUpdate>();

                foreach (var update in updates)
                {
                    var currentel = Kooboo.Sites.Service.DomService.GetElementByKoobooId(CurrentPage.Dom, update.KoobooId) as Element;

                    if (currentel != null)
                    {
                        var sameel = Helper.ElementHelper.FindSameElement(currentel, item.Dom);
                        if (sameel != null)
                        {
                            string koobooid = Service.DomService.GetKoobooId(sameel);
                            itemupdates.Add(new InlineSourceUpdate() { KoobooId = koobooid, AttributeName = update.AttributeName, Value = update.Value });
                        }
                    }
                }

                if (itemupdates.Count() > 0)
                {
                    item.Body = UpdateHelper.Update(item.Body, itemupdates);
                    sitedb.Pages.AddOrUpdate(item);
                }

            }
        }
    }

}
