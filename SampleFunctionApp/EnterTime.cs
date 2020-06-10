using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Models;
using Models.Harvest;
using Services;
using System.Threading.Tasks;

namespace TimeTracking
{
    public class EnterTime
    {
        private readonly IGenericHTTPClient _genericHTTPClient;
        private readonly IHarvestURLBuilder _harvestURLBuilder;
        public EnterTime(IGenericHTTPClient genericHTTPClient, IHarvestURLBuilder harvestURLBuilder)
        {
            _genericHTTPClient = genericHTTPClient;
            _harvestURLBuilder = harvestURLBuilder;

        }
        [FunctionName("EnterTime")]
        [return: Queue("twiliosendmessage")]
        public async Task<TwilioMessage> Run([QueueTrigger("entertime")]TimeEntryParsed timeEntryParsed, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: ");
            if (timeEntryParsed.IsFailedParse)
            {
                return new TwilioMessage
                {
                    Message = timeEntryParsed.FailedParseMessage,
                };
            }

            var harvestURL = _harvestURLBuilder.GetHarvestURL(Services.Enums.HarvestHttpClientEnum.PostTimeEntry);
            var result = await _genericHTTPClient.Post<TimeEntryPost>(Services.Enums.GenericHttpClientEnum.HarvestClient, harvestURL, timeEntryParsed.TimeEntryPost);
            if(!result.IsSuccessStatusCode)
            {
                return new TwilioMessage
                {
                    Message = "Time entry failed. " + result.Content,
                };
            }

            return new TwilioMessage
            {
                Message = "Time successfully entered.",
            };
            
        }
    }
}
