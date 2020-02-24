//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Sites.DataSources;
using Kooboo.Sites.Ecommerce.Repository;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.DataSource
{ 

    //public class EcommerceProduct : SiteDataSource
    //{
    //    public bool IsDefault
    //    {
    //        get
    //        {
    //            return this.Context.RenderContext.Request.Channel == Data.Context.RequestChannel.Default;
    //        }
    //    }

    //    public List<ViewModel.CategoryViewModel> TopCategories()
    //    {
    //        var sitedb = this.Context.RenderContext.WebSite.SiteDb();

    //        var all = sitedb.GetSiteRepository<CategoryRepository>().All().Where(o => o.ParentId == default(Guid)).ToList();

    //        List<ViewModel.CategoryViewModel> result = new List<ViewModel.CategoryViewModel>();

    //        var culture = this.Context.RenderContext.Culture;
    //        foreach (var item in all)
    //        {
    //            var value = item.GetValue(culture);
    //            ViewModel.CategoryViewModel model = new ViewModel.CategoryViewModel();
    //            model.Id = item.Id;
    //            model.name = value.ToString();
    //            result.Add(model);
    //        }
    //        return result;
    //    }
                                               
    //    public List<ViewModel.CategoryViewModel> SubCategories(Guid id)
    //    {
    //        var sitedb = this.Context.RenderContext.WebSite.SiteDb();

    //        var all = sitedb.GetSiteRepository<CategoryRepository>().All().Where(o => o.ParentId == id).ToList();

    //        List<ViewModel.CategoryViewModel> result = new List<ViewModel.CategoryViewModel>();

    //        var culture = this.Context.RenderContext.Culture;
    //        foreach (var item in all)
    //        {
    //            var value = item.GetValue(culture);
    //            ViewModel.CategoryViewModel model = new ViewModel.CategoryViewModel();
    //            model.Id = item.Id;
    //            model.name = value.ToString();
    //            result.Add(model);
    //        }
    //        return result;
    //    }

    //    public List<ViewModel.ProductViewModel> ListByCategoryName(string name)
    //    {
    //        if (string.IsNullOrEmpty(name))
    //        {
    //            return new List<ViewModel.ProductViewModel>();
    //        }

    //        Guid id = Lib.Security.Hash.ComputeGuidIgnoreCase(name);

    //        var allproducts = this.Context.SiteDb.GetSiteRepository<ProductRepository>().GetByCategory(id);

    //        List<ViewModel.ProductViewModel> result = new List<ViewModel.ProductViewModel>();

    //        string culture = this.Context.RenderContext.Culture; 

    //        foreach (var item in allproducts)
    //        {
    //            var type = this.Context.SiteDb.GetSiteRepository<ProductTypeRepository>().Get(item.ProductTypeId);

    //            if (type !=null)
    //            {
    //                var view = Helper.ProductHelper.ToView(item, culture, type.Properties);

    //                result.Add(view); 
    //            }  
    //        }     
    //        return result;
    //    }

    //    public List<ViewModel.ProductViewModel> AllProducts()
    //    {   
    //        var allproducts = this.Context.SiteDb.GetSiteRepository<ProductRepository>().All();  

    //        List<ViewModel.ProductViewModel> result = new List<ViewModel.ProductViewModel>();

    //        string culture = this.Context.RenderContext.Culture;

    //        foreach (var item in allproducts)
    //        {
    //            var type = this.Context.SiteDb.GetSiteRepository<ProductTypeRepository>().Get(item.ProductTypeId);

    //            if (type != null)
    //            {
    //                var view = Helper.ProductHelper.ToView(item, culture, type.Properties);

    //                result.Add(view);
    //            }
    //        }
    //        return result;    
    //    }

    //    [Kooboo.Attributes.RequireProductType]
    //    public List<ViewModel.ProductViewModel> ByProductType(Guid ProductTypeId, bool IsAscending)
    //    {
    //        var allproducts = this.Context.SiteDb.GetSiteRepository<ProductRepository>().All();

    //        List<ViewModel.ProductViewModel> result = new List<ViewModel.ProductViewModel>();

    //        string culture = this.Context.RenderContext.Culture;

    //        foreach (var item in allproducts)
    //        {
    //            var type = this.Context.SiteDb.GetSiteRepository<ProductTypeRepository>().Get(item.ProductTypeId);

    //            if (type != null)
    //            {
    //                var view = Helper.ProductHelper.ToView(item, culture, type.Properties);

    //                result.Add(view);
    //            }
    //        }
    //        return result;
    //    }


    //    private HashSet<Guid> GetAllSubCats(Guid Id)
    //    {
    //        HashSet<Guid> result = new HashSet<Guid>();
    //        var all = this.Context.RenderContext.WebSite.SiteDb().GetSiteRepository<CategoryRepository>().All();
    //        result.Add(Id);
    //        SetSubs(all, Id, ref result);
    //        return result;
    //    }

    //    private void SetSubs(List<Ecommerce.Models.Category> all, Guid ParentId, ref HashSet<Guid> result)
    //    {
    //        var subs = all.Where(o => o.ParentId == ParentId).ToList();
    //        foreach (var item in subs)
    //        {
    //            result.Add(item.Id);
    //            SetSubs(all, item.Id, ref result);
    //        }
    //    }

    //    //[Kooboo.Attributes.RequireFolder]
    //    //[Kooboo.Attributes.ReturnType(typeof(List<ViewModel.ProductViewModel>))]
    //    //public PagedResult ProductByCategory(Guid FolderId, List<FilterDefinition> Filters, int PageSize, int PageNumber, string SortField, Boolean IsAscending)
    //    //{
    //    //    var query = Context.SiteDb.TextContent.Query.Where(o => o.FolderId == FolderId);

    //    //    if (this.IsDefault)
    //    //    {
    //    //        query.Where(o => o.Online == true);
    //    //    }
    //    //    var orgcontents = query.SelectAll();
    //    //    var props = Context.SiteDb.ContentTypes.GetPropertiesByFolder(FolderId);

    //    //    SetSortField(ref SortField, props);

    //    //    var contentviews = Helper.ContentHelper.ToViews(orgcontents, this.Context.RenderContext.Culture, props);

    //    //    contentviews = SortFilterContentViews(contentviews, FolderId, Filters, SortField, IsAscending);
    //    //    return GetPagedResult(contentviews, PageSize, PageNumber);
    //    //}



    //    //private static PagedResult GetPagedResult(List<TextContentViewModel> contents, int PageSize, int PageNumber)
    //    //{
    //    //    if (PageSize <= 0)
    //    //    {
    //    //        PageSize = 100;
    //    //    }
    //    //    if (PageNumber <= 0)
    //    //    {
    //    //        PageNumber = 1;
    //    //    }
    //    //    if (contents != null)
    //    //    {
    //    //        int skip = (PageNumber - 1) * PageSize;
    //    //        PagedResult pagedresult = new PagedResult();
    //    //        pagedresult.TotalCount = contents.Count();
    //    //        pagedresult.TotalPages = Lib.Helper.CalculationHelper.GetPageCount(pagedresult.TotalCount, PageSize);
    //    //        pagedresult.DataList = contents.Skip(skip).Take(PageSize).ToList().ToList<object>();
    //    //        return pagedresult;
    //    //    }
    //    //    return null;
    //    //}

    //    //private object GetValue(object input, DataTypes datatype)
    //    //{
    //    //    if (datatype == DataTypes.Undefined || input == null)
    //    //    {
    //    //        return input;
    //    //    }
    //    //    else
    //    //    {
    //    //        if (datatype == DataTypes.Undefined)
    //    //        {
    //    //            return input;
    //    //        }
    //    //        if (datatype == DataTypes.DateTime)
    //    //        {
    //    //            DateTime value;
    //    //            if (input.GetType() == typeof(DateTime))
    //    //            {
    //    //                return input;
    //    //            }
    //    //            else
    //    //            {
    //    //                if (DateTime.TryParse(input.ToString(), out value))
    //    //                {
    //    //                    return value;
    //    //                }
    //    //                return default(DateTime);
    //    //            }

    //    //        }
    //    //        else if (datatype == DataTypes.Bool)
    //    //        {
    //    //            bool value;
    //    //            if (bool.TryParse(input.ToString(), out value))
    //    //            {
    //    //                return value;
    //    //            }
    //    //            return false;
    //    //        }
    //    //        else if (datatype == DataTypes.Decimal)
    //    //        {
    //    //            Decimal value;
    //    //            if (Decimal.TryParse(input.ToString(), out value))
    //    //            {
    //    //                return value;
    //    //            }
    //    //            return default(Decimal);
    //    //        }
    //    //        else if (datatype == DataTypes.Guid)
    //    //        {
    //    //            Guid value = default(Guid);
    //    //            if (System.Guid.TryParse(input.ToString(), out value))
    //    //            {
    //    //                return value;
    //    //            }
    //    //            return default(Guid);
    //    //        }
    //    //        else if (datatype == DataTypes.Number)
    //    //        {
    //    //            int value = default(int);
    //    //            if (System.Int32.TryParse(input.ToString(), out value))
    //    //            {
    //    //                return value;
    //    //            }
    //    //            return value;
    //    //        }
    //    //        return input;
    //    //    }
    //    //}

    //    //private object ChangeType(object input, Type clrType)
    //    //{
    //    //    var value = Convert.ChangeType(input, clrType);
    //    //    if (value != null)
    //    //    {
    //    //        return value;
    //    //    }
    //    //    return value.ToString();
    //    //}

    //    //private List<TextContentViewModel> SortFilterContentViews(List<TextContentViewModel> contentviews, Guid FolderId, List<FilterDefinition> Filters, string SortField, bool IsAscending)
    //    //{
    //    //    if (Filters != null && Filters.Count > 0)
    //    //    {
    //    //        var folder = Context.SiteDb.ContentFolders.Get(FolderId);
    //    //        if (folder == null)
    //    //        {
    //    //            return null;
    //    //        }
    //    //        var columns = Context.SiteDb.ContentFolders.GetColumns(folder).ToList();

    //    //        foreach (var filter in Filters)
    //    //        {
    //    //            contentviews = Filter(contentviews, filter.FieldName, filter.Comparer, filter.FieldValue, columns);
    //    //        }
    //    //    }

    //    //    if (!String.IsNullOrWhiteSpace(SortField))
    //    //    {
    //    //        DataTypes datatype = DataTypes.Undefined;
    //    //        var contenttype = this.Context.SiteDb.ContentTypes.GetByFolder(FolderId);
    //    //        if (contenttype != null)
    //    //        {
    //    //            var field = contenttype.Properties.Find(o => o.Name == SortField);
    //    //            if (field != null)
    //    //            {
    //    //                datatype = field.DataType;
    //    //            }
    //    //        }

    //    //        Type clrtype = Data.Helper.DataTypeHelper.ToClrType(datatype);

    //    //        if (IsAscending)
    //    //        {
    //    //            return contentviews.OrderBy(c => GetValue(c.GetValue(SortField), datatype)).ToList();
    //    //        }
    //    //        else
    //    //        {
    //    //            return contentviews.OrderByDescending(c => GetValue(c.GetValue(SortField), datatype)).ToList();
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        if (IsAscending)
    //    //        {
    //    //            return contentviews.OrderBy(c => c.LastModified).ToList();
    //    //        }
    //    //        else
    //    //        {
    //    //            return contentviews.OrderByDescending(c => c.LastModified).ToList();
    //    //        }
    //    //    }
    //    //}

    //    //private List<TextContentViewModel> Filter(List<TextContentViewModel> contents, string fieldName, Comparer comparer, string fieldValue, List<ContentProperty> columns)
    //    //{
    //    //    if (columns == null)
    //    //    {
    //    //        return contents;
    //    //    }
    //    //    var column = columns.Find(col => col.Name == fieldName);
    //    //    if (column == null)
    //    //    {
    //    //        return contents;
    //    //    }

    //    //    if (String.IsNullOrWhiteSpace(fieldValue) && column.DataType != DataTypes.String)
    //    //    {
    //    //        return contents;
    //    //    }

    //    //    var clrtype = Data.Helper.DataTypeHelper.ToClrType(column.DataType);

    //    //    List<TextContentViewModel> result = new List<TextContentViewModel>();

    //    //    foreach (var item in contents)
    //    //    {
    //    //        var itemvalue = item.GetValue(fieldName);

    //    //        string contentFieldValue = itemvalue != null ? itemvalue.ToString() : string.Empty;

    //    //        if (FilterHelper.Check(contentFieldValue, comparer, fieldValue, clrtype))
    //    //        {
    //    //            result.Add(item);
    //    //        }
    //    //    }

    //    //    return result;
    //    //}

    //    //internal List<TextContentViewModel> ByCategory(Guid CategoryId, Guid ContentFolderId, List<FilterDefinition> Filters, string SortField, Boolean IsAscending)
    //    //{
    //    //    var allcontentids = this.Context.SiteDb.ContentCategories.Query.Where(o => o.CategoryId == CategoryId).SelectAll().Select(o => o.ContentId).ToList();

    //    //    var categoryContentquery = this.Context.SiteDb.TextContent.Query.Where(o => o.FolderId == ContentFolderId).WhereIn("Id", allcontentids);

    //    //    if (this.IsDefault)
    //    //    {
    //    //        categoryContentquery.Where(o => o.Online == true);
    //    //    }

    //    //    var allorgcontents = categoryContentquery.SelectAll();

    //    //    var props = Context.SiteDb.ContentTypes.GetPropertiesByFolder(ContentFolderId);

    //    //    SetSortField(ref SortField, props);

    //    //    var allcontents = Helper.ContentHelper.ToViews(allorgcontents, this.Context.RenderContext.Culture, props);

    //    //    return SortFilterContentViews(allcontents, ContentFolderId, Filters, SortField, IsAscending);

    //    //}

    //    //private List<TextContentViewModel> ByAllCategory(Guid ContentFolderId, List<FilterDefinition> Filters, string SortField, Boolean IsAscending)
    //    //{
    //    //    var AllContentInCategoryIds = this.Context.SiteDb.ContentCategories.Query.SelectAll().Select(o => o.ContentId).ToList();

    //    //    var allcategoryquery = this.Context.SiteDb.TextContent.Query.Where(o => o.FolderId == ContentFolderId).WhereIn("Id", AllContentInCategoryIds);

    //    //    if (this.IsDefault)
    //    //    {
    //    //        allcategoryquery.Where(o => o.Online == true);
    //    //    }
    //    //    var allorgcontents = allcategoryquery.SelectAll();

    //    //    var props = Context.SiteDb.ContentTypes.GetPropertiesByFolder(ContentFolderId);

    //    //    SetSortField(ref SortField, props);

    //    //    var allcontents = Helper.ContentHelper.ToViews(allorgcontents, this.Context.RenderContext.Culture, props);

    //    //    return SortFilterContentViews(allcontents, ContentFolderId, Filters, SortField, IsAscending);
    //    //}

    //    //[Kooboo.Attributes.RequireFolder]
    //    //[Kooboo.Attributes.ReturnType(typeof(List<TextContentViewModel>))]
    //    //public PagedResult ByCategoryId(Guid Id, Guid FolderId, List<FilterDefinition> Filters, int PageSize, int PageNumber, string SortField, Boolean IsAscending)
    //    //{
    //    //    List<TextContentViewModel> result;
    //    //    if (Id != default(Guid))
    //    //    {
    //    //        var category = this.Context.SiteDb.TextContent.Get(Id);
    //    //        result = ByCategory(category.Id, FolderId, Filters, SortField, IsAscending);
    //    //    }
    //    //    else
    //    //    {
    //    //        result = ByAllCategory(FolderId, Filters, SortField, IsAscending);
    //    //    }
    //    //    return GetPagedResult(result, PageSize, PageNumber);
    //    //}

    //    //[Kooboo.Attributes.RequireFolder]
    //    //[Kooboo.Attributes.ReturnType(typeof(List<TextContentViewModel>))]
    //    //public PagedResult ByCategoryKey(string UserKey, Guid FolderId, List<FilterDefinition> Filters, int PageSize, int PageNumber, string SortField, Boolean IsAscending)
    //    //{
    //    //    Guid Id = default(Guid);
    //    //    if (!string.IsNullOrEmpty(UserKey))
    //    //    {
    //    //        Id = Lib.Security.Hash.ComputeGuid(UserKey);
    //    //    }
    //    //    return ByCategoryId(Id, FolderId, Filters, PageSize, PageNumber, SortField, IsAscending);
    //    //} 

    //    //private void SetSortField(ref string sortField, List<ContentProperty> props)
    //    //{
    //    //    if (string.IsNullOrEmpty(sortField) && props != null)
    //    //    {
    //    //        var seqEditable = props.Find(o => o.Name.ToLower() == SystemFields.Sequence.Name.ToLower());
    //    //        if (seqEditable != null && seqEditable.Editable)
    //    //        {
    //    //            sortField = SystemFields.Sequence.Name.ToLower();
    //    //        }
    //    //    }
    //    //} 
    //} 
}
