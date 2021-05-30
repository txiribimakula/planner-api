namespace PlannerApi.Models
{
    using Newtonsoft.Json;

    interface ISheetRange
    {
        [JsonProperty("values")]
        public object[][] Values { get; set; }
    }
}
