using Kooboo.Data.Context;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Scripting.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Scripting.Global
{
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverterDynamicObject))]
    public class DynamicTableObject : Kooboo.Data.Interface.IDynamic
    {
        public IDictionary<string, object> obj { get; set; }
        private RenderContext Context { get; set; }
        private Table Table { get; set; }

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
            this.Context = renderContext;
            this.Table = orgtable;
        }

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
            if (Context?.WebSite != null)
            {
                var sitedb = Context.WebSite.SiteDb();
                var repo = sitedb.GetSiteRepository<TableRelationRepository>();
                var relation = repo.GetRelation(this.Table.Name, key);
                if (relation != null)
                {
                    var db = Kooboo.Data.DB.GetKDatabase(this.Context.WebSite);

                    if (relation.TableA == this.Table.Name)
                    {
                        if (obj.ContainsKey(relation.FieldA))
                        {
                            var fielda = obj[relation.FieldA];

                            if (relation.Relation == EnumTableRelation.ManyMany || relation.Relation == EnumTableRelation.OneMany)
                            {
                                var tableB = Data.DB.GetTable(db, relation.TableB);
                                var result = tableB.Query.WhereEqual(relation.FieldB, fielda).Take(999);
                                return CreateList(result.ToArray(), tableB, this.Context);
                            }
                            else
                            {
                                var tableB = Data.DB.GetTable(db, relation.TableB);
                                var result = tableB.Query.WhereEqual(relation.FieldB, fielda).FirstOrDefault();
                                return Create(result, tableB, this.Context);
                            }
                        }
                    }
                    else if (relation.TableB == this.Table.Name)
                    {
                        if (obj.ContainsKey(relation.FieldB))
                        {
                            var fieldb = obj[relation.FieldB];

                            if (relation.Relation == EnumTableRelation.ManyMany || relation.Relation == EnumTableRelation.OneMany)
                            {
                                var tableB = Data.DB.GetTable(db, relation.TableA);
                                var result = tableB.Query.WhereEqual(relation.FieldA, fieldb).Take(999);
                                return CreateList(result.ToArray(), tableB, this.Context);
                            }
                            else
                            {
                                var tableB = Data.DB.GetTable(db, relation.TableA);
                                var result = tableB.Query.WhereEqual(relation.FieldA, fieldb).FirstOrDefault();
                                return Create(result, tableB, this.Context);
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static DynamicTableObject[] CreateList(IDictionary<string, object>[] list, Table targetTable, RenderContext context)
        {
            int len = list.Length;

            DynamicTableObject[] result = new DynamicTableObject[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Create(list[i], targetTable, context);
            }
            return result;
        }

        public static DynamicTableObject Create(IDictionary<string, object> item, Table sourceTable, RenderContext context)
        {
            if (item != null)
            {
                return new DynamicTableObject(item, sourceTable, context);
            }
            return null;
        }

        public object GetValue(string fieldName)
        {
            return GetValueFromDict(fieldName);
        }

        public object GetValue(string fieldName, RenderContext context)
        {
            return GetValueFromDict(fieldName);
        }

        public void SetValue(string fieldName, object value)
        {
            obj[fieldName] = value;
        }
    }
}