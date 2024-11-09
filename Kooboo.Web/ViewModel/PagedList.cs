//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Api;

namespace Kooboo.Web.ViewModel
{
    public class PagedListViewModel<T>
    {
        public List<T> List { get; set; } = new List<T>();

        public int TotalCount { get; set; }

        public int PageNr { get; set; }

        public int PageSize { get; set; }

        private int _totalPages;

        public int TotalPages
        {
            get
            {
                if (_totalPages == 0)
                {
                    if (this.TotalCount > 0 && this.PageSize > 0)
                    {
                        _totalPages = ApiHelper.GetPageCount(this.TotalCount, this.PageSize);
                    }
                }
                return _totalPages;
            }
            set
            {
                _totalPages = value;
            }
        }

        /// <summary>
        /// �Ƿ������޷�ҳ
        /// </summary>
        public bool Infinite { get; set; }
    }

    public class PagedListViewModel<TRow, TColumn> : PagedListViewModel<TRow> where TColumn : BaseColumnViewModel
    {
        public IEnumerable<TColumn> Columns { get; set; }
    }

    public class BaseColumnViewModel
    {
        public string Name { get; set; }
        public bool IsSummaryField { get; set; }
        public string DisplayName { get; set; }
        public string ControlType { get; set; }
        public bool MultipleValue { get; set; }
        public string SelectionOptions { get; set; }
    }

    public class InfiniteListViewModel<T>
    {
        public IEnumerable<T> List { get; set; } = Enumerable.Empty<T>();

        private int page;
        public int PageNr
        {
            get
            {
                if (page < 1)
                {
                    return 1;
                }
                return page;
            }
            set
            {
                page = value;
            }
        }

        public int PageSize { get; set; } = 20;

        public bool HasMore { get; set; }
    }
}
