namespace PlannerApi.Models
{
    using Newtonsoft.Json;

    public class WorkbookTableRowsResponse
    {
        [JsonProperty("value")]
        public Value[] Rows { get; set; }
    }

    public class Value
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("values")]
        public object[][] Values { get; set; }
    }

}
