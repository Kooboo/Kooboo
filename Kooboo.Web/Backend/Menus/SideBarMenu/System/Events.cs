using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;

namespace Kooboo.Web.Menus.SideBarMenu.System
{
    public class Events : ISideBarMenu, IDynamicMenu
    {
        public SideBarSection Parent => SideBarSection.System;

        public string Name => "Events";

        public string Icon => "";

        public string Url => "";

        public int Order => 12;

        public List<ICmsMenu> SubItems { get; set; }

        public string GetDisplayName(RenderContext Context)
        {
            return Hardcoded.GetValue("Events", Context);
        }

        public List<ICmsMenu> ShowSubItems(RenderContext context)
        {   
            List<ICmsMenu> Result = new List<ICmsMenu>();

            Result.Add(new GeneralMenu() { Name = Hardcoded.GetValue("Overview", context), Url = "Events" });  

            var names = Enum.GetNames(typeof(Kooboo.Sites.FrontEvent.enumEventType));

            List<GroupEvent> groupnames = new List<GroupEvent>();
            foreach (var item in names)
            {
                GroupEvent eventname = new GroupEvent();
                eventname.name = item;
                eventname.group = GetEventGroup(item);
                groupnames.Add(eventname);
            } 

            foreach (var group in groupnames.GroupBy(o => o.group))
            {
                var item = new GeneralMenu { Name = group.Key }; 
                foreach (var oneevent in group.ToList())
                {
                    item.SubItems.Add(new GeneralMenu
                    {
                        Name = oneevent.name,
                        Url = "Events/Event?name=" + oneevent.name.ToString()
                    });
                } 
                Result.Add(item);
            } 
            return Result; 
        }

        public bool Show(RenderContext context)
        { 
            if (context.WebSite != null && context.WebSite.EnableFrontEvents)
            {
                return true;
            } 
            return false;
        }
         

        public class GroupEvent
        {
            public string group { get; set; }
            public string name { get; set; }
        }

        private string GetEventGroup(string input)
        {
            string group = string.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                var currentchar = input[i];
                if (i == 0)
                {
                    group += currentchar;
                }
                else
                {
                    if (Lib.Helper.CharHelper.isUppercaseAscii(currentchar))
                    {
                        return group;
                    }
                    else
                    {
                        group += currentchar;
                    }
                }
            }
            return group;
        }


    }
}
