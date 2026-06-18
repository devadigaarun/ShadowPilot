namespace ShadowPilot
{
    public class TfsWorkItemSummary
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string State { get; set; }

        public string WorkItemType { get; set; }

        public string AssignedTo { get; set; }

        public string Url { get; set; }

        public override string ToString()
        {
            return $"#{Id} [{WorkItemType}] {Title}";
        }
    }
}