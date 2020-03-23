using System;
using System.Threading.Tasks;
using Glasswall.Kernel.Storage;
using Glasswall.Providers.Storage.AzureBlob.Exceptions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Glasswall.Providers.Storage.AzureBlob.Connection
{
    public class BlobConnectionManager : IStorageConnectionManager<CloudBlobClient>
    {
        private readonly IStorageConfiguration _storageConfiguration;
        private CloudBlobClient _cloudBlobClient;

        public BlobConnectionManager(Func<IStorageConfiguration> storageConfigurationFactory)
        {
            if (storageConfigurationFactory == null) throw new ArgumentNullException(nameof(storageConfigurationFactory));
            _storageConfiguration = storageConfigurationFactory() ?? throw new ArgumentNullException(nameof(storageConfigurationFactory));
        }

        public async Task<CloudBlobClient> GetStorageClient()
        {
            if (_cloudBlobClient == null)
                await Connect();
            return _cloudBlobClient;
        }

        private Task Connect()
        {
            var connectionString = _storageConfiguration.ConnectionString;
            if (!CloudStorageAccount.TryParse(connectionString, out var account))
                throw new BlobStorageConnectionError($"Was unable to parse the connection string {connectionString}");

            _cloudBlobClient = account.CreateCloudBlobClient();
            return Task.CompletedTask;
        }
    }
}
