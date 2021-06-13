namespace PlannerApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Graph;
    using Newtonsoft.Json;
    using PlannerApi.Models;
    using PlannerApi.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class PlannerController : ControllerBase
    {
        private readonly string baseAddress = "https://graph.microsoft.com/v1.0/me/";

        [HttpGet("plans")]
        public async Task<Plan[][]> GetPlans() {
            GraphServiceClient graphClient = new GraphServiceClient(new AuthenticationProvider(Auth.GetAuthHeader(Request.Headers)));

            var plansResponse = await graphClient.Me.Drive.Items["EB4D21CF97FBA497!11746"].Workbook.Tables["plans"].Rows
                .Request()
                .GetAsync();

            return plansResponse.GetPlans();
        }

        [HttpPost("plans")]
        public async Task<Plan[][]> UpdatePlans(Plan[][] plans) {
            bool isAnythingPostedOrDeleted = false;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Authorization = Auth.GetAuthHeader(Request.Headers);

            var currentListsOfPlans = await GetPlans();
            var currentPlans = currentListsOfPlans.ElementAt(0);

            for (int i = currentPlans.Length - 1; i >= 0; i--) {
                int index = Array.FindIndex(plans[0], plan => plan.Title == currentPlans[i].Title);
                if (index == -1) {
                    await client.DeleteAsync($"drive/items/EB4D21CF97FBA497!11746/workbook/tables/plans/rows/$/ItemAt(index={index})");
                    isAnythingPostedOrDeleted = true;
                }
            }

            List<Plan> plansToPost = new List<Plan>();
            foreach (var plan in plans[0]) {
                int index = Array.FindIndex(currentPlans, currentPlan => currentPlan.Title == plan.Title);
                if (index == -1) {
                    plansToPost.Add(plan);
                    isAnythingPostedOrDeleted = true;
                }
            }
            if (plansToPost.Count() > 0) {
                var serialized = JsonConvert.SerializeObject(
                    new Value() {
                        Values = plansToPost.Select(plan => new object[] {
                            plan.Title,
                            Array.Find(plans[1], shortPlan => shortPlan.Title == plan.Title).Index,
                            Array.Find(plans[2], midPlan => midPlan.Title == plan.Title).Index,
                            Array.Find(plans[3], longPlan => longPlan.Title == plan.Title).Index
                        }).ToArray()
                    }
                );
                var postRequestContent = new StringContent(serialized);
                var postResponse = await client.PostAsync($"drive/items/EB4D21CF97FBA497!11746/workbook/tables/plans/rows/add", postRequestContent);
                var postResponseContent = await postResponse.Content.ReadAsStringAsync();

                WorkbookTableRowsResponse rowsResponse = JsonConvert.DeserializeObject<WorkbookTableRowsResponse>(postResponseContent);
            }

            if (!isAnythingPostedOrDeleted) {
                for (int i = 0; i < plans[0].Length; i++) {
                    if (!plans[0][i].Equals(currentListsOfPlans[0][i])
                        || !Array.Find(plans[1], plan => plan.Title == plans[0][i].Title).Equals(currentListsOfPlans[1][i])
                        || !Array.Find(plans[2], plan => plan.Title == plans[0][i].Title).Equals(currentListsOfPlans[2][i])
                        || !Array.Find(plans[3], plan => plan.Title == plans[0][i].Title).Equals(currentListsOfPlans[3][i])) {
                        var serialized = JsonConvert.SerializeObject(
                        new Value() {
                            Values = new object[][] {
                            new object [] {
                                plans[0][i].Title,
                                Array.Find(plans[1], shortPlan => shortPlan.Title == plans[0][i].Title).Index,
                                Array.Find(plans[2], midPlan => midPlan.Title == plans[0][i].Title).Index,
                                Array.Find(plans[3], longPlan => longPlan.Title == plans[0][i].Title).Index
                            }
                            }
                        });
                        var patchRequestContent = new StringContent(serialized);
                        await client.PatchAsync($"drive/items/EB4D21CF97FBA497!11746/workbook/tables/plans/rows/$/ItemAt(index={i})", patchRequestContent);
                    }
                }
            }

            return await GetPlans();
        }

        [HttpGet("events")]
        public async Task<HttpResponseMessage> GetEvents() {
            GraphServiceClient graphClient = new GraphServiceClient(new AuthenticationProvider(Auth.GetAuthHeader(Request.Headers)));

            var events = await graphClient.Me.Events
                .Request()
                .GetAsync();

            return new HttpResponseMessage();
        }
    }
}
