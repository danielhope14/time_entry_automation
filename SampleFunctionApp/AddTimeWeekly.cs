using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Models;
using Models.Harvest;
using Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracking
{
    public class AddTimeWeekly
    {        
        private readonly IHarvestURLBuilder _harvestURLBuilder;
        private readonly IGetHarvestTimeEntries _getHarvestTimeEntries;
        private readonly ICreateTimeEntries _createTimeEntries;
        private string FromDate = DateTime.UtcNow.AddDays(-6).ToString("yyyy-MM-dd");
        private string ToDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        public AddTimeWeekly(ICreateTimeEntries createTimeEntries, IHarvestURLBuilder harvestURLBuilder, IGetHarvestTimeEntries getHarvestTimeEntries)
        {            
            _harvestURLBuilder = harvestURLBuilder;
            _getHarvestTimeEntries = getHarvestTimeEntries;
            _createTimeEntries = createTimeEntries;
        }
        [FunctionName("AddTimeWeekly")]
        [return: Queue("twiliosendmessage")]
        public async Task<TwilioMessage> Run([TimerTrigger("0 0 23 * * SUN", RunOnStartup = false)]TimerInfo myTimer, ILogger log)
        {
            
            var timeEntryFilter = new TimeEntryFilter
            {
                to = ToDate,
                from = FromDate,
                url = _harvestURLBuilder.GetHarvestURL(Services.Enums.HarvestHttpClientEnum.GetTimeEntry)
            };
            var importantEntries = await _getHarvestTimeEntries.GetImportantTimeEntries(timeEntryFilter);
            if (importantEntries.Sum(x => x.Hours) >= 40)
            {
                log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
                return new TwilioMessage
                {
                    Message = "40 hours of billable time entered for the week, please submit your week."
                };
            }
            var result = await _createTimeEntries.CreateTimeEntriesByRange(FromDate, ToDate, importantEntries);
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            return result;            
        }
    }
}
