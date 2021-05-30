namespace PlannerApi.Models
{
    public class Plan
    {
        public string Title { get; set; }

        public int Value { get; set; }

        public Plan(string title, int value = 0) {
            Title = title;
            Value = value;
        }
    }
}
