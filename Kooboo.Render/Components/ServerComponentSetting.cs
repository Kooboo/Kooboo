using System;
using System.Collections.Generic;
using Kooboo.Dom;

namespace Kooboo.Render.Components
{

    public class ServerComponentSetting
    {
        public string NameOrId { get; set; }

        public Dictionary<string, string> Settings { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public string InnerHtml { get; set; }

        public ServerComponentSetting()
        {
            Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public static ServerComponentSetting LoadFromElement(Element ComponentElement)
        {
            ServerComponentSetting setting = new ServerComponentSetting();

            setting.NameOrId = ComponentElement.id;

            setting.InnerHtml = ComponentElement.InnerHtml;

            foreach (var item in ComponentElement.attributes)
            {
                setting.Attributes.Add(item.name, item.value);
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

    public class SettingNode
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public List<SettingNode> ChildNodes { get; set; }
    }

}
