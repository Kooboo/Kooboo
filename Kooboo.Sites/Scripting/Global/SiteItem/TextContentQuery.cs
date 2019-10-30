//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Definition;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Scripting.Global.SiteItem
{
    public class TextContentQuery
    {
        public TextContentQuery(TextContentObjectRepository txtObjRepo)
        {
            this.txtObjRepo = txtObjRepo;
        }

        [Attributes.SummaryIgnore]
        public TextContentObjectRepository txtObjRepo { get; set; }

        public int skipcount { get; set; }

        public bool Ascending { get; set; }

        public string OrderByField { get; set; }

        public string SearchCondition { get; set; }

        public TextContentQuery skip(int skip)
        {
            this.skipcount = skip;
            return this;
        }

        public TextContentQuery Where(string searchCondition)
        {
            this.SearchCondition = searchCondition;
            return this;
        }

        public TextContentQuery OrderBy(string fieldname)
        {
            this.OrderByField = fieldname;
            this.Ascending = true;
            return this;
        }

        public TextContentQuery OrderByDescending(string fieldname)
        {
            this.OrderByField = fieldname;
            this.Ascending = false;
            return this;
        }

        public TextContentQuery OrderByDesc(string fieldname)
        {
            return this.OrderByDescending(fieldname);
        }

        public List<TextContentObject> take(int count)
        {
            var sitedb = this.txtObjRepo.context.WebSite.SiteDb();

            var allContentTypes = sitedb.ContentTypes.All();

            ContentType onlyType = null;
            ContentFolder onlyFolder = null;

            var condition = this.txtObjRepo.ParseCondition(this.SearchCondition);

            var tablequery = sitedb.TextContent.Query.Where(o => o.Online == true);

            if (condition.FolderId != default(Guid))
            {
                tablequery.Where(o => o.FolderId == condition.FolderId);

                var folder = sitedb.ContentFolders.Get(condition.FolderId);
                if (folder != null)
                {
                    onlyFolder = folder;
                    onlyType = sitedb.ContentTypes.Get(folder.ContentTypeId);
                }
            }

            if (condition.ContentTypeId != default(Guid))
            {
                tablequery.Where(o => o.ContentTypeId == condition.ContentTypeId);
                var type = sitedb.ContentTypes.Get(condition.ContentTypeId);
                if (type != null)
                {
                    onlyType = type;
                }
            }

            if (condition.CategoryId != default(Guid))
            {
                var allcontentids = sitedb.ContentCategories.Query.Where(o => o.CategoryId == condition.CategoryId).SelectAll().Select(o => o.ContentId).ToList();

                tablequery.WhereIn("Id", allcontentids);
            }

            var all = tablequery.SelectAll();

            var filteritems = this.txtObjRepo.filterItems(all, condition.Conditions, onlyType, onlyFolder);

            if (filteritems == null || !filteritems.Any())
            {
                return new List<TextContentObject>();
            }

            if (!string.IsNullOrWhiteSpace(this.OrderByField))
            {
                ContentProperty prop = null;
                if (onlyType != null)
                {
                    prop = onlyType.Properties.Find(o => Lib.Helper.StringHelper.IsSameValue(o.Name, this.OrderByField));
                }

                if (prop == null)
                {
                    var uniqueC = filteritems
                                  .Select(p => p.ContentTypeId)
                                  .Distinct();

                    foreach (var item in uniqueC)
                    {
                        var uniquetype = sitedb.ContentTypes.Get(item);
                        if (uniquetype != null)
                        {
                            var find = uniquetype.Properties.Find(o => Lib.Helper.StringHelper.IsSameValue(o.Name, this.OrderByField));
                            if (find != null)
                            {
                                prop = find;
                                break;
                            }
                        }
                    }
                }

                if (prop != null)
                {
                    if (this.Ascending)
                    {
                        filteritems = filteritems.OrderBy(o => GetValue(o.GetValue(this.OrderByField), prop.DataType)).ToList();
                    }
                    else
                    {
                        filteritems = filteritems.OrderByDescending(c => GetValue(c.GetValue(OrderByField), prop.DataType)).ToList();
                    }
                }
            }

            var txtResult = filteritems.Skip(this.skipcount).Take(count);

            List<TextContentObject> result = new List<TextContentObject>();

            foreach (var item in txtResult)
            {
                var obj = new TextContentObject(item, this.txtObjRepo.context);
                result.Add(obj);
            }
            return result;
        }

        private object GetValue(object input, DataTypes datatype)
        {
            if (datatype == DataTypes.Undefined || input == null)
            {
                return input;
            }
            else
            {
                switch (datatype)
                {
                    case DataTypes.Undefined:
                        return input;
                    case DataTypes.DateTime when input is DateTime:
                        return input;
                    case DataTypes.DateTime:
                    {
                        return DateTime.TryParse(input.ToString(), out var value) ? value : default(DateTime);
                    }
                    case DataTypes.Bool:
                    {
                        return bool.TryParse(input.ToString(), out var value) && value;
                    }
                    case DataTypes.Decimal:
                    {
                        return Decimal.TryParse(input.ToString(), out var value) ? value : default(Decimal);
                    }
                    case DataTypes.Guid:
                    {
                        return System.Guid.TryParse(input.ToString(), out var value) ? value : default(Guid);
                    }
                    case DataTypes.Number:
                    {
                        if (System.Int32.TryParse(input.ToString(), out var value))
                        {
                            return value;
                        }
                        return value;
                    }
                    default:
                        return input;
                }
            }
        }

        public int count()
        {
            // TODO: improve performance.
            var all = take(99999);
            return all == null ? 0 : all.Count();
        }
    }
}