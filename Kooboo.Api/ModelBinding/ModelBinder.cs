//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooboo.Api.ModelBinding
{
    public class ModelBinder
    {
        public static BindingResponse TryBind(string Json, Type ObjectType)
        {
            try
            {
                var data = Lib.Helper.JsonHelper.Deserialize(Json, ObjectType);

                var context = new ValidationContext(data);
                var results = new List<ValidationResult>();
                var isSuccess = Validator.TryValidateObject(data, context, results, true);

                if (!isSuccess)
                {
                    var messages = new Dictionary<string, string>();
                    foreach (var result in results)
                    {
                        foreach (var name in result.MemberNames)
                        {
                            messages.Add(name, result.ErrorMessage);
                        }
                    }
                    return new BindingResponse()
                    {
                        IsSuccess = false,
                        FieldMessage = messages
                    };
                }
                return new BindingResponse()
                {
                    IsSuccess = true,
                    Model = data
                };
            }
            catch (Exception e)
            {
                return new BindingResponse()
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
    }
}
