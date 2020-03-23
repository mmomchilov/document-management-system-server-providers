using System;
using System.Threading.Tasks;
using Glasswall.Kernel.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Glasswall.Providers.Storage.AzureBlob.Connection
{
    public class BlobConnection : IStorageConnection<CloudBlobContainer, Guid>
    {
        private readonly IStorageConnectionManager<CloudBlobClient> _storageConnectionManager;

        public BlobConnection(IStorageConnectionManager<CloudBlobClient> storageConnectionManager)
        {
            _storageConnectionManager = storageConnectionManager ?? throw new ArgumentNullException(nameof(storageConnectionManager));
        }

        public async Task<CloudBlobContainer> GetObjectAsync(Guid Id)
        {
            if (Id == Guid.Empty) throw new ArgumentException($"{nameof(Id)} cannot be Empty");

            var account = await _storageConnectionManager.GetStorageClient();
            var container = account.GetContainerReference(Id.ToString());
            await container.CreateIfNotExistsAsync();
            return container;
        }

        public async Task RemoveObjectAsync(Guid Id)
        {
            if (Id == Guid.Empty) throw new ArgumentException($"{nameof(Id)} cannot be Empty");

            var account = await _storageConnectionManager.GetStorageClient();
            var container = account.GetContainerReference(Id.ToString());
            await container.DeleteIfExistsAsync();
        }
    }
}
