using System;
using System.Collections.Generic;

namespace Kooboo.Render.Components
{
    public static class ComponentService
    {
        static ComponentService()
        {
            RegularTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            RegularTags.UnionWith(Kooboo.Sites.Tag.ContentModel.SectionList);
            RegularTags.UnionWith(Sites.Tag.ContentModel.GroupingList);
            RegularTags.UnionWith(Sites.Tag.ContentModel.TextList);
            RegularTags.UnionWith(Sites.Tag.ContentModel.FormList);
            RegularTags.UnionWith(Sites.Tag.ContentModel.TableList);
            RegularTags.UnionWith(Sites.Tag.ContentModel.MetaList);
            RegularTags.UnionWith(Sites.Tag.ContentModel.Interactive);
        }

        private static HashSet<string> RegularTags { get; set; }

        public static bool IsServerComponent(string tagname)
        {
            if (Source.ComponentList.ContainsKey(tagname))
            {
                return true;
            }

            //TODO: when DEDUG, try reload, in order for people easier to use.
            if (RegularTags.Contains(tagname))
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