using Kooboo.Sites.Models;

namespace Kooboo.Web.ViewModel;

public abstract class PagedViewModel<TFile, TFolder>
{
    public IEnumerable<string> Providers { get; set; }

    public IEnumerable<TFolder> Folders { get; set; }

    public PagedListViewModel<TFile> Files { get; set; }

    public IEnumerable<CrumbPath> CrumbPath { get; set; }
}
