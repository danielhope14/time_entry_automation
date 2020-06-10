using Models;
using Models.Harvest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface ICreateTimeEntries
    {
        Task<TwilioMessage> CreateTimeEntriesByRange(string fromDate, string toDate, List<TimeEntryFlattened> existingTimeEntries);
    }
}
