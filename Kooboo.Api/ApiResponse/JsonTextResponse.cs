using System.Collections.Generic;

namespace Kooboo.Api.ApiResponse
{
    public class JsonTextResponse : IResponse
    {
        public JsonTextResponse(string Json)
        {
            this.Model = Json;
        }

        public JsonTextResponse()
        {

        }

        public object Model { get; set; }

        public bool DataChange { get; set; }

        public bool Success { get; set; } = true;

        public int? HttpCode { get; set; } = 200;

        public List<string> Messages { get; set; } = new List<string>();

        public List<FieldError> FieldErrors { get; set; } = new List<FieldError>();

    }





}
