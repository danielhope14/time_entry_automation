using Services.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services
{
    public class CreateMicrosoftTokenRefresh : ICreateMicrosoftTokenRefresh
    {
        private readonly IAzureKeyVaultService _azureKeyVaultService;
        public CreateMicrosoftTokenRefresh(IAzureKeyVaultService azureKeyVaultService)
        {
            _azureKeyVaultService = azureKeyVaultService;
        }
        public async Task<FormUrlEncodedContent> GetMicrosoftTokenRefreshData()
        {
            var formData = new Dictionary<string, string>();
            var refreshToken = await _azureKeyVaultService.GetSecretByEnum(AzureKeyVaultEnum.Office365RefreshToken);
            var appSecret = await _azureKeyVaultService.GetSecretByEnum(AzureKeyVaultEnum.Office365AppSecret);
            formData.Add("client_id", Environment.GetEnvironmentVariable("MicrosoftAccountId"));
            formData.Add("scope", "offline_access calendars.read calendars.read.shared");
            formData.Add("grant_type", "refresh_token");
            formData.Add("refresh_token", refreshToken);
            formData.Add("client_secret", appSecret);
            return new FormUrlEncodedContent(formData);
        }
    }
}
