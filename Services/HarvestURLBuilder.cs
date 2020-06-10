using Services.Enums;
using System;

namespace Services
{
    public class HarvestURLBuilder : IHarvestURLBuilder
    {
        public string GetHarvestURL(HarvestHttpClientEnum harvestHttpClientEnum)
        {
            var harvestBaseURL = Environment.GetEnvironmentVariable("HarvestBaseURL");
            switch (harvestHttpClientEnum)
            {
                case HarvestHttpClientEnum.GetTimeEntry:
                    return harvestBaseURL + "time_entries";                    
                case HarvestHttpClientEnum.PostTimeEntry:
                    return harvestBaseURL + "time_entries";
                case HarvestHttpClientEnum.PutTimeEntry:
                    break;
                case HarvestHttpClientEnum.GetProjectAssignments:
                    return $"{harvestBaseURL}users/me/project_assignments";
                default:
                    break;
            }
            return harvestBaseURL;
        }
    }
}
