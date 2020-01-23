//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace KScript
{
    public interface ITableQuery
    {
        bool Ascending { get; set; }

        string OrderByField { get; set; }
        string SearchCondition { get; set; }
        int skipcount { get; set; }

        int count();
        ITableQuery OrderBy(string fieldname);
        ITableQuery OrderByDescending(string fieldname);
        ITableQuery skip(int skip);
        IDynamicTableObject[] take(int count);
        ITableQuery Where(string searchCondition);
    }
}