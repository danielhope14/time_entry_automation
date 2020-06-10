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
    public class TwilioEnterTime
    {
        private readonly IGenericHTTPClient _genericHTTPClient;
        private readonly IHarvestURLBuilder _harvestURLBuilder;
        public TwilioEnterTime(IGenericHTTPClient genericHTTPClient, IHarvestURLBuilder harvestURLBuilder)
        {
            _genericHTTPClient = genericHTTPClient;
            _harvestURLBuilder = harvestURLBuilder;

        }
        [FunctionName("ParseTwilioEnteredTime")]
        [return: Queue("entertime")]
        public async Task<TimeEntryParsed> Run([QueueTrigger("parsetwiliotimeentry")]string myQueueItem, ILogger log)
        {
            var harvestURL = _harvestURLBuilder.GetHarvestURL(Services.Enums.HarvestHttpClientEnum.GetProjectAssignments);
            var projectAssignments = await _genericHTTPClient.Get<ProjectAssignment>(Services.Enums.GenericHttpClientEnum.HarvestClient, harvestURL);
            var hours = projectAssignments.project_assignments
                .Where(x => x.project.name == Environment.GetEnvironmentVariable("ProjectName"))
                .SelectMany(x => x.task_assignments
                .Select(a => new                
                {
                    TaskID = a.task.id,
                    ProjectID = x.project.id,
                    TaskName = a.task.name.Replace(" ", String.Empty).ToUpper()
                })).ToList();
            hours.AddRange(projectAssignments.project_assignments
                .Where(x => x.project.name != Environment.GetEnvironmentVariable("ProjectName"))
                .SelectMany(x => x.task_assignments
                .Select(a => new                
                {
                    TaskID = a.task.id,
                    ProjectID = x.project.id,
                    TaskName = x.project.name.Replace(" ", String.Empty).ToUpper().Substring(0, 3) + a.task.name.Replace(" ", String.Empty).ToUpper()
                })).ToList());
            var items = myQueueItem.ToUpper().Split(' ');
            if (items.Any(x => x.Any(char.IsDigit)) && items.Any(x => hours.Any(a => a.TaskName.Contains(x.ToUpper()))))
            {
                var taskAssignmnet = hours.FirstOrDefault(a => items.Contains(a.TaskName));
                var hour = items.Where(x => x.Any(char.IsDigit)).First();
                log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
                return new TimeEntryParsed
                {
                    IsFailedParse = false,
                    TimeEntryPost = new TimeEntryPost
                    {
                        project_id = taskAssignmnet.ProjectID,
                        task_id = taskAssignmnet.TaskID,
                        spent_date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                        hours = decimal.Parse(hour),
                    },
                };
            }
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            var failedTimeEntryString = "Time entry failed, please enter hours and one of the following categories" + Environment.NewLine;
            foreach (var item in hours)
            {
                failedTimeEntryString += item + Environment.NewLine;
            }
            return new TimeEntryParsed
            { IsFailedParse = true, FailedParseMessage = failedTimeEntryString };
            
        }
    }
}
