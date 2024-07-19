using System.Text.Json.Serialization;
using SpamassassinNet;

namespace Kooboo.ApiMarket
{
    public class TellCommandModel : CommandModel
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageClass? MessageClass { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DatabaseKind? Set { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DatabaseKind? Remove { get; set; }
    }

}
