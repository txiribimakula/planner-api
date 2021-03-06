namespace PlannerApi.Models
{
    public class Plan
    {
        public string Title { get; set; }

        public int Index { get; set; }

        public int Value { get => (3 - Index) < 0 ? 0 : (3 - Index); }

        public Plan(string title, int index) {
            Title = title;
            Index = index;
        }

        public override bool Equals(object obj) {
            Plan planToCompare = (Plan)obj;
            return this.Title == planToCompare.Title && this.Index == planToCompare.Index;
        }

        public override int GetHashCode() {
            return this.Title.GetHashCode() + this.Index.GetHashCode();
        }
    }
}
