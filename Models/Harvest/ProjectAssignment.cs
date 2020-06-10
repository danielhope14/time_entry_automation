using System;

namespace Models.Harvest
{

    public class ProjectAssignment : HarvestBase
    {
        public Project_Assignments[] project_assignments { get; set; }

    }
    public class Project_Assignments
    {
        public int id { get; set; }
        public bool is_project_manager { get; set; }
        public bool is_active { get; set; }
        public bool use_default_rates { get; set; }
        public object budget { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public decimal? hourly_rate { get; set; }
        public Project project { get; set; }
        public Client client { get; set; }
        public Task_Assignments[] task_assignments { get; set; }
    }
    public class Task_Assignments
    {
        public int id { get; set; }
        public bool billable { get; set; }
        public bool is_active { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public decimal? hourly_rate { get; set; }
        public object budget { get; set; }
        public HarvestTask task { get; set; }
    }
}
