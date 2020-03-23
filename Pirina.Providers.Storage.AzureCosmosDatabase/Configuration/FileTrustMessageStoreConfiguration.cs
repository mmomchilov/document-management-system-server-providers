using Pirina.Kernel.Data.Connection;
using Pirina.Providers.Storage.AzureCosmosDatabase.Resolver;

namespace Pirina.Providers.Storage.AzureCosmosDatabase.Configuration
{
    public class FileTrustMessageStoreConfiguration : ICosmosDbConfiguration
    {
        public string DatabaseId { get; private set; }
        public string EndPointUri { get; private set; }
        public string PrimaryKey { get; private set;  }

        public FileTrustMessageStoreConfiguration(IConnectionStringProvider<CosmosConnectionSettings> cosmosConnectionResolver)
        {
            var connectionSettings = cosmosConnectionResolver.GetConnectionString();

            DatabaseId = connectionSettings.DatabaseId;
            EndPointUri = connectionSettings.EndPointUri;
            PrimaryKey = connectionSettings.PrimaryKey;
        }
    }
}
