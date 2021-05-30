namespace PlannerApi.Models
{
    public class Plan
    {
        public string Title { get; set; }

        public int Index { get; set; }

        public Plan(string title, int index) {
            Title = title;
            Index = index;
        }
    }
}
