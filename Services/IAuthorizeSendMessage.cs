using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Services
{
    public interface IAuthorizeSendMessage
    {
        Task<bool> IsValidRequest(HttpRequest httpRequest); 
    }
}
