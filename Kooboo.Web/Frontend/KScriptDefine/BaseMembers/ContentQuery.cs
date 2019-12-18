using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface ContentQuery
    {
        [Description("the number of items that will be skipped")]
        int Skipcount { get; set; }

        [Description("Is ascending order")]
        bool Ascending { get; set; }

        [Description("The field name to order by")]
        string OrderByField { get; set; }

        [Description("The search query")]
        string SearchCondition { get; set; }

        int Count();

        [Description("the order by field name")]
        ContentQuery OrderBy(string fieldname);

        [Description("the field name to order by in Descending order")]
        ContentQuery OrderByDescending(string fieldname);

        ContentQuery Skip(int skip);

        [Description("Return an array of the TextContentObjects.")]
        TextContentObject[] Take(int count);
    }
}
