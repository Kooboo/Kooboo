using KScript;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Sites.Payment.Response
{
    public class RequireDataResponse : IPaymentResponse
    {
        public Guid requestId { get; set; }

        public string paymemtMethodReferenceId { get; set; }

        [Description("Current data set that should be submitted back again")]
        public KDictionary Data { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnumResponseType Type => EnumResponseType.submitdata;

        public List<DataFields> RequireData { get; set; }
    }
}
