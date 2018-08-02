using Newtonsoft.Json;

namespace SlackDeleter.Models
{
    class DeletePayload
    {
        [JsonProperty("file")]
        public string FileId { get; set; }
    }
}
