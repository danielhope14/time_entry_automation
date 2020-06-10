using Models.Harvest;

namespace Models
{
    public class TimeEntryParsed
    {
        public TimeEntryParsed()
        {
            TimeEntryPost = new TimeEntryPost();
        }
        public TimeEntryPost TimeEntryPost { get; set; }
        public bool IsFailedParse { get; set; }
        public string FailedParseMessage { get; set; }
    }
}
