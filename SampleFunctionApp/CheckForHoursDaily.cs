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
    public class CheckForHoursDaily
    {        
        private readonly IHarvestURLBuilder _harvestURLBuilder;
        private readonly IGetHarvestTimeEntries _getHarvestTimeEntries;
        public CheckForHoursDaily(IHarvestURLBuilder harvestURLBuilder, IGetHarvestTimeEntries getHarvestTimeEntries)
        {            
            _harvestURLBuilder = harvestURLBuilder;
            _getHarvestTimeEntries = getHarvestTimeEntries;

        }
        [FunctionName("CheckForHoursDaily")]
        [return: Queue("twiliosendmessage")]
        public async Task<TwilioMessage> Run([TimerTrigger("0 0 23 * * 1-5", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            var twilioMessage = new TwilioMessage();            
            var timeEntryFilter = new TimeEntryFilter
            {
                to = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                from = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                url = _harvestURLBuilder.GetHarvestURL(Services.Enums.HarvestHttpClientEnum.GetTimeEntry)
            };
            var timeEntries = await _getHarvestTimeEntries.GetImportantTimeEntries(timeEntryFilter);

            if (!timeEntries.Any())
            {
                twilioMessage.Message = "No time entry for today, reply with the number of hours you wish to enter for the day.";
            }
            else if (timeEntries.Sum(x => x.Hours) < 8)
            {
                twilioMessage.Message = "You have fewer than 8 billable hours for the day, if you worked more hours send that number of hours.";
            }
            else
            {
                twilioMessage.Message = "You have 8 billable hours entered for the day.";
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            return twilioMessage;
        }
    }
}
