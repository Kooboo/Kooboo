using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KScript.Sites
{
    public class RouteObjectRepository
    {

        [KIgnore]
        private RenderContext context { get; set; }

        private Kooboo.Sites.Repository.RouteRepository repo { get; set; }
        public RouteObjectRepository(Kooboo.Sites.Repository.RouteRepository repo, RenderContext context)
        {
            this.context = context;
            this.repo = repo;
        }

        public Kooboo.Sites.Routing.Route GetByNameOrId(string NameOrId)
        {
            return this.repo.GetByNameOrId(NameOrId);
        }

        public Kooboo.Sites.Routing.Route GetByUrl(string Url)
        {
            return this.repo.GetByUrl(Url);
        }

        public Kooboo.Sites.Routing.Route GetByObjectId(object objectId)
        {
            if (objectId == null)
            {
                return null;
            }
            Guid id;
            if (Guid.TryParse(objectId.ToString(), out id))
            {
                return this.repo.GetByObjectId(id);
            }

            return null;
        }

        public void ChangeRoute(string oldurl, string newurl)
        {
            this.repo.ChangeRoute(oldurl, newurl);
        }

    }

}
