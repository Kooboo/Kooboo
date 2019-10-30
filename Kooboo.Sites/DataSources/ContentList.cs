//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Definition;
using Kooboo.Data.Models;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.DataSources
{
    public class ContentList : SiteDataSource
    {
        public bool IsDefault
        {
            get
            {
                return this.Context.RenderContext.Request.Channel == Data.Context.RequestChannel.Default;
            }
        }

        [Kooboo.Attributes.RequireFolder]
        [Kooboo.Attributes.ReturnType(typeof(List<TextContentViewModel>))]
        public PagedResult ByFolder(Guid folderId, List<FilterDefinition> filters, int pageSize, int pageNumber, string sortField, Boolean isAscending)
        {
            var query = Context.SiteDb.TextContent.Query.Where(o => o.FolderId == folderId);

            if (this.IsDefault)
            {
                query.Where(o => o.Online);
            }
            var orgcontents = query.SelectAll();
            var props = Context.SiteDb.ContentTypes.GetPropertiesByFolder(folderId);

            SetSortField(ref sortField, props);

            var contentviews = Helper.ContentHelper.ToViews(orgcontents, this.Context.RenderContext.Culture, props);

            contentviews = SortFilterContentViews(contentviews, folderId, filters, sortField, isAscending);
            return GetPagedResult(contentviews, pageSize, pageNumber);
        }

        private static PagedResult GetPagedResult(List<TextContentViewModel> contents, int pageSize, int pageNumber)
        {
            if (pageSize <= 0)
            {
                pageSize = 100;
            }
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }
            if (contents != null)
            {
                int skip = (pageNumber - 1) * pageSize;
                PagedResult pagedresult = new PagedResult {TotalCount = contents.Count()};
                pagedresult.TotalPages = Lib.Helper.CalculationHelper.GetPageCount(pagedresult.TotalCount, pageSize);
                pagedresult.DataList = contents.Skip(skip).Take(pageSize).ToList().ToList<object>();
                return pagedresult;
            }
            return null;
        }

        private object GetValue(object input, DataTypes datatype)
        {
            if (datatype == DataTypes.Undefined || input == null)
            {
                return input;
            }
            else
            {
                if (datatype == DataTypes.Undefined)
                {
                    return input;
                }
                if (datatype == DataTypes.DateTime)
                {
                    if (input is DateTime)
                    {
                        return input;
                    }
                    else
                    {
                        return DateTime.TryParse(input.ToString(), out var value) ? value : default(DateTime);
                    }
                }
                else if (datatype == DataTypes.Bool)
                {
                    return bool.TryParse(input.ToString(), out var value) && value;
                }
                else if (datatype == DataTypes.Decimal)
                {
                    return Decimal.TryParse(input.ToString(), out var value) ? value : default(Decimal);
                }
                else if (datatype == DataTypes.Guid)
                {
                    return System.Guid.TryParse(input.ToString(), out var value) ? value : default(Guid);
                }
                else if (datatype == DataTypes.Number)
                {
                    if (System.Int32.TryParse(input.ToString(), out var value))
                    {
                        return value;
                    }
                    return value;
                }
                return input;
            }
        }

        private object ChangeType(object input, Type clrType)
        {
            var value = Convert.ChangeType(input, clrType);
            return value ?? value?.ToString();
        }

        private List<TextContentViewModel> SortFilterContentViews(List<TextContentViewModel> contentviews, Guid folderId, List<FilterDefinition> filters, string sortField, bool isAscending)
        {
            if (filters != null && filters.Count > 0)
            {
                var folder = Context.SiteDb.ContentFolders.Get(folderId);
                if (folder == null)
                {
                    return null;
                }
                var columns = Context.SiteDb.ContentFolders.GetColumns(folder).ToList();

                foreach (var filter in filters)
                {
                    contentviews = Filter(contentviews, filter.FieldName, filter.Comparer, filter.FieldValue, columns);
                }
            }

            if (!String.IsNullOrWhiteSpace(sortField))
            {
                DataTypes datatype = DataTypes.Undefined;
                var contenttype = this.Context.SiteDb.ContentTypes.GetByFolder(folderId);
                var field = contenttype?.Properties.Find(o => o.Name == sortField);
                if (field != null)
                {
                    datatype = field.DataType;
                }

                Type clrtype = Data.Helper.DataTypeHelper.ToClrType(datatype);

                if (isAscending)
                {
                    return contentviews.OrderBy(c => GetValue(c.GetValue(sortField), datatype)).ToList();
                }
                else
                {
                    return contentviews.OrderByDescending(c => GetValue(c.GetValue(sortField), datatype)).ToList();
                }
            }
            else
            {
                if (isAscending)
                {
                    return contentviews.OrderBy(c => c.LastModified).ToList();
                }
                else
                {
                    return contentviews.OrderByDescending(c => c.LastModified).ToList();
                }
            }
        }

        private List<TextContentViewModel> Filter(List<TextContentViewModel> contents, string fieldName, Comparer comparer, string fieldValue, List<ContentProperty> columns)
        {
            if (columns == null)
            {
                return contents;
            }
            var column = columns.Find(col => col.Name == fieldName);
            if (column == null)
            {
                return contents;
            }

            if (String.IsNullOrWhiteSpace(fieldValue) && column.DataType != DataTypes.String)
            {
                return contents;
            }

            var clrtype = Data.Helper.DataTypeHelper.ToClrType(column.DataType);

            List<TextContentViewModel> result = new List<TextContentViewModel>();

            foreach (var item in contents)
            {
                var itemvalue = item.GetValue(fieldName);

                string contentFieldValue = itemvalue != null ? itemvalue.ToString() : string.Empty;

                if (FilterHelper.Check(contentFieldValue, comparer, fieldValue, clrtype))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        internal List<TextContentViewModel> ByCategory(Guid categoryId, Guid contentFolderId, List<FilterDefinition> filters, string sortField, Boolean isAscending)
        {
            var allcontentids = this.Context.SiteDb.ContentCategories.Query.Where(o => o.CategoryId == categoryId).SelectAll().Select(o => o.ContentId).ToList();

            var categoryContentquery = this.Context.SiteDb.TextContent.Query.Where(o => o.FolderId == contentFolderId).WhereIn("Id", allcontentids);

            if (this.IsDefault)
            {
                categoryContentquery.Where(o => o.Online == true);
            }

            var allorgcontents = categoryContentquery.SelectAll();

            var props = Context.SiteDb.ContentTypes.GetPropertiesByFolder(contentFolderId);

            SetSortField(ref sortField, props);

            var allcontents = Helper.ContentHelper.ToViews(allorgcontents, this.Context.RenderContext.Culture, props);

            return SortFilterContentViews(allcontents, contentFolderId, filters, sortField, isAscending);
        }

        private List<TextContentViewModel> ByAllCategory(Guid contentFolderId, List<FilterDefinition> filters, string sortField, Boolean isAscending)
        {
            var allContentInCategoryIds = this.Context.SiteDb.ContentCategories.Query.SelectAll().Select(o => o.ContentId).ToList();

            var allcategoryquery = this.Context.SiteDb.TextContent.Query.Where(o => o.FolderId == contentFolderId).WhereIn("Id", allContentInCategoryIds);

            if (this.IsDefault)
            {
                allcategoryquery.Where(o => o.Online == true);
            }
            var allorgcontents = allcategoryquery.SelectAll();

            var props = Context.SiteDb.ContentTypes.GetPropertiesByFolder(contentFolderId);

            SetSortField(ref sortField, props);

            var allcontents = Helper.ContentHelper.ToViews(allorgcontents, this.Context.RenderContext.Culture, props);

            return SortFilterContentViews(allcontents, contentFolderId, filters, sortField, isAscending);
        }

        [Kooboo.Attributes.RequireFolder]
        [Kooboo.Attributes.ReturnType(typeof(List<TextContentViewModel>))]
        public PagedResult ByCategoryId(Guid id, Guid folderId, List<FilterDefinition> filters, int pageSize, int pageNumber, string sortField, Boolean isAscending)
        {
            List<TextContentViewModel> result;
            if (id != default(Guid))
            {
                var category = this.Context.SiteDb.TextContent.Get(id);
                result = ByCategory(category.Id, folderId, filters, sortField, isAscending);
            }
            else
            {
                result = ByAllCategory(folderId, filters, sortField, isAscending);
            }
            return GetPagedResult(result, pageSize, pageNumber);
        }

        [Kooboo.Attributes.RequireFolder]
        [Kooboo.Attributes.ReturnType(typeof(List<TextContentViewModel>))]
        public PagedResult ByCategoryKey(string userKey, Guid folderId, List<FilterDefinition> filters, int pageSize, int pageNumber, string sortField, Boolean isAscending)
        {
            Guid id = default(Guid);
            if (!string.IsNullOrEmpty(userKey))
            {
                id = Lib.Security.Hash.ComputeGuidIgnoreCase(userKey);
            }
            return ByCategoryId(id, folderId, filters, pageSize, pageNumber, sortField, isAscending);
        }

        private void SetSortField(ref string sortField, List<ContentProperty> props)
        {
            if (string.IsNullOrEmpty(sortField) && props != null)
            {
                var seqEditable = props.Find(o => o.Name.ToLower() == SystemFields.Sequence.Name.ToLower());
                if (seqEditable != null && seqEditable.Editable)
                {
                    sortField = SystemFields.Sequence.Name.ToLower();
                }
            }
        }
    }
}