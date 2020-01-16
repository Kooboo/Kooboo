using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Scripting.Helper;
using System.Collections.Generic;
using System.Linq; 

namespace KScript
{
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverterDynamicObject))]
    public class DynamicTableObject :IDynamicTableObject
    {
        [KIgnore]
        public IDictionary<string, object> obj { get; set; }

        private RenderContext context { get; set; }
        private Table table { get; set; }

        public Dictionary<string, object> Values
        {
            get
            {
                return this.obj.ToDictionary(o => o.Key, o => o.Value);
            }
        }

        public DynamicTableObject(IDictionary<string, object> orgObj, Table orgtable, RenderContext renderContext)
        {
            this.obj = orgObj;
            this.context = renderContext;
            this.table = orgtable;
        }

        [KIgnore]
        public object this[string key]
        {
            get
            {
                return GetValueFromDict(key);
            }
            set
            {
                this.obj[key] = value;
            }
        }

        private object GetValueFromDict(string key)
        {
            if (obj.ContainsKey(key))
            {
                return obj[key];
            }

            // check if table in relation. 
            if (context != null && context.WebSite != null)
            {
                var sitedb = context.WebSite.SiteDb();
                var repo = sitedb.GetSiteRepository<TableRelationRepository>();
                var relation = repo.GetRelation(this.table.Name, key);
                if (relation != null)
                {
                    var db = Kooboo.Data.DB.GetKDatabase(this.context.WebSite);

                    if (relation.TableA == this.table.Name)
                    {
                        if (obj.ContainsKey(relation.FieldA))
                        {
                            var fielda = obj[relation.FieldA];

                            if (relation.Relation == EnumTableRelation.ManyMany || relation.Relation == EnumTableRelation.OneMany)
                            {
                                var tableB = Kooboo.Data.DB.GetTable(db, relation.TableB);
                                var result = tableB.Query.WhereEqual(relation.FieldB, fielda).Take(999);
                                return CreateList(result.ToArray(), tableB, this.context);
                            }
                            else
                            {
                                var tableB = Kooboo.Data.DB.GetTable(db, relation.TableB);
                                var result = tableB.Query.WhereEqual(relation.FieldB, fielda).FirstOrDefault();
                                return Create(result, tableB, this.context);
                            }
                        }
                    }
                    else if (relation.TableB == this.table.Name)
                    {
                        if (obj.ContainsKey(relation.FieldB))
                        {
                            var fieldb = obj[relation.FieldB];

                            if (relation.Relation == EnumTableRelation.ManyMany || relation.Relation == EnumTableRelation.OneMany)
                            {
                                var tableB = Kooboo.Data.DB.GetTable(db, relation.TableA);
                                var result = tableB.Query.WhereEqual(relation.FieldA, fieldb).Take(999);
                                return CreateList(result.ToArray(), tableB, this.context);
                            }
                            else
                            {
                                var tableB = Kooboo.Data.DB.GetTable(db, relation.TableA);
                                var result = tableB.Query.WhereEqual(relation.FieldA, fieldb).FirstOrDefault();
                                return Create(result, tableB, this.context);
                            }

                        }
                    }
                }
            }
            return null;
        }

        [KIgnore]
        public static DynamicTableObject[] CreateList(IDictionary<string, object>[] list, Table TargetTable, RenderContext context)
        {
            int len = list.Length;

            DynamicTableObject[] result = new DynamicTableObject[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Create(list[i], TargetTable, context);
            }
            return result;
        }

        [KIgnore]
        public static DynamicTableObject Create(IDictionary<string, object> item, Table sourceTable, RenderContext context)
        {
            if (item != null)
            {
                return new DynamicTableObject(item, sourceTable, context);
            }
            return null;

        }

        public object GetValue(string FieldName)
        {
            return GetValueFromDict(FieldName);
        }

        public object GetValue(string FieldName, RenderContext Context)
        {
            return GetValueFromDict(FieldName);
        }

        public void SetValue(string FieldName, object Value)
        {
            obj[FieldName] = Value;
        }
    }
}
