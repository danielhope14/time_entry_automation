using Services.Enums;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services
{
    public interface IGenericHTTPClient
    {
        Task<T> Get<T>(GenericHttpClientEnum genericHttpClientType, string url);
        Task<T> Get<T>(string url, string authToken);
        Task<HttpResponseMessage> Post<T>(GenericHttpClientEnum genericHttpClientType, string url, T dataToPost);
        Task<ReturnData> PostForm<ReturnData>(GenericHttpClientEnum genericHttpClientType, string url, FormUrlEncodedContent postData);

    }
}
