using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Web.Api.Implementation
{
    public class TableRelationApi : SiteObjectApi<TableRelation>
    {
        public List<TableFieldsViewModel> getTablesAndFields(ApiCall call)
        {
            List<TableFieldsViewModel> Result = new List<TableFieldsViewModel>();

            var db = Kooboo.Data.DB.GetKDatabase(call.Context.WebSite);
            var tablelist = db.GetTables();

            foreach (var item in tablelist)
            {
                var table = db.GetOrCreateTable(item);
                if (table != null)
                {
                    TableFieldsViewModel model = new TableFieldsViewModel();

                    model.Name = table.Name;
                    foreach (var col in table.Setting.Columns)
                    {
                        if (!col.IsSystem)
                        {
                            model.Fields.Add(col.Name);
                        }
                    }
                    Result.Add(model);
                }

            }

            return Result;
        }

        public override Guid Post(ApiCall call)
        {
            return base.Post(call);
        }

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

        public override List<object> List(ApiCall call)
        {
            var types = _GetRelationTypes(call);
            var sitedb = call.Context.WebSite.SiteDb();
            var list = sitedb.GetSiteRepository<TableRelationRepository>().All();

            List<TableRelationViewModel> result = new List<TableRelationViewModel>();
            foreach (var item in list)
            {
                var model = new TableRelationViewModel(item);
                if (item.Relation == EnumTableRelation.OneOne)
                {
                    model.DisplayName = Data.Language.Hardcoded.GetValue("OneOne", call.Context);
                }
                else if (item.Relation == EnumTableRelation.OneMany)
                {
                    model.DisplayName = Data.Language.Hardcoded.GetValue("OneMany", call.Context);
                }
                else if (item.Relation == EnumTableRelation.ManyMany)
                {
                    model.DisplayName = Data.Language.Hardcoded.GetValue("ManyMany", call.Context);
                }
                else if (item.Relation == EnumTableRelation.ManyOne)
                {
                    model.DisplayName = Data.Language.Hardcoded.GetValue("ManyOne", call.Context);
                }

                result.Add(model);

            }

            return result.ToList<object>();
        }
    }

    public class TableRelationViewModel : TableRelation
    {
        public TableRelationViewModel(TableRelation relation)
        {
            this.FieldA = relation.FieldA;
            this.FieldB = relation.FieldB;
            this.Relation = relation.Relation;
            this.TableA = relation.TableA;
            this.TableB = relation.TableB;
        }

        public string DisplayName { get; set; }

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
