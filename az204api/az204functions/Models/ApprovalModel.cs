using Newtonsoft.Json;

namespace az204functions.Models
{
    public class ApprovalModel
    {
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; } = "Approval";
    }
}
