using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Model.Render.ApiMeta;
using Kooboo.Model.Setting;

namespace Kooboo.Model.Render
{
    public class ApiMetaProvider : IApiMetaProvider
    {
        public ApiInfo GetMeta(string url)
        {
            var requiredRule = new ValidationRules.RequiredRule("Required");

            var minLengthRule = new ValidationRules.MinLengthRule(3, "username min length is {0}");
            var maxLengthRule = new ValidationRules.MaxLengthRule(20, "password max length is {0}");
            switch (url)
            {
                case "/user/login":
                    return new ApiInfo
                    {
                        Model = new ModelInfo
                        {
                            Properties = new List<PropertyInfo>
                                {
                                    new PropertyInfo { Name = "UserName", Rules = new List<ValidationRules.ValidationRule> { requiredRule,minLengthRule }, Type = typeof(String) },
                                    new PropertyInfo { Name = "Password", Rules = new List<ValidationRules.ValidationRule>(){ requiredRule,maxLengthRule}, Type = typeof(string) },
                                    new PropertyInfo { Name = "Remerber", Rules = new List<ValidationRules.ValidationRule>(), Type = typeof(bool) },
                                    new PropertyInfo { Name = "Returnurl", Rules = new List<ValidationRules.ValidationRule>(), Type = typeof(string) },
                                },
                            Type = typeof(ModelInfo)
                        },
                        Parameters = new List<PropertyInfo>
                            {
                                new PropertyInfo { Name = "userModel", Type = typeof(UserModelSetting) }
                            }
                    };
                case "/user/sameas":
                    var sameAs = new ValidationRules.SameAsRule("user.userName", "sameAs userName");
                    return new ApiInfo
                    {
                        Model = new ModelInfo
                        {
                            Properties = new List<PropertyInfo>
                                {
                                    new PropertyInfo { Name = "UserName", Rules = new List<ValidationRules.ValidationRule> {  }, Type = typeof(String) },
                                    new PropertyInfo { Name = "Password", Rules = new List<ValidationRules.ValidationRule>(){ sameAs}, Type = typeof(string) },
                                    new PropertyInfo { Name = "Remerber", Rules = new List<ValidationRules.ValidationRule>(), Type = typeof(bool) },
                                    new PropertyInfo { Name = "Returnurl", Rules = new List<ValidationRules.ValidationRule>(), Type = typeof(string) },
                                },
                            Type = typeof(ModelInfo)
                        },
                        Parameters = new List<PropertyInfo>
                            {
                                new PropertyInfo { Name = "userModel", Type = typeof(UserModelSetting) }
                            }
                    };

                case "layout/post":
                    return new ApiInfo
                    {
                        Model = new ModelInfo
                        {
                            Properties = new List<PropertyInfo>
                                {
                                    new PropertyInfo { Name = "Name", Rules = new List<ValidationRules.ValidationRule> { requiredRule }, Type = typeof(String) },
                                    new PropertyInfo { Name = "Pages", Rules = new List<ValidationRules.ValidationRule>(), Type = typeof(List<PropertyInfo>) },
                                },
                            Type = typeof(ModelInfo)
                        },
                        Parameters = new List<PropertyInfo>
                            {
                                new PropertyInfo { Name = "Id", Type = typeof(Guid) }
                            }
                    };
                case "layout/get":
                    return new ApiInfo
                    {
                        Result = new ModelInfo
                        {
                            Properties = new List<PropertyInfo>
                                {
                                    new PropertyInfo { Name = "Name", Rules = new List<ValidationRules.ValidationRule> { requiredRule }, Type = typeof(String) },
                                    new PropertyInfo { Name = "Pages", Rules = new List<ValidationRules.ValidationRule>(), Type = typeof(List<PropertyInfo>) },
                                },
                            Type = typeof(ModelInfo)
                        },
                        Parameters = new List<PropertyInfo>
                            {
                                new PropertyInfo { Name = "Id", Type = typeof(Guid) }
                            }
                    };
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
