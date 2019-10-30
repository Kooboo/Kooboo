//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;

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

        public static ComponentSetting LoadFromElement(Element componentElement)
        {
            ComponentSetting setting = new ComponentSetting
            {
                NameOrId = componentElement.id,
                TagName = componentElement.tagName,
                InnerHtml = componentElement.InnerHtml
            };



            foreach (var item in componentElement.attributes)
            {
                if (item != null && item.name.ToLower() == Kooboo.Sites.Render.Components.Constants.KoobooAttributeName)
                {
                    setting.Engine = item.value?.ToLower();
                }

                if (item != null) setting.TagAttributes.Add(item.name, item.value);
            }

            if (string.IsNullOrEmpty(setting.TagName))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(setting.NameOrId) && string.IsNullOrWhiteSpace(setting.InnerHtml))
            {
                return null;
            }

            foreach (var item in componentElement.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    if (!(item is Element child))
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