using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using Services;

[assembly: FunctionsStartup(typeof(TimeTracking.Startup))]
namespace TimeTracking
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var azureServicesTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServicesTokenProvider.KeyVaultTokenCallback));
            builder.Services.AddSingleton<IKeyVaultClient>(keyVaultClient);
            builder.Services.AddHttpClient();
            builder.Services.AddTransient<IAzureKeyVaultService, AzureKeyVaultService>();
            builder.Services.AddTransient<IHarvestURLBuilder, HarvestURLBuilder>();
            builder.Services.AddTransient<IGenericHTTPClient, GenericHttpClient>();
            builder.Services.AddTransient<IAuthorizeSendMessage, AuthorizeSendMessage>();
            builder.Services.AddTransient<ICreateMicrosoftTokenRefresh, CreateMicrosoftTokenRefresh>();
            builder.Services.AddTransient<IGetHarvestTimeEntries, GetHarvestTimeEntries>();
            builder.Services.AddTransient<IGetHarvestProjects, GetHarvestProjects>();
            builder.Services.AddTransient<IGetOffice365Events, GetOffice365Events>();
            builder.Services.AddTransient<ICreateTimeEntries, CreateTimeEntries>();
            
        }
    }
}
