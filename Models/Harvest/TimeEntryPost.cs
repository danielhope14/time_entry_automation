namespace Models.Harvest
{
    public class TimeEntryPost
    {            
            public int project_id { get; set; }
            public int task_id { get; set; }
            public string spent_date { get; set; }
            public decimal hours { get; set; }
    }
}
