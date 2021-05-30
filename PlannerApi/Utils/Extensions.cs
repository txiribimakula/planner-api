using PlannerApi.Models;
using System;
using System.Collections.Generic;

namespace PlannerApi.Utils
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<Plan>> GetPlans(this WorkbookTableRowsResponse workbookTableRowsResponse) {
            List<Plan> plans = new List<Plan>();
            List<Plan> shortPlans = new List<Plan>();
            List<Plan> midPlans = new List<Plan>();
            List<Plan> longPlans = new List<Plan>();
            foreach (var row in workbookTableRowsResponse.Rows) {
                string planName = row.Values[0][0].ToString();
                plans.Add(new Plan(planName));
                shortPlans.Add(new Plan(planName, Convert.ToInt32(row.Values[0][1])));
                midPlans.Add(new Plan(planName, Convert.ToInt32(row.Values[0][2])));
                longPlans.Add(new Plan(planName, Convert.ToInt32(row.Values[0][3])));
            }

            IEnumerable<IEnumerable<Plan>> listsOfPlans = new List<IEnumerable<Plan>>() {
                plans, shortPlans, midPlans, longPlans
            };

            return listsOfPlans;
        }
    }
}
