namespace PlannerApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Graph;
    using Newtonsoft.Json;
    using PlannerApi.Models;
    using PlannerApi.Utils;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class PlannerController : ControllerBase
    {
        private readonly string baseAddress = "https://graph.microsoft.com/v1.0/me/";

        [HttpGet("plans")]
        public async Task<IEnumerable<IEnumerable<Plan>>> GetPlans() {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Authorization = Auth.GetAuthHeader(Request.Headers);

            var response = await client.GetAsync("drive/items/EB4D21CF97FBA497!11746/workbook/tables/plans/rows");
            var responseContent = await response.Content.ReadAsStringAsync();

            WorkbookTableRowsResponse rowsResponse = JsonConvert.DeserializeObject<WorkbookTableRowsResponse>(responseContent);

            return rowsResponse.GetPlans();
        }

        [HttpPost("plans")]
        public async Task<HttpResponseMessage> UpdatePlans(Plan[] plans) {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Authorization = Auth.GetAuthHeader(Request.Headers);

            var requestContent = new StringContent(JsonConvert.SerializeObject(
                new Value() {
                    Index = plan.Index,
                    Values = new object[][] {
                    new object[] { plan.Title, plan.Index, null, null},
                    }
                }
            ));

            return await client.PatchAsync($"drive/items/EB4D21CF97FBA497!11746/workbook/tables/plans/rows/$/ItemAt(index={plan.Index})", requestContent);
        }
    }
}
