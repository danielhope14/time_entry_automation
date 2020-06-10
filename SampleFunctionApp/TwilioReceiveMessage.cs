using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Services;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracking
{
    public class TwilioReceiveMessage
    {
        private readonly IAuthorizeSendMessage _authorizeSendMessage;
        public TwilioReceiveMessage(IAuthorizeSendMessage authorizeSendMessage)
        {
            _authorizeSendMessage = authorizeSendMessage;
        }
        [FunctionName("TwilioReceiveMessage")]
        [return: Queue("parsetwiliotimeentry")]
        public async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ReceiveSMS")] HttpRequest req,
            ILogger log)
        {
            var isAuthorized = await _authorizeSendMessage.IsValidRequest(req);
            if (!isAuthorized)
            {
                var unathorizedMessage = "Received Unathorized Request on Twilio Time Entry Service.";
                log.LogInformation(unathorizedMessage);
                throw new System.Exception(unathorizedMessage);
            }
            var formData = await req.ReadFormAsync();
            var messageBody = formData.FirstOrDefault(x => x.Key == "Body").Value.ToString();

            log.LogInformation("Received Twilio Time Entry.");
            return messageBody;
        }
    }
}
