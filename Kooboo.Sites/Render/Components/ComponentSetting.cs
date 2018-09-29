//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom;

namespace Kooboo.Sites.Render.Components
{
    public class ComponentSetting
    {
        public string NameOrId { get; set; }

        public Dictionary<string, string> Settings { get; set; }

        public string Engine { get; set; }

        public Dictionary<string, string> TagAttributes { get; set; }
        /// <summary>
        /// The tag name of this component, in order to get the type to render the component. 
        /// </summary>
        public string TagName { get; set; }

        public string InnerHtml { get; set; }

        public ComponentSetting()
        {
            Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            TagAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); 
        }

        public static ComponentSetting LoadFromElement(Element ComponentElement)
        {
            ComponentSetting setting = new ComponentSetting();

            setting.NameOrId = ComponentElement.id;
            setting.TagName = ComponentElement.tagName;

            setting.InnerHtml = ComponentElement.InnerHtml;

            foreach (var item in ComponentElement.attributes)
            {
                if (item !=null && item.name.ToLower() == Kooboo.Sites.Render.Components.Constants.KoobooAttributeName)
                {
                    setting.Engine = item.value!=null? item.value.ToLower():null; 
                }
                setting.TagAttributes.Add(item.name, item.value); 
            }
             
            if (string.IsNullOrEmpty(setting.TagName))
            {
                return null;
            } 

            if (string.IsNullOrWhiteSpace(setting.NameOrId) && string.IsNullOrWhiteSpace(setting.InnerHtml))
            {
                return null;
            }
              
            foreach (var item in ComponentElement.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    Element child = item as Element;
                    if (child == null)
                    {
                        continue;
                    }
                    string elementid = child.id;
                    if (string.IsNullOrEmpty(elementid))
                    {
                        elementid = child.tagName;
                    }

                    if (!string.IsNullOrEmpty(elementid) && !setting.Settings.ContainsKey(elementid))
                    {
                        setting.Settings.Add(elementid, child.textContent);
                    }
                }
            }
            return setting;
        }
    }
}
