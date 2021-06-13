using System;

namespace PlannerApi.Models
{
    public class Event
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string PlanTitle { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
