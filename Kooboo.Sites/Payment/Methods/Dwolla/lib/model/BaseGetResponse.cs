using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    public interface IEmbeddedResponse<T>
    {
        IEmbed<T> Embedded { get; set; }
    }

    public abstract class BaseGetResponse<T> : BaseResponse, IEmbeddedResponse<T>
    {
        [JsonProperty(PropertyName = "_embedded")]
        public IEmbed<T> Embedded { get; set; }

        public int Total { get; set; }
    }

    public interface IEmbed<T>
    {
        //List<Error> Errors { get; set; }
        List<T> Results();
    }

    public abstract class Embed<T> : IEmbed<T>
    {
        //public List<Error> Errors { get; set; }

        public abstract List<T> Results();
    }
}
