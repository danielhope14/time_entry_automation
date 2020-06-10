using Services.Enums;

namespace Services
{
    public interface IHarvestURLBuilder
    {
        string GetHarvestURL(HarvestHttpClientEnum harvestHttpClientEnum);
    }
}
