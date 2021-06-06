﻿namespace PlannerApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
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
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Authorization = Auth.GetAuthHeader(Request.Headers);

            var response = await client.GetAsync("drive/items/EB4D21CF97FBA497!11746/workbook/tables/plans/rows");
            var responseContent = await response.Content.ReadAsStringAsync();

            WorkbookTableRowsResponse rowsResponse = JsonConvert.DeserializeObject<WorkbookTableRowsResponse>(responseContent);

            return rowsResponse.GetPlans();
        }

        [HttpPost("plans")]
        public async Task<Plan[][]> UpdatePlans(Plan[][] plans) {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Authorization = Auth.GetAuthHeader(Request.Headers);

            var currentPlans = (await GetPlans()).ElementAt(0);

            List<int> indexesToDelete = new List<int>();
            for (int i = currentPlans.Length - 1; i >= 0; i--) {
                int index = Array.FindIndex(plans[0], plan => plan.Title == currentPlans[i].Title);
                if (index == -1) {
                    await client.DeleteAsync($"drive/items/EB4D21CF97FBA497!11746/workbook/tables/plans/rows/$/ItemAt(index={indexesToDelete[i]})");
                }
            }

            List<Plan> plansToPost = new List<Plan>();
            foreach (var plan in plans[0]) {
                int index = Array.FindIndex(currentPlans, currentPlan => currentPlan.Title == plan.Title);
                if (index == -1) {
                    plansToPost.Add(plan);
                }
            }
            if(plansToPost.Count() > 0) {
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

            return await GetPlans();
        }
    }
}
