using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio.Security;

namespace Services
{
    public class AuthorizeSendMessage : IAuthorizeSendMessage
    {
        private readonly IAzureKeyVaultService _azureKeyVaultService;
        public AuthorizeSendMessage(IAzureKeyVaultService azureKeyVaultService)
        {
            _azureKeyVaultService = azureKeyVaultService;
        }
        public async Task<bool> IsValidRequest(HttpRequest request)
        {
            var authToken = await _azureKeyVaultService.GetSecretByEnum(Enums.AzureKeyVaultEnum.TwilioKey);
            var requestValidator = new RequestValidator(authToken);
            var requestUrl = RequestRawUrl(request);
            var parameters = ToDictionary(request.Form);
            var signature = request.Headers["X-Twilio-Signature"];
            return requestValidator.Validate(requestUrl, parameters, signature);
        }

        private static string RequestRawUrl(HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        }

        private static IDictionary<string, string> ToDictionary(IFormCollection collection)
        {
            return collection.Keys
                .Select(key => new { Key = key, Value = collection[key] })
                .ToDictionary(p => p.Key, p => p.Value.ToString());
        }
    }
}
