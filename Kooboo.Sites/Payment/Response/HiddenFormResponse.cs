using Kooboo.Data.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Sites.Payment.Response
{
    public class HiddenFormResponse : IPaymentResponse
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public EnumResponseType Type   => EnumResponseType.hiddenform;
 
        public Guid requestId { get; set; }

        public string paymemtMethodReferenceId { get; set; }

        public bool hasHTML
        {
            get
            {
                return !string.IsNullOrEmpty(html);
            }
        }

        public string SubmitUrl { get; set; }

        [Description("HTTP method, POST or GET")]
        public string method { get; set; }

        private KScript.KDictionary _fieldvalues;
        public KScript.KDictionary fieldValues
        {
            get
            {
                if (_fieldvalues == null)
                {
                    _fieldvalues = new KScript.KDictionary();
                }
                return _fieldvalues;
            }
            set
            {
                _fieldvalues = value;
            }
        }

        public string html { get; set; }

        [KIgnore]
        public void setFieldValues(Dictionary<string, string> input)
        {
            foreach (var item in input)
            {
                setFieldValues(item.Key, item.Value);
            }
        }
        [KIgnore]
        public void setFieldValues(string key, string value)
        {
            this.fieldValues.Add(key, value);
        }
    }
}
