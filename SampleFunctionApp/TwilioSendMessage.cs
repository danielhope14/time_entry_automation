using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Models;
using Services;
using Services.Enums;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace TimeTracking
{
    public class TwilioSendMessage
    {
        private readonly IAzureKeyVaultService _azureKeyVaultService;
        public TwilioSendMessage(IAzureKeyVaultService azureKeyVaultService)
        {
            _azureKeyVaultService = azureKeyVaultService;
        }
        [FunctionName("TwilioSendMessage")]
        public async Task Run([QueueTrigger("twiliosendmessage")]TwilioMessage myQueueItem, ILogger log)
        {
            var twilioKey = await _azureKeyVaultService.GetSecretByEnum(AzureKeyVaultEnum.TwilioKey);            
            TwilioClient.Init(Environment.GetEnvironmentVariable("TwilioAccountSID"), twilioKey);
            var message = MessageResource.Create(
            body: myQueueItem.Message,
            from: new Twilio.Types.PhoneNumber(myQueueItem.FromNumber),
            to: new Twilio.Types.PhoneNumber(myQueueItem.ToNumber)
            );
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            return;
        }
    }
}
