namespace PlannerApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;
    using PlannerApi.Utils;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class PlannerController : ControllerBase
    {
        private readonly string baseAddress = "https://graph.microsoft.com/v1.0/me/";

        [HttpGet("plans")]
        public async Task<string> GetPlans() {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);

            client.DefaultRequestHeaders.Authorization = Auth.GetAuthHeader(Request.Headers);

            var response = await client.GetAsync("drive/items/EB4D21CF97FBA497!11746/workbook/worksheets('plans')/usedRange");
            var responseContent = await response.Content.ReadAsStringAsync();

            var responseObject = JObject.Parse(responseContent);
            var responseObjectValues = responseObject.SelectToken("values");

            return responseObjectValues.ToString();
        }
    }
}
