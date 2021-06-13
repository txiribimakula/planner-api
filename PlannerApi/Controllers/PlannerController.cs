﻿namespace PlannerApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Graph;
    using Newtonsoft.Json.Linq;
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
        private readonly string FileId = "EB4D21CF97FBA497!11746";
        private readonly string PlansTableName = "plans";

        public GraphServiceClient GraphServiceClient { get; set; }

        public PlannerController(GraphServiceClient graphServiceClient) {
            GraphServiceClient = graphServiceClient;
        }

        [HttpGet("plans")]
        public async Task<Plan[][]> GetPlans() {
            var plansResponse = await GraphServiceClient.Me.Drive.Items[FileId].Workbook.Tables[PlansTableName].Rows
                .Request()
                .GetAsync();

            return plansResponse.GetPlans();
        }

        [HttpPost("plans")]
        public async Task<Plan[][]> UpdatePlans(Plan[][] plans) {
            bool isAnythingPostedOrDeleted = false;

            var currentListsOfPlans = await GetPlans();
            var currentPlans = currentListsOfPlans.ElementAt(0);

            for (int i = currentPlans.Length - 1; i >= 0; i--) {
                int index = Array.FindIndex(plans[0], plan => plan.Title == currentPlans[i].Title);
                if (index == -1) {
                    await GraphServiceClient.Me.Drive.Items[FileId].Workbook.Tables[PlansTableName].Rows[$"ItemAt(index={i})"]
                        .Request()
                        .DeleteAsync();
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
                var values = JToken.FromObject(plansToPost.Select(plan => new object[] {
                    plan.Title,
                    Array.Find(plans[1], shortPlan => shortPlan.Title == plan.Title).Index,
                    Array.Find(plans[2], midPlan => midPlan.Title == plan.Title).Index,
                    Array.Find(plans[3], longPlan => longPlan.Title == plan.Title).Index
                }));

                var postResponse = await GraphServiceClient.Me.Drive.Items[FileId].Workbook.Tables[PlansTableName].Rows
                    .Add(null, values)
                    .Request()
                    .PostAsync();
            }

            if (!isAnythingPostedOrDeleted) {
                for (int i = 0; i < plans[0].Length; i++) {
                    if (!plans[0][i].Equals(currentListsOfPlans[0][i])
                        || !Array.Find(plans[1], plan => plan.Title == plans[0][i].Title).Equals(currentListsOfPlans[1][i])
                        || !Array.Find(plans[2], plan => plan.Title == plans[0][i].Title).Equals(currentListsOfPlans[2][i])
                        || !Array.Find(plans[3], plan => plan.Title == plans[0][i].Title).Equals(currentListsOfPlans[3][i])) {
                        var values = JToken.FromObject(
                            new object[][] {
                                new object [] {
                                    plans[0][i].Title,
                                    Array.Find(plans[1], shortPlan => shortPlan.Title == plans[0][i].Title).Index,
                                    Array.Find(plans[2], midPlan => midPlan.Title == plans[0][i].Title).Index,
                                    Array.Find(plans[3], longPlan => longPlan.Title == plans[0][i].Title).Index
                                }
                            }
                        );

                        await GraphServiceClient.Me.Drive.Items[FileId].Workbook.Tables[PlansTableName].Rows[$"ItemAt(index={i})"]
                            .Request()
                            .UpdateAsync(new WorkbookTableRow() { Values = values });
                    }
                }
            }

            return await GetPlans();
        }

        [HttpGet("events")]
        public async Task<HttpResponseMessage> GetEvents() {
            var events = await GraphServiceClient.Me.Events
                .Request()
                .GetAsync();

            return new HttpResponseMessage();
        }
    }
}
