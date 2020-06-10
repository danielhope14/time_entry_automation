using Microsoft.Azure.KeyVault;
using Services.Enums;
using System;
using System.Threading.Tasks;

namespace Services
{

    public class AzureKeyVaultService : IAzureKeyVaultService
    {
        private readonly IKeyVaultClient _keyVaultClient;
        public AzureKeyVaultService(IKeyVaultClient keyVaultClient)
        {
            _keyVaultClient = keyVaultClient;
        }
        public async Task<string> GetSecretByEnum(AzureKeyVaultEnum azureKeyVaultEnum)
        {
            var azureKeyVaultKey = azureKeyVaultEnum.ToString();
            var key = await _keyVaultClient.GetSecretAsync(Environment.GetEnvironmentVariable("MyKeyVaultURL"), azureKeyVaultKey);
            return key.Value;
        }
    }
}
