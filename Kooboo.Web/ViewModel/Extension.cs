//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{
    public class TypeModel
    {
        public string FullName { get; set; }

        public string AssemblyQualifiedName { get; set; }

        public List<MethodModel> SelectedMethods { get; set; } = new List<MethodModel>();

    }

    public class MethodModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsPost { get; set; }

        public Guid MethodHash { get; set; }
    }
}
