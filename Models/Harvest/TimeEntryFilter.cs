using Utilities;

namespace Models.Harvest
{
    public class TimeEntryFilter : QueryStringBase
    {
        public int? user_id { get; set; }
        public int? client_id { get; set; }
        public int? project_id { get; set; }
        public bool? is_billed { get; set; }
        public bool? is_running { get; set; }
        public string updated_since { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public int page { get; set; } = 1;
        public int per_page { get; set; } = 100;        
    }
}
