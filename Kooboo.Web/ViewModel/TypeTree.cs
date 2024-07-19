//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Web.ViewModel
{
    public class TypeTree
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string TypeName { get; set; }

        public string TypeFullName { get; set; }

        public string TypeAssemblyQualifiedName { get; set; }

        public string Text { get; set; }

        public string Icon { get; set; }

        public bool IsOpen { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public NodeType NodeType { get; set; }

        private List<TypeTree> _children;
        public List<TypeTree> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<TypeTree>();
                }
                return _children;
            }
            set { _children = value; }
        }

        public void AddChild(TypeTree node)
        {
            node.Parent = this;
            this.Children.Add(node);
        }

        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public TypeTree Parent { get; set; }
    }

    public class TreeNodeState
    {
        public bool Opened { get; set; }

        public bool Disabled { get; set; }

        public bool Selected { get; set; }
    }

    public enum NodeType
    {
        Assembly,
        NameSpace,
        Type,
        Method
    }


}
