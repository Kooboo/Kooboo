//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Contents.Models;

namespace Kooboo.Web.ViewModel
{
    public class ContentTypeItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int PropertyCount { get; set; }

        public DateTime LastModified { get; set; }


    }

    public class ContentTypeViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsNested { get; set; }

        public List<ContentProperty> Columns { get; set; }
    }

}
