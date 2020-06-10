using Models.Harvest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IGetHarvestTimeEntries
    {                
        Task<List<TimeEntryFlattened>> GetImportantTimeEntries(TimeEntryFilter timeEntryFilter);

    }
}
