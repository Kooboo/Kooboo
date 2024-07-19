//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Reflection;
using Kooboo.Api;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class Helper : IApi
    {
        public string ModelName
        {
            get
            {
                return "help";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;
            }
        }

        public List<ApiViewModel> Get(ApiCall call)
        {
            List<ApiViewModel> result = new List<ApiViewModel>();
            foreach (var item in ApiContainer.List)
            {
                if (item.Key != this.ModelName)
                {
                    ApiViewModel model = new ApiViewModel();
                    model.Name = item.Key;
                    model.Url = "/_api/help/list/" + item.Key;

                    result.Add(model);
                }

            }

            return result;
        }

        public object List(ApiCall call)
        {
            string name = call.NameOrId;
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            List<ApiMethodViewModel> methods = new List<ApiMethodViewModel>();

            var api = ApiContainer.List[name];

            var allmethods = Kooboo.Lib.Reflection.TypeHelper.GetPublicMethods(api.GetType());

            foreach (var item in allmethods)
            {
                ApiMethodViewModel model = new ApiMethodViewModel();
                model.Name = item.Name;

                var requirepara = item.GetCustomAttribute(typeof(Kooboo.Attributes.RequireParameters));
                if (requirepara != null)
                {
                    var RequiredParas = requirepara as Attributes.RequireParameters;
                    model.Parameters = RequiredParas.Parameters;
                }

                var requiremodel = item.GetCustomAttribute(typeof(Kooboo.Attributes.RequireModel), true);
                if (requiremodel != null)
                {
                    model.RequireModel = true;
                    var required = requiremodel as Attributes.RequireModel;
                    var sampledata = Kooboo.Lib.Development.FakeData.GetFakeValue(required.ModelType);
                    model.UpdateModel = Lib.Helper.JsonHelper.Serialize(sampledata);
                }

                var defineReturnType = item.GetCustomAttribute(typeof(Kooboo.Attributes.ReturnType), true);

                Type returnType = null;

                if (defineReturnType != null)
                {
                    var definereturn = defineReturnType as Attributes.ReturnType;
                    returnType = definereturn.Type;
                }
                else
                {
                    returnType = item.ReturnType;

                    if (returnType == typeof(object) || returnType == typeof(List<object>))
                    {
                        Type modeltype;
                        if (api is ISiteObjectApi)
                        {
                            var siteobjectapi = api as ISiteObjectApi;
                            modeltype = siteobjectapi.ModelType;
                        }
                        else
                        {
                            modeltype = Kooboo.Lib.Reflection.TypeHelper.GetType(api.ModelName);
                        }

                        if (modeltype != null)
                        {
                            if (returnType == typeof(object))
                            {
                                returnType = modeltype;
                            }
                            else
                            {
                                var listType = typeof(List<>);
                                var constructedListType = listType.MakeGenericType(modeltype);
                                returnType = constructedListType;
                            }
                        }
                    }

                }


                if (returnType == typeof(void))
                {
                    model.ResponseModel = "";
                }
                else
                {
                    var SampleResponseData = Kooboo.Lib.Development.FakeData.GetFakeValue(returnType);
                    model.ResponseModel = Lib.Helper.JsonHelper.Serialize(SampleResponseData);
                }

                methods.Add(model);

            }

            return methods;

        }

    }

}
