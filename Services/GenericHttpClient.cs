using Newtonsoft.Json;
using Services.Enums;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class GenericHttpClient : IGenericHTTPClient
    {
        private readonly HttpClient _client;
        private readonly IAzureKeyVaultService _azureKeyVaultService;
        public GenericHttpClient(IHttpClientFactory httpClientFactory, IAzureKeyVaultService azureKeyVaultService)
        {
            _client = httpClientFactory.CreateClient();
            _azureKeyVaultService = azureKeyVaultService;
        }
        public async Task<T> Get<T>(GenericHttpClientEnum genericHttpClientType, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);            
            await AddHeaders(request, genericHttpClientType);
            var httpResult = await _client.SendAsync(request);
            var stringResult = await httpResult.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResult);            
        }

        private async Task AddHeaders(HttpRequestMessage request, GenericHttpClientEnum genericHttpClientEnum)
        {
            switch (genericHttpClientEnum)
            {
                case GenericHttpClientEnum.HarvestClient:
                    var harvestKey = await _azureKeyVaultService.GetSecretByEnum(AzureKeyVaultEnum.HarvestKey);
                    request.Headers.Add("Authorization", "Bearer " + harvestKey);
                    request.Headers.Add("Harvest-Account-Id", Environment.GetEnvironmentVariable("HarvestAccountId"));
                    request.Headers.Add("User-Agent", "MyApp test");
                    return;
                case GenericHttpClientEnum.MicrosoftTokenRefreshClient:
                    request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");                                    
                    return;
                default:
                    return;
            }
        }

        public async Task<HttpResponseMessage> Post<T>(GenericHttpClientEnum genericHttpClientType, string url, T dataToPost)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            await AddHeaders(request, genericHttpClientType);            
            request.Content = new StringContent(JsonConvert.SerializeObject(dataToPost), Encoding.UTF8, "application/json");
            return await _client.SendAsync(request);            
        }

        public async Task<ReturnData> PostForm<ReturnData>(GenericHttpClientEnum genericHttpClientType, string url, FormUrlEncodedContent postData)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = postData
            };
            var httpResult = await _client.SendAsync(request);
            var stringResult = await httpResult.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReturnData>(stringResult);
        }

        public async Task<T> Get<T>(string url, string authToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", "Bearer " + authToken);
            var httpResult = await _client.SendAsync(request);
            var stringResult = await httpResult.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResult);
        }
    }
}
