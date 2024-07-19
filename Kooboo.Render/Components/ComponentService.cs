using System;
using System.Collections.Generic;

namespace Kooboo.Render.Components
{
    public static class ComponentService
    {
        static ComponentService()
        {
            regularTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            regularTags.UnionWith(Kooboo.Sites.Tag.ContentModel.SectionList);
            regularTags.UnionWith(Sites.Tag.ContentModel.GroupingList);
            regularTags.UnionWith(Sites.Tag.ContentModel.TextList);
            regularTags.UnionWith(Sites.Tag.ContentModel.FormList);
            regularTags.UnionWith(Sites.Tag.ContentModel.TableList);
            regularTags.UnionWith(Sites.Tag.ContentModel.MetaList);
            regularTags.UnionWith(Sites.Tag.ContentModel.Interactive);
        }

        private static HashSet<string> regularTags { get; set; }

        public static bool IsServerComponent(string tagname)
        {
            if (Source.ComponentList.ContainsKey(tagname))
            {
                return true;
            }

            //TODO: when DEDUG, try reload, in order for people easier to use. 
            if (regularTags.Contains(tagname))
            {
                return false;
            }

            return false;
        }

        private static bool TryReload(string tagName)
        {

            return false;
        }



        public static Component GetComponent(string componentName)
        {
            if (Source.ComponentList.ContainsKey(componentName))
            {
                // check if it is the lastest one. 
                var item = Source.ComponentList[componentName];


#if Debug
                // reload the file.  
                 var com = LoadComponent(item.FullDiskPath); 
                return com; 

#endif
                return item;

            }
            else
            {

#if Debug

                string path = System.IO.Path.Combine(Data.AppSettings.RootPath, "_admin");  
                var file = FindMapping(path, componentName);
                if (file !=null)
                {
                    var com = LoadComponent(file);
                    ComponentList[componentName] = com;
                    return com; 
                } 
#endif

            }

            return null;
        }




    }
}
