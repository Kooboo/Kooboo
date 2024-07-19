//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;

namespace Kooboo.Web.Areas.Admin.ViewModels
{
    public class ViewDataSourceViewModel
    {
        public Guid Id { get; set; }
        public Guid MethodId { get; set; }
        public string AliasName { get; set; }
        public IDictionary<string, string> ParameterMappings { get; set; }
        public Guid ParentId { get; set; }
        public string DataSourceName { get; set; }
        public string DataSourceDisplayName { get; set; }

        public static List<ViewDataSourceViewModel> Create(ViewDataMethod dataSource)
        {
            List<ViewDataSourceViewModel> result = new List<ViewDataSourceViewModel>();

            var model = new ViewDataSourceViewModel
            {
                Id = dataSource.Id,
                MethodId = dataSource.MethodId,
                AliasName = dataSource.AliasName,
            };

            result.Add(model);

            if (dataSource.HasChildren)
            {
                foreach (var child in dataSource.Children)
                {
                    var sublist = Create(child);

                    foreach (var item in sublist)
                    {
                        if (item.ParentId == default(Guid))
                        {
                            item.ParentId = dataSource.Id;
                        }
                    }

                    result.AddRange(sublist);
                }
            }

            return result;
        }
    }
}
