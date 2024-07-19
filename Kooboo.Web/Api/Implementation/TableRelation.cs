using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Web.Api.Implementation
{
    public class TableRelationApi : SiteObjectApi<TableRelation>
    {
        [Permission(Feature.TABLE_RELATION, Action = Data.Permission.Action.VIEW)]
        public List<TableFieldsViewModel> getTablesAndFields(ApiCall call)
        {
            List<TableFieldsViewModel> Result = new List<TableFieldsViewModel>();

            var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);
            var tablelist = db.GetTables();

            foreach (var item in tablelist)
            {
                if (!item.StartsWith("_sys_"))
                {
                    var table = Data.DB.GetTable(db, item);
                    if (table != null)
                    {
                        TableFieldsViewModel model = new TableFieldsViewModel();

                        model.Name = table.Name;
                        foreach (var col in table.Setting.Columns)
                        {
                            //if (!col.IsSystem && col.Name != "_id")
                            //{
                            model.Fields.Add(col.Name);
                            //}
                        }
                        Result.Add(model);
                    }
                }
            }

            return Result;
        }

        [Permission(Feature.TABLE_RELATION, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

        [Permission(Feature.TABLE_RELATION, Action = Data.Permission.Action.VIEW)]
        public List<RelationTypeViewModel> GetRelationTypes(ApiCall call)
        {
            List<RelationTypeViewModel> Result = _GetRelationTypes(call);

            return Result;
        }

        private List<RelationTypeViewModel> _GetRelationTypes(ApiCall call)
        {
            List<RelationTypeViewModel> Result = new List<RelationTypeViewModel>();

            Result.Add(new RelationTypeViewModel() { Type = EnumTableRelation.OneOne.ToString(), DisplayName = Kooboo.Data.Language.Hardcoded.GetValue("OneOne", call.Context) });

            Result.Add(new RelationTypeViewModel() { Type = EnumTableRelation.OneMany.ToString(), DisplayName = Kooboo.Data.Language.Hardcoded.GetValue("OneMany", call.Context) });

            Result.Add(new RelationTypeViewModel() { Type = EnumTableRelation.ManyMany.ToString(), DisplayName = Kooboo.Data.Language.Hardcoded.GetValue("ManyMany", call.Context) });

            Result.Add(new RelationTypeViewModel() { Type = EnumTableRelation.ManyOne.ToString(), DisplayName = Kooboo.Data.Language.Hardcoded.GetValue("ManyOne", call.Context) });
            return Result;
        }

        [Permission(Feature.TABLE_RELATION, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }

        [Permission(Feature.TABLE_RELATION, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.TABLE_RELATION, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.TABLE_RELATION, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.TABLE_RELATION, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            return sitedb
                .GetSiteRepository<TableRelationRepository>()
                .All()
                .Select(item =>
                {
                    var model = new TableRelationViewModel(item);
                    if (item.Relation == EnumTableRelation.OneOne)
                    {
                        model.relationName = Data.Language.Hardcoded.GetValue("OneOne", call.Context);
                    }
                    else if (item.Relation == EnumTableRelation.OneMany)
                    {
                        model.relationName = Data.Language.Hardcoded.GetValue("OneMany", call.Context);
                    }
                    else if (item.Relation == EnumTableRelation.ManyMany)
                    {
                        model.relationName = Data.Language.Hardcoded.GetValue("ManyMany", call.Context);
                    }
                    else if (item.Relation == EnumTableRelation.ManyOne)
                    {
                        model.relationName = Data.Language.Hardcoded.GetValue("ManyOne", call.Context);
                    }
                    return model;
                })
                .OrderBy(it => it.Name)
                .ToList<object>();
        }
    }

    public class TableRelationViewModel
    {
        public TableRelationViewModel(TableRelation relation)
        {
            this.FieldA = relation.FieldA;
            this.FieldB = relation.FieldB;
            this.Relation = relation.Relation;
            this.TableA = relation.TableA;
            this.TableB = relation.TableB;
            this.Id = relation.Id;
            this.Name = relation.Name;
        }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public string TableA { get; set; }

        public string FieldA { get; set; }


        public string TableB { get; set; }

        public string FieldB { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnumTableRelation Relation { get; set; }

        public string relationName { get; set; }
    }


    public class RelationTypeViewModel
    {
        public string Type { get; set; }

        public string DisplayName { get; set; }
    }

    public class TableFieldsViewModel
    {
        public string Name { get; set; }

        public List<string> Fields { get; set; } = new List<string>();

    }
}








//getTableRelationTypes
//[{
//  type: '',
//  displayName: ''
//}]


//AddRelation
//{
//  name: '',      userfood.var user = k.getuser(); user.userfood
//tableA: '',    user
//fieldA: '',
//  tableB: '',    food
//  fieldB: '',
//  relationType: ''
//}


//RelationList
//[{
//  name: '',      userfood.var user = k.getuser(); user.userfood
//tableA: '',    user
//fieldA: '',
//  tableB: '',    food
//  fieldB: '',
//  relation: {}
//}]