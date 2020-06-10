using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IGetOffice365Events
    {
        Task<List<MicrosoftEventFlattened>> GetOffice365EventsFlattened(string fromDate, string toDate);
    }
}
