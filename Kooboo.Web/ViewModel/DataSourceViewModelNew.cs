//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.DataSources;
using Kooboo.Sites.DataSources.New.Models;
using Kooboo.Sites.Repository;

namespace Kooboo.Web.Areas.Admin.ViewModels
{
    public class DataSourceViewModel
    {
        public DataSourceViewModel()
        {
            this.Methods = new List<DataMethodViewModel>();
        }

        private Type _type;
        internal Type DataSourceType
        {
            get
            {
                if (_type == null)
                {
                    _type = Kooboo.Data.TypeCache.GetType(this.Type);
                }
                return _type;
            }
        }

        public string Type { get; set; }

        public string DisplayName
        {
            get
            {
                return this.DataSourceType?.Name;
            }
        }

        public Guid TypeHash { get; set; }

        public string Assembly
        {
            get
            {
                if (this.DataSourceType != null)
                {
                    return DataSourceType.Assembly.GetName().Name;
                }
                return null;
            }
        }

        public bool IsThridPartyDataSource { get; set; }

        public List<DataMethodViewModel> Methods { get; set; } = new List<DataMethodViewModel>();

        public void AddMethod(SiteDb siteDb, IDataMethodSetting dataMethod, bool isGlobal)
        {
            var typeInfo = DataSourceHelper.GetFields(siteDb, dataMethod);
            DataMethodViewModel model = new DataMethodViewModel
            {
                Id = dataMethod.Id,
                IsGlobal = isGlobal,
                DisplayName = dataMethod.DisplayName,
                Parameters = dataMethod.Parameters,
                MethodName = dataMethod.MethodName,
                OriginalMethodName = dataMethod.OriginalMethodName,
                TotalRelations = 10,
                MethodSignatureHash = dataMethod.MethodSignatureHash,
                Description = dataMethod.Description,
                DeclareType = dataMethod.DeclareType,
                IsNew = dataMethod.Id == Guid.Empty,
                IsPublic = dataMethod.IsPublic,
                ReturnType = dataMethod.ReturnType,
                IsPagedResult = dataMethod.IsPagedResult,
                IsConfigurable = dataMethod.Parameters.Any(),
                ItemFields = typeInfo.ItemFields,
                Enumerable = typeInfo.Enumerable
            };

            if (!dataMethod.IsPublic && !isGlobal)
            {

                var viewdatamethod = siteDb.ViewDataMethods.Query.Where(o => o.MethodId == dataMethod.Id).SelectAll();

                foreach (var item in viewdatamethod)
                {
                    if (item.ViewId != default(Guid))
                    {
                        var view = siteDb.Views.Get(item.ViewId, true);
                        if (view != null)
                        {
                            model.ViewId = view.Id;
                            model.ViewName = view.Name;
                        }
                        break;
                    }
                }
            }

            if (model.IsGlobal && model.Parameters.Any())
            {
                model.IsConfigurable = true;
            }
            Methods.Add(model);
        }

    }

    public class DataMethodViewModel
    {
        public DataMethodViewModel()
        {
            this.Parameters = new Dictionary<string, string>();
        }

        public Guid Id { get; set; } = default(Guid);

        public string MethodName { get; set; }

        public string DisplayName { get; set; }

        public string DeclareType { get; set; }

        public string ReturnType { get; set; }
        public bool IsPagedResult { get; set; }

        public Dictionary<string, string> BindingParas { get; set; } = new Dictionary<string, string>();

        public string OriginalMethodName { get; set; }

        public Guid ViewId { get; set; } = default(Guid);

        public string ViewName { get; set; }

        public Dictionary<string, int> Relations { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// whether this is a global method or not. Global method can not be configured. 
        /// </summary>
        public bool IsGlobal { get; set; }

        public bool IsConfigurable { get; set; }

        public int TotalRelations { get; set; }

        public Guid MethodSignatureHash { get; set; }

        public bool IsNew { get; set; }

        /// <summary>
        /// it's shared by global or only used in spefific view.
        /// </summary>
        public bool IsPublic { get; set; }

        public bool Enumerable { get; set; }

        /// <summary>
        /// this is description...
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        public Dictionary<string, ParameterBinding> ParameterBinding { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// return type
        /// </summary>
        public List<TypeFieldModel> Fields { get; set; }

        /// <summary>
        /// case return type is a collection,eg: List<Article>
        /// ItemFields need to describe the fields in Article
        /// </summary>
        public List<TypeFieldModel> ItemFields { get; set; }
    }

}
