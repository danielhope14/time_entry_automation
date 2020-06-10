using System;

namespace Models.Harvest
{
    public class TimeEntry : HarvestBase
    {
        public Time_Entries[] time_entries { get; set; }
    }

    public class Time_Entries
    {
        public int id { get; set; }
        public string spent_date { get; set; }
        public User user { get; set; }
        public Client client { get; set; }
        public Project project { get; set; }
        public HarvestTask task { get; set; }
        public User_Assignment user_assignment { get; set; }
        public Task_Assignment task_assignment { get; set; }
        public decimal hours { get; set; }
        public string notes { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool is_locked { get; set; }
        public string locked_reason { get; set; }
        public bool is_closed { get; set; }
        public bool is_billed { get; set; }
        public object timer_started_at { get; set; }
        public string started_time { get; set; }
        public string ended_time { get; set; }
        public bool is_running { get; set; }
        public Invoice invoice { get; set; }
        public object external_reference { get; set; }
        public bool billable { get; set; }
        public bool budgeted { get; set; }
        public decimal? billable_rate { get; set; }
        public decimal? cost_rate { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class User_Assignment
    {
        public int id { get; set; }
        public bool is_project_manager { get; set; }
        public bool is_active { get; set; }
        public object budget { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public decimal? hourly_rate { get; set; }
    }

    public class Task_Assignment
    {
        public int id { get; set; }
        public bool billable { get; set; }
        public bool is_active { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public decimal? hourly_rate { get; set; }
        public object budget { get; set; }
    }

    public class Invoice
    {
        public int id { get; set; }
        public string number { get; set; }
    }

}
