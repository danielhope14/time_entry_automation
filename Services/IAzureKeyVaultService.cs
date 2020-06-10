using Services.Enums;
using System.Threading.Tasks;

namespace Services
{
    public interface IAzureKeyVaultService
    {
        Task<string> GetSecretByEnum(AzureKeyVaultEnum azureKeyVaultEnum);
    }
}
