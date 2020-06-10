using Models.Harvest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IGetHarvestProjects
    {
        Task<List<TaskProjectAssignmentGrouping>> GetTaskProjectAssignmentGroupings();
    }
}
