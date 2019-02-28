//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Repository;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.Sites.Render.Components
{
    public static class Manager
    {
        public static string Preview(SiteDb sitedb, string TagName, string NameOrId)
        {
            var component = Container.Get(TagName);
            if (component != null)
            {
                return component.Preview(sitedb, NameOrId);
            }
            return null;
        }


        public static bool ElementHasEngine(Element element)
        {
            var att = element.attributes.Find(o => Lib.Helper.StringHelper.IsSameValue(o.name, Constants.KoobooAttributeName));

            if (att != null)
            {
                return Kooboo.Sites.Engine.Manager.HasEngine(att.value);
            }
            else
            {
                return false;
            }
        }


        public static bool IsComponent(Element element)
        {
            return IsComponentElement(element);
        }

        // should save the special store for this component or as embedded. etc. 
        public static bool IsComponentElement(Element element)
        {

            var component = Container.Get(element.tagName);

            if (component == null)
            {
                return false;
            }

            if (component.IsRegularHtmlTag)
            {
                if (ElementHasEngine(element))
                {
                    return true;
                }
            }
            else      
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(element.id))
            {
                return false;
            }    

            foreach (var item in element.childNodes.item)
            {
                if (item.nodeType == enumNodeType.TEXT)
                {
                    var textnode = item as Text;
                    if (textnode != null && !string.IsNullOrWhiteSpace(textnode.data))
                    {
                        return false;
                    }
                }
                else if (item.nodeType == enumNodeType.ELEMENT)
                {
                    var el = item as Element;
                    if (el != null)
                    {
                        var tag = el.tagName.ToLower();
                        if (tag != "id" && !tag.StartsWith("kooboo") && !tag.StartsWith("kb") && !tag.StartsWith("setting"))
                        {
                            return false;
                        }
                    }
                }
            }


            foreach (var item in element.attributes)
            {
                if (item.name != null)
                {
                    var lower = item.name.ToLower();
                    if (lower != "id" && !lower.StartsWith("koobooo") && !lower.StartsWith("kb") && !lower.StartsWith("setting"))
                    {
                        return false;
                    }
                }
            }

            if (string.IsNullOrEmpty(element.id))
            {
                return false;
            }


            return true;
        }

        // should save the special store for this component or as embedded. etc. 
        public static bool IsKScript(Element element)
        {
            if (element.tagName.ToLower() == "script")
            {
                var engine = element.getAttribute("engine");

                if (Lib.Helper.StringHelper.IsSameValue(engine, "kscript"))
                {
                    return true;
                }
            }

            return false;
        }


        public static List<ComponentInfo> AvailableObjects(SiteDb sitedb, string TagName)
        {
            var component = Container.Get(TagName);
            if (component != null)
            {
                return component.AvaiableObjects(sitedb);
            }
            return null;
        }
    }
}
