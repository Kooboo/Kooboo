//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Definition;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.ViewModel;

namespace Kooboo.Web.ViewModel
{
    public class LangTextContentViewModel
    {
        /// <summary>
        /// {folderid,categoriesids}
        /// </summary>
        public Dictionary<Guid, List<Guid>> Categories { get; set; } = new Dictionary<Guid, List<Guid>>();

        public Dictionary<Guid, List<Guid>> Embedded { get; set; } = new Dictionary<Guid, List<Guid>>();

        public string Id { get; set; }

        public string ParentId { get; set; }

        public string UserKey { get; set; }

        public string FolderId { get; set; }

        public string ContentTypeId { get; set; }

        public bool Online { get; set; }

        private Dictionary<string, Dictionary<string, string>> _values;

        /// <summary>
        /// {enus:{title:"title",summary:"summary"}}
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Values
        {
            get
            {
                if (_values == null)
                {
                    _values = new Dictionary<string, Dictionary<string, string>>();
                }
                return _values;
            }
            set { _values = value; }
        }
    }

    public class ColumnViewModel : BaseColumnViewModel
    {
        public DataTypes DataType { get; set; }

        public List<string> Operators
        {
            get
            {

                var support = Kooboo.Data.Helper.DataTypeHelper.GetSupportedComparers(this.DataType);

                List<string> result = new List<string>();

                foreach (var item in support)
                {
                    result.Add(item.ToString());
                }

                return result;
                //  EqualTo = 0,
                //GreaterThan = 1,
                //GreaterThanOrEqual = 2,
                //LessThan = 3,
                //LessThanOrEqual = 4,
                //NotEqualTo = 5,
                //StartWith = 6,
                //Contains = 7, 

                //List<string> operators = new List<string>();
                //operators.Add("EqualTo");
                //operators.Add("NotEqualTo");

                //if (this.DataType == DataTypes.Number || this.DataType == DataTypes.DateTime || this.DataType == DataTypes.Decimal)
                //{
                //    operators.Add("GreaterThan");
                //    operators.Add("GreaterThanOrEqual");
                //    operators.Add("LessThan");
                //    operators.Add("LessThanOrEqual");
                //}
                //if (this.DataType == DataTypes.String)
                //{
                //    operators.Add("contains");
                //    operators.Add("startwith");
                //}
                //return operators;
            }
        }
    }

    public class ContentFieldViewModel : BaseColumnViewModel
    {
        public Dictionary<string, string> Values = new Dictionary<string, string>();

        public string ControlType { get; set; }

        public string Validations { get; set; }

        public string Settings { get; set; }

        public string ToolTip { get; set; }

        public int Order { get; set; }

        public bool IsMultilingual { get; set; }

        public bool MultipleValue { get; set; }

        public string selectionOptions { get; set; }

    }

    public class CategoryContentViewModel
    {
        public ContentFolder CategoryFolder { get; set; }

        public string Alias { get; set; }

        public List<TextContentViewModel> Contents { get; set; } = new List<TextContentViewModel>();
        public BaseColumnViewModel[] Columns { get; set; }

        public string Display { get; set; }

        public bool MultipleChoice { get; set; }
    }

    public class EmbeddedContentViewModel:Sites.ViewModel.EmbeddedContentViewModel
    {
        public BaseColumnViewModel[] Columns { get; set; }
    }


    public class ContentEditViewModel
    {
        public List<ContentFieldViewModel> Properties { get; set; }

        public List<CategoryContentViewModel> Categories { get; set; }

        public List<EmbeddedContentViewModel> Embedded { get; set; }

        public Guid FolderId { get; set; }
    }

    public class CategoryOptionViewModel
    {
        public Guid Id { get; set; }

        public string Alias { get; set; }

        public string Display { get; set; }

        public bool MultipleChoice { get; set; }
        public BaseColumnViewModel[] Columns { get; set; }
        public List<TextContentViewModel> Options { get; set; } = new List<TextContentViewModel>();
    }

    public class PagedTextContentListViewModel : PagedListViewModel<TextContentViewModel, BaseColumnViewModel>
    {
        public List<CategoryOptionViewModel> Categories { get; set; }
    }
}
