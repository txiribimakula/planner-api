using Microsoft.Graph;
using PlannerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlannerApi.Utils
{
    public static class Extensions
    {
        public static Plan[][] GetPlans(this IWorkbookTableRowsCollectionPage workbookTableRowsResponse) {
            List<Plan> plans = new List<Plan>();
            List<Plan> shortPlans = new List<Plan>();
            List<Plan> midPlans = new List<Plan>();
            List<Plan> longPlans = new List<Plan>();
            foreach (var row in workbookTableRowsResponse) {
                string planName = row.Values[0][0].ToString();
                if (string.IsNullOrEmpty(planName)) {
                    break;
                }
                plans.Add(new Plan(planName, row.Index.Value));
                shortPlans.Add(new Plan(planName, Convert.ToInt32(row.Values[0][1])));
                midPlans.Add(new Plan(planName, Convert.ToInt32(row.Values[0][2])));
                longPlans.Add(new Plan(planName, Convert.ToInt32(row.Values[0][3])));
            }

            Plan[][] listsOfPlans = new Plan[][] {
                plans.ToArray(), shortPlans.ToArray(), midPlans.ToArray(), longPlans.ToArray()
            };

            return listsOfPlans;
        }

        public static IEnumerable<Models.Event> GetEvents(this IEnumerable<Microsoft.Graph.Event> eventsResponse) {
            var events = new List<Models.Event>();

            foreach (var item in eventsResponse) {
                events.Add(new Models.Event() {
                    Id = item.Id,
                    Title = item.Subject,
                    PlanTitle = item.Categories.Count() > 0 ? item.Categories.ElementAt(0) : null,
                    StartDate = item.Start.DateTime.GetDateTime(),
                    EndDate = item.End.DateTime.GetDateTime()
                });
            }

            return events;
        }

        public static Microsoft.Graph.Event GetEvent(this Models.Event @event) {
            return new Microsoft.Graph.Event {
                Subject = @event.Title,
                Start = new DateTimeTimeZone {
                    DateTime = @event.StartDate.GetString(),
                    TimeZone = "UTC"
                },
                End = new DateTimeTimeZone {
                    DateTime = @event.EndDate.GetString(),
                    TimeZone = "UTC"
                },
                Categories = new List<string> { 
                    @event.PlanTitle
                }
            };
        }

        public static DateTime GetDateTime(this string dateTimeText) {
            var startDateTime = dateTimeText.Split('T');
            var startDate = startDateTime[0];
            var startDateSplitted = startDate.Split('-');
            var startTime = startDateTime[1];
            var startTimeSplitted = startTime.Split(':');

            return new DateTime(int.Parse(startDateSplitted[0]), int.Parse(startDateSplitted[1]), int.Parse(startDateSplitted[2]), int.Parse(startTimeSplitted[0]) + 2, int.Parse(startTimeSplitted[1]), 0);
        }

        public static string GetString(this DateTime dateTime) {
            var year = dateTime.Year;
            var month = dateTime.Month.ToString("00");
            var day = dateTime.Day.ToString("00");
            var hour = dateTime.Hour.ToString("00");
            var minute = dateTime.Minute.ToString("00");
            
            return $"{year}-{month}-{day}T{hour}:{minute}";
        }
    }
}