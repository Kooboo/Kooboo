using System.Linq;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.Database;

public class PagedListColumn
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string DataType { get; set; }

    public string ControlType { get; set; }

    public bool IsSystem { get; set; }
}

public class PagedListViewModelWithPrimaryKey<T> : PagedListViewModel<T>
{
    public PagedListViewModelWithPrimaryKey(IEnumerable<DbTableColumn> columns)
    {
        PrimaryKey = columns.FirstOrDefault(it => it.IsPrimaryKey)?.Name;

        Columns = columns.Select(it => new PagedListColumn
        {
            Name = it.Name,
            DataType = it.DataType,
            ControlType = it.ControlType,
            IsSystem = it.IsSystem
        });
    }

    public PagedListViewModelWithPrimaryKey(IEnumerable<TableColumn> columns)
    {
        PrimaryKey = columns.FirstOrDefault(it => it.IsPrimaryKey)?.Name;
        Columns = columns.Select(it => new PagedListColumn
        {
            Name = it.Name,
            DataType = it.DataType,
            ControlType = it.ControlType,
            IsSystem = it.IsSystem
        });
    }

    public string PrimaryKey { get; set; }

    public IEnumerable<PagedListColumn> Columns { get; set; }
}