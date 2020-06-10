using System.Net.Http;
using System.Threading.Tasks;

namespace Services
{
    public interface ICreateMicrosoftTokenRefresh
    {
        Task<FormUrlEncodedContent> GetMicrosoftTokenRefreshData();
    }
}
