using Models.Harvest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class GetHarvestProjects : IGetHarvestProjects
    {
        private readonly IGenericHTTPClient _genericHTTPClient;
        private readonly IHarvestURLBuilder _harvestURLBuilder;
        public GetHarvestProjects(IGenericHTTPClient genericHTTPClient, IHarvestURLBuilder harvestURLBuilder)
        {
            _genericHTTPClient = genericHTTPClient;
            _harvestURLBuilder = harvestURLBuilder;
        }
        public async Task<List<TaskProjectAssignmentGrouping>> GetTaskProjectAssignmentGroupings()
        {            
            var projectAssignments = await _genericHTTPClient
                .Get<ProjectAssignment>(
                Enums.GenericHttpClientEnum.HarvestClient, 
                _harvestURLBuilder.GetHarvestURL(Enums.HarvestHttpClientEnum.GetProjectAssignments));

            return projectAssignments.project_assignments
                .SelectMany(x => x.task_assignments
                .Select(a => new TaskProjectAssignmentGrouping
                {
                    TaskID = a.task.id,
                    ProjectID = x.project.id,
                    TaskName = a.task.name.ToUpper(),
                    ProjectName = x.project.name.ToUpper(),
                })).ToList();
        }
    }
}
