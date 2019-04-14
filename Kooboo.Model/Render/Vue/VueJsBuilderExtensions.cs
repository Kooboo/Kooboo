using System;
using System.Collections.Generic;

using Kooboo.Model.Render.Vue;

namespace Kooboo.Model.Render
{
    public static class VueJsBuilderExtensions
    {
        public static IJsBuilder Load(this IJsBuilder builder, string url, string modelName)
        {
            return builder.AddItem(new Vue.LoadData { Url = url, ModelName = modelName });
        }

        public static IJsBuilder Load(this IJsBuilder builder)
        {
            return builder.AddItem(new Vue.LoadData());
        }

        public static IJsBuilder Data(this IJsBuilder builder, string name, string json)
        {
            return builder.AddItem(new Vue.Data { Name = name, Json = json });
        }

        public static IJsBuilder Submit(this IJsBuilder builder, string url, string modelName)
        {
            return builder.AddItem(new Vue.SubmitData { Url = url, ModelName = modelName });
        }

        public static IJsBuilder Validation(this IJsBuilder builder, string name,List<Validation> validations)
        {
            return builder.AddItem(new Vue.Validation { Name = name,Validations=validations});
        }

        public static IJsBuilder El(this IJsBuilder builder, string name)
        {
            return builder.AddItem(new Vue.El { Name = name });
        }

        public static IJsBuilder Component(this IJsBuilder builder, string name, string js)
        {
            return builder.AddItem(new Vue.Component { Name = name, Js = js });
        }
    }
}
