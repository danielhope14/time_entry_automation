namespace Models.Harvest
{
    public class HarvestBase
    {
        public int per_page { get; set; }
        public int total_pages { get; set; }
        public int total_entries { get; set; }
        public object next_page { get; set; }
        public object previous_page { get; set; }
        public int page { get; set; }
        public Links links { get; set; }

    }
    public class Links
    {
        public string first { get; set; }
        public object next { get; set; }
        public object previous { get; set; }
        public string last { get; set; }
    }
}
