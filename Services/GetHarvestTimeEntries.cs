using Models.Harvest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class GetHarvestTimeEntries : IGetHarvestTimeEntries
    {
        private readonly IGenericHTTPClient _genericHTTPClient;        
        public GetHarvestTimeEntries(IGenericHTTPClient genericHTTPClient)
        {
            _genericHTTPClient = genericHTTPClient;            
        }
        public async Task<List<TimeEntryFlattened>> GetImportantTimeEntries(TimeEntryFilter timeEntryFilter)
        {
            var timeEntries = await GetTimeEntries(timeEntryFilter);
            return timeEntries.time_entries
                 .Where(x => x.task_assignment.billable || x.task.name.Contains("PTO") || x.task.name.Contains("Holiday"))
                 .GroupBy(x => new
                 {
                     TimeEntryDate = DateTime.ParseExact(x.spent_date, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                 })
                 .Select(x => new 
                 TimeEntryFlattened
                 {
                     TimeEntryDate = x.Key.TimeEntryDate,
                     Hours = x.Sum(a => a.hours)
                 }).ToList();
        }

        private async Task<TimeEntry> GetTimeEntries(TimeEntryFilter timeEntryFilter)
        {
            var queryString = timeEntryFilter.GetDynamicURLWithQueryString();
            return await _genericHTTPClient.Get<TimeEntry>(Services.Enums.GenericHttpClientEnum.HarvestClient, queryString);
        }
    }
}
